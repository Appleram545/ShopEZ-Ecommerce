import { Injectable } from "@angular/core";
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { Router } from "@angular/router";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {

    // Reads token directly from localStorage every time
    let token = "";
    try {
      const stored = localStorage.getItem("shopez_user");
      if (stored) {
        const user = JSON.parse(stored);
        token = user?.token || "";
      }
    } catch {
      token = "";
    }

    // Clone request and attach Bearer token if available
    const authReq = token
      ? req.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        })
      : req;

    console.log("Interceptor - Token attached:", !!token); 

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          
          // If the  token exists but still getting 401, it may be expired
          
          const stored = localStorage.getItem("shopez_user");
          if (!stored) {
            this.router.navigate(["/login"]);
          }
        }
        return throwError(() => error);
      }),
    );
  }
}
