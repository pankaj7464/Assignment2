import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class IsLoggedInGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean {
    // Check if user and token exist in localStorage
    const user = localStorage.getItem('user');
    const token = localStorage.getItem('token');

    if (user && token) {
      // User and token exist, allow access
      return true;
    } else {
      // User or token missing, redirect to login page
      this.router.navigate(['/login']);
      return false;
    }
  }
}

