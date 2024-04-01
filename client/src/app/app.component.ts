import { Component } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  constructor(private router: Router) {
    let token = localStorage.getItem('token')
    if (token) {
      this.router.navigate(['home/dashboard'])
    }
  }
  title = 'client';


  navigateTo(arg0: string) {
    this.router.navigate([arg0])
  }
}
