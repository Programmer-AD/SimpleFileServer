import { Routes } from '@angular/router';
import { NotFoundPage } from './pages/not-found-page/not-found-page';
import { HomePage } from './pages/home-page/home-page';
import { LoginPage } from './pages/login-page/login-page';

export const routes: Routes = [
    {
        path: "",
        component: HomePage,
    },
    {
        path: "login",
        component: LoginPage,
    },
    {
        path: "**",
        component: NotFoundPage,
    }
];
