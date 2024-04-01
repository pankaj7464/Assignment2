import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { UttilsService } from '../../uttils/uttils.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  email: string;
  logMessage: string = '';
  url = '';
  constructor(private authService: AuthService,private uttilService:UttilsService) {
    this.email = '';
  }

  register(): void {
    // Implement login logic here
    console.log('Login button clicked');
    this.authService.register(this.email).subscribe(
      (response) => {
        console.log('Response:', response);
        this.url = response.link;
        this.uttilService.showSuccessToast(response.message);
        this.logMessage = 'Login successful';
      },
      (error) => {
        console.error('Error:', error);
        this.logMessage = 'Login failed';
      }
    );
    console.log('Email:', this.email);
    // Redirect or perform authentication logic
  }


  logout(): void {
    
  }


}
