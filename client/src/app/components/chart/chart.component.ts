import { Component, Input, NgZone, OnInit } from '@angular/core';
import { SignalrService } from '../../uttils/signalr.service';
import { UttilsService } from '../../uttils/uttils.service';


@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponents implements OnInit {


  chartData:any;
  userChart:any;
  user:any;
  constructor(private signalRService:SignalrService,
    private zone: NgZone,
    private uttilSerive:UttilsService) {
    this.signalRService.addTransferChartDataListener();
    this.signalRService.retrieveChartData().subscribe((data) => {
      console.log(data)
      this.chartData = data;
    });
  }

  public  chartOptions: any = {
    series: [], 
    chart: {},
    labels: [], 
    responsive: []
  };
  ngOnInit() {
    this.initializeChartOptions();
    this.signalRService.addTransferChartDataListener();
    this.signalRService.retrieveChartData().subscribe((data) => {
      this.zone.run(() => {
        this.chartData = data;
        this.updateChartOptions();
      });
    });
   
  }
  private initializeChartOptions() {
    this.chartOptions = {
      series: [this.chartData.activeUser, this.chartData.totalUser-this.chartData.activeUser],
      chart: {
        height: 350,
        type: "pie"
      },
      labels: ["Online User", "Offline User"],
      responsive: [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: 200
            },
            legend: {
              position: 'bottom'
            }
          }
        }
      ]
    };
  }
  private updateChartOptions() {
    this.chartOptions.series = [this.chartData.activeUser, this.chartData.totalUser-this.chartData.activeUser];
  }
}