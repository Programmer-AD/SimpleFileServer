import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthService } from "../services/auth-service";
import { catchError, throwError } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const authService = inject(AuthService);

    return next(request).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
                authService.navigateToLogin();
            }

            return throwError(() => error);
        })
    );
};
