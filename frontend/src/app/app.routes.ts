import { Routes } from '@angular/router';
import { NotFoundPage } from './pages/not-found-page/not-found-page';
import { HomePage } from './pages/home-page/home-page';

export const routes: Routes = [
    {
        path: "",
        component: HomePage,
    },
    {
        path: "**",
        component: NotFoundPage,
    }
];
