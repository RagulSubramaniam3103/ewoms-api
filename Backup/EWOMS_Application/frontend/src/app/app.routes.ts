import { Routes } from '@angular/router';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin/admin.component';
import { UserComponent } from './user/user.component';
import { ManagerComponent } from './manager/manager.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ChatComponent } from './chat/chat.component';

import { authGuard } from './auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'admin', component: AdminComponent, canActivate: [authGuard] },
  { path: 'user', component: UserComponent, canActivate: [authGuard] },
  { path: 'manager', component: ManagerComponent, canActivate: [authGuard] },
  { path: 'messenger', component: ChatComponent, canActivate: [authGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];
