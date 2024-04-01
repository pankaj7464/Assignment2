import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { finalize } from 'rxjs/operators';
import { UttilsService } from '../../uttils/uttils.service';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl:string
  constructor(private http: HttpClient,private uttilService:UttilsService) {
    this.apiUrl = uttilService.API_URL()
  }

  // Get Register API service 
  register( email:string): Observable<any> {
    this.uttilService.showLoader();
    return this.http
      .post<any>(this.apiUrl + 'register/?email='+email,null)
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }

  // Get Login API service
  login(token:string,userId:string): Observable<any> {
    this.uttilService.showLoader();
    return this.http
      .get<any>(this.apiUrl + 'login?token='+token+'&userId='+userId)
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }
  getUser(): any {
    return JSON.parse(localStorage.getItem('user')||'');
  }

  logout(id:string){
    this.uttilService.showLoader();
    return this.http
      .get<any>(this.apiUrl + 'logout?userId='+id)
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }


}
