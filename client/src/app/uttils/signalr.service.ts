import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: HubConnection;
  private activeUser = new Subject<any>();
  private userChart = new Subject<any>();
  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.API_URL+"userHub")
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.error('Error while starting connection: ' + err));
  }

  public addTransferChartDataListener() {
    this.hubConnection.on('ReceiveMessage', (data) => {
      console.log(data);
      this.activeUser.next(data);
    });


    this.hubConnection.on('ReceiveUserDetailOnDisconnect', (data) => {
     this.userChart.next(data);

    });
    this.hubConnection.on('ReceiveUserDetailOnConnect', (data) => {
      console.log(data);
      this.userChart.next(data);
    });
  }

  
  public retrieveUserChart() {
    return this.userChart.asObservable();
    
  }
  public retrieveChartData() {
    return this.activeUser.asObservable();

  }
}
