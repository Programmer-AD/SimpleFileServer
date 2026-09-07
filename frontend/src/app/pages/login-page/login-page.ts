import { Component, inject, input, signal } from "@angular/core";
import { AuthService } from "../../services/auth-service";
import { Router } from "@angular/router";
import { PasswordInputComponent } from "../../components/password-text-input-component/password-input-component";

@Component({
    imports: [PasswordInputComponent],
    selector: "app-login-page",
    styleUrl: "./login-page.scss",
    templateUrl: "./login-page.html",
})
export class LoginPage {
    private authService = inject(AuthService);
    private router = inject(Router);

    public returnUrl = input<string>("");

    protected secretValue = signal("");
    protected showAuthFailedError = signal(false);

    protected async onLoginClick() {
        this.showAuthFailedError.set(false);

        const authResult = await this.authService.tryAuthenticateAsync(this.secretValue());
        if (!authResult) {
            this.showAuthFailedError.set(true);
            return;
        }

        this.router.navigateByUrl(this.returnUrl());
    }
}
