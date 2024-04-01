import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { UttilsService } from '../../uttils/uttils.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  constructor(private router: Router, private authService: AuthService,private uttilService:UttilsService) { }
  title = 'client';
  Navigations = [
    { path: 'home/dashboard', displayName: 'Dashboard' },
    { path: 'home/students', displayName: 'Students' },
  ];

  logout() {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    this.router.navigate(['/login'])
  }
  navigateTo(path: string) {
    this.router.navigate([path])
  }
}
