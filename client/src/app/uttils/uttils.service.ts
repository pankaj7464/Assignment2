import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable, Subject } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
@Injectable({
  providedIn: 'root'
})
export class UttilsService {
  private apiUrl = `${environment.API_URL}api/app/`;
  private loadingSubject: Subject<boolean> = new Subject<boolean>();

  constructor( private snackBar: MatSnackBar) {
  }

  API_URL(){
    return this.apiUrl;
  }
  showSuccessToast(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['success-snackbar'],
    });
  }
  showLoader(): void {
    this.loadingSubject.next(true);
  }

  hideLoader(): void {
    this.loadingSubject.next(false);
  }
  isLoading(): Observable<boolean> {
    return this.loadingSubject.asObservable();
  }

 

}
