import { Injectable } from "@angular/core";
import { CanActivate, Router, ActivatedRouteSnapshot } from "@angular/router";
import { AuthService } from "../services/auth.service";

@Injectable({ providedIn: "root" })
export class AuthGuard implements CanActivate {
  constructor(
    private auth: AuthService,
    private router: Router,
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {

    
    const storedUser = localStorage.getItem("shopez_user");
    const user = storedUser ? JSON.parse(storedUser) : null;
    const isLoggedIn = !!user?.token;

    if (!isLoggedIn) {
      this.router.navigate(["/login"]);
      return false;
    }

   
    if (route.data["role"] === "admin") {
      const isAdmin = user?.role?.toLowerCase() === "admin";
      if (!isAdmin) {
        this.router.navigate(["/"]);
        return false;
      }
    }

    return true;
  }
}
