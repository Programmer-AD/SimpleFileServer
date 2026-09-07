import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth-service';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.html',
    styleUrl: './app.scss'
})
export class App {
    private authService = inject(AuthService);

    protected readonly currentYear = new Date().getFullYear();

    protected async onLogoutClick() {
        await this.authService.logoutAsync();
    }
}
