import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { finalize } from 'rxjs/operators';
import { UttilsService } from '../../../uttils/uttils.service';
@Injectable({
  providedIn: 'root'
})
export class StudentService {
  apiUrl:string
  constructor(private http: HttpClient,private uttilService:UttilsService) {
    this.apiUrl = uttilService.API_URL()
  }

  // Get Role API service 
  getStudent( ): Observable<any> {
    this.uttilService.showLoader();
    return this.http
      .get<any>(this.apiUrl + 'student/')
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }

  // Get Role API service 
  postStudent( data: any): Observable<any> {
    this.uttilService.showLoader();
    return this.http
      .post<any>(this.apiUrl + 'student/',data)
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }

   // Get Role API service 
   putStudent(id:string, data: any): Observable<any> {
    this.uttilService.showLoader();
    return this.http
      .put<any>(this.apiUrl + 'student/'+id,data)
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }

 
   // Get Role API service 
   deleteStudent( id: any): Observable<any> {
    this.uttilService.showLoader();
    return this.http
      .delete<any>(this.apiUrl + 'student/'+id)
      .pipe(finalize(() => {
        this.uttilService.hideLoader();
      }));
  }



}
