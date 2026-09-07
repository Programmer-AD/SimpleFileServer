import { inject, Service } from "@angular/core";
import { Router } from "@angular/router";

@Service()
export class AuthService {
    private router = inject(Router);

    public navigateToLogin(): void {
        if (this.router.url.startsWith("/login")) {
            return;
        }

        this.router.navigate(["/login"], {
            queryParams: {
                returnUrl: this.router.url,
            }
        })
    }

    public async tryAuthenticateAsync(secret: string): Promise<boolean> {
        // Cannot use injected HttpClient here since it would cause unexpected behavior due to auth intercepter which call this class.
        const response = await fetch("/api/auth/cookies", {
            method: "GET",
            headers: {
                "Authorization": `PresharedSecret ${secret}`
            }
        });

        return response.ok;
    }

    public async logoutAsync() {
        await fetch("/api/auth/cookies", {
            method: "DELETE",
        });

        this.navigateToLogin();
    }
}
