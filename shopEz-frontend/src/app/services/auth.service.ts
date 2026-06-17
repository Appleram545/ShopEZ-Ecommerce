import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { User, LoginRequest, RegisterRequest } from "../models/user";
import { environment } from "../../environments/environment";

@Injectable({ providedIn: "root" })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(
    this.getStoredUser(),
  );
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  private getStoredUser(): User | null {
    const stored = localStorage.getItem("shopez_user");
    return stored ? JSON.parse(stored) : null;
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }
  get isLoggedIn(): boolean {
    return !!localStorage.getItem("shopez_user");
  }

  get isAdmin(): boolean {
    const role = this.currentUserSubject.value?.role;
    if (!role) return false;
    return role.toLowerCase() === "admin";
  }

  private decodeToken(token: string): any {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split("")
          .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
          .join(""),
      );
      return JSON.parse(jsonPayload);
    } catch {
      return {};
    }
  }

  login(credentials: LoginRequest): Observable<{ token: string }> {
    return this.http
      .post<{ token: string }>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap((response) => {
          const token = response.token;
          const decoded = this.decodeToken(token);
          const rawRole =
            decoded["role"] ||
            decoded["Role"] ||
            decoded[
              "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            ] ||
            decoded[
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"
            ] ||
            "customer";
          const userId =
            decoded["nameid"] ||
            decoded["sub"] ||
            decoded[
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            ] ||
            decoded["userId"] ||
            decoded["id"];
          const userName =
            decoded["unique_name"] ||
            decoded["name"] ||
            decoded[
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            ] ||
            decoded["email"] ||
            credentials.email;
          const user: User = {
            id: userId,
            name: userName,
            email: decoded["email"] || credentials.email,
            role: rawRole.toLowerCase() as "admin" | "customer",
            token: token,
          };
          localStorage.setItem("shopez_user", JSON.stringify(user));
          this.currentUserSubject.next(user);
        }),
      );
  }

  register(data: RegisterRequest): Observable<{ token: string }> {
    return this.http
      .post<{ token: string }>(`${this.apiUrl}/register`, data)
      .pipe(
        tap((response) => {
          const token = response.token;

          const decoded = this.decodeToken(token);

          const rawRole =
            decoded["role"] ||
            decoded["Role"] ||
            decoded[
              "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            ] ||
            "customer";

          const userId = decoded["nameid"] || decoded["sub"] || decoded["id"];

          const userName =
            decoded["unique_name"] || decoded["name"] || decoded["email"];

          const user: User = {
            id: userId,
            name: userName,
            email: decoded["email"] || data.email,
            role: rawRole.toLowerCase() as "admin" | "customer",
            token: token,
          };

          localStorage.setItem("shopez_user", JSON.stringify(user));

          this.currentUserSubject.next(user);
        }),
      );
  }

  logout(): void {
    localStorage.removeItem("shopez_user");
    this.currentUserSubject.next(null);

    //  logout event so cart can clear itself after logout
    this.logoutEvent.next();
  }

  logoutEvent = new BehaviorSubject<void>(undefined);
}
