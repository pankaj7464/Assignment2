import { Component } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { SignalrService } from '../../../uttils/signalr.service';
import { UttilsService } from '../../../uttils/uttils.service';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  chartData:any;
  userChart:any;
  user:any;
  constructor(private authService: AuthService,private signalRService:SignalrService,private uttilSerive:UttilsService) {
    this.user = authService.getUser();
  }


  ngOnInit() {
    this.signalRService.addTransferChartDataListener();
    this.signalRService.retrieveChartData().subscribe((data) => {
      console.log(data)
      this.chartData = data;
    });

    this.signalRService.retrieveUserChart().subscribe((data) => {
      console.log(data)
      this.userChart = data;
      this.uttilSerive.showSuccessToast(`${ this.userChart.currentUser.userName} is online`)

    });
  }

}
