import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor() {
    console.log('TokenInterceptor');
   }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    // Get token from localStorage
    const token = localStorage.getItem('token');
    console.log(token)
    // Clone the request and attach the Bearer token if available
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json' // Separate headers with comma
        }
      });
    }

    return next.handle(request);
  }
}
