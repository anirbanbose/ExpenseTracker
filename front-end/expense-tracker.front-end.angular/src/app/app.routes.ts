import { Routes } from '@angular/router';
import { authGuard } from './_helpers/auth.guard';

export const routes: Routes = [
    {
        path: "",
        loadComponent: () => import('./_components/general/general.component').then((m) => m.GeneralComponent),
        children: [
            {
                path: "",
                redirectTo: "index",
                pathMatch: "full"
            },
            {
                path: "index",
                loadComponent: () => import('./_components/general/landing/landing.component').then((m) => m.LandingComponent),
            },
            {
                path: "sign-up",
                loadComponent: () => import('./_components/general/sign-up/sign-up.component').then((m) => m.SignUpComponent),
            },
            {
                path: "sign-in",
                loadComponent: () => import('./_components/general/sign-in/sign-in.component').then((m) => m.SignInComponent),
            },
            {
                path: "features",
                loadComponent: () => import('./_components/general/features/features.component').then((m) => m.FeaturesComponent),
            },
            {
                path: "testimonials",
                loadComponent: () => import('./_components/general/testimonials/testimonials.component').then((m) => m.TestimonialsComponent),
            },
            {
                path: "logout",
                loadComponent: () => import('./_components/general/logout/logout.component').then((m) => m.LogoutComponent),
            },
            {
                path: "page-not-found",
                loadComponent: () => import('./_components/general/page-not-found/page-not-found.component').then((m) => m.PageNotFoundComponent),
            },
        ]
    },
    {
        path: "account",
        canActivateChild: [authGuard],
        loadComponent: () => import('./_components/account/account.component').then((m) => m.AccountComponent),
        children: [
            {
                path: "",
                redirectTo: "dashboard",
                pathMatch: "full"
            },
            {
                path: "dashboard",
                loadComponent: () => import('./_components/account/dashboard/dashboard.component').then((m) => m.DashboardComponent),
            },
            {
                path: "expenses",
                loadComponent: () => import('./_components/account/expenses/expenses.component').then((m) => m.ExpensesComponent),
                children: [
                    {
                        path: "",
                        redirectTo: "list",
                        pathMatch: "full"
                    },
                    {
                        path: "list",
                        loadComponent: () => import('./_components/account/expenses/expense-list/expense-list.component').then((m) => m.ExpenseListComponent),
                    },
                    {
                        path: "add",
                        loadComponent: () => import('./_components/account/expenses/expense-add-edit/expense-add-edit.component').then((m) => m.ExpenseAddEditComponent),
                    },
                    {
                        path: "edit/:id",
                        loadComponent: () => import('./_components/account/expenses/expense-add-edit/expense-add-edit.component').then((m) => m.ExpenseAddEditComponent),
                    },
                ]
            },
            {
                path: "reports",
                loadComponent: () => import('./_components/account/reports/reports.component').then((m) => m.ReportsComponent),
            },
            {
                path: "profile",
                loadComponent: () => import('./_components/account/profile/profile.component').then((m) => m.ProfileComponent),
            },
            {
                path: "settings",
                loadComponent: () => import('./_components/account/settings/settings.component').then((m) => m.SettingsComponent),
            },
        ]
    },
    {
        path: '**',
        redirectTo: "page-not-found"
    }
];
