import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { UttilsService } from '../../uttils/uttils.service';

@Component({
  selector: 'app-authenticate',
  templateUrl: './authenticate.component.html',
  styleUrl: './authenticate.component.css'
})

export class AuthenticateComponent {
  token: string;
  userId: string;

  constructor(private router: Router, private uttilService: UttilsService, private route: ActivatedRoute, private authService: AuthService) {
    this.token = '';
    this.userId = '';
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      this.userId = params['userId'];
    });


    this.authService.login(this.token, this.userId).subscribe(
      (response) => {
        this.uttilService.showSuccessToast('Login successful');
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(response.user));
        this.router.navigate(['/home/dashboard']);
        console.log('Response:', response);
      },
      (error) => {
        this.uttilService.showSuccessToast(error.error);
        this.router.navigate(['/login']);
        console.error('Error:', error);

      }
    );
  }
}
