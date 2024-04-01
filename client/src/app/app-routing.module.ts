import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthComponent } from './pages/auth/auth.component';
import { DashboardComponent } from './pages/home/dashboard/dashboard.component';
import { StudentComponent } from './pages/home/student/student.component';
import { AuthenticateComponent } from './pages/authenticate/authenticate.component';
import { IsLoggedInGuard } from './uttils/isLogin.guard';
import { HomeComponent } from './pages/home/home.component';

const routes: Routes = [
  { path: 'login', component: AuthComponent },
  { path: 'authenticate', component: AuthenticateComponent },
  {
    path: 'home', component: HomeComponent,
    // canActivateChild: [IsLoggedInGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent, canActivate: [IsLoggedInGuard] },
      { path: 'students', component: StudentComponent, canActivate: [IsLoggedInGuard] },
      { path: '',redirectTo: 'dashboard', pathMatch: 'full' },
    ]
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login', pathMatch: 'full' }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
