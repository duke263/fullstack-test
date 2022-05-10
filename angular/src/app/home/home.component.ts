import { Component, Injector, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {MessageService} from 'primeng/api';
import { Calendar } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';

// declare var google: any;
@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./home.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase  {
  data: any;
  //maps

  options: any;
  // overlays: any[];
  // dialogVisible: boolean;
  // markerTitle: string;
  // selectedPosition: any;
  // infoWindow: any;
  // draggable: boolean;

  constructor(
    injector: Injector,
    private messageService: MessageService) {
    super(injector);

    const name = Calendar.name;
    //chart
    this.data = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
            label: 'First Dataset',
            data: [65, 59, 80, 81, 56, 55, 40],
            fill: false,
            borderColor: '#4bc0c0'
        },
        {
            label: 'Second Dataset',
            data: [28, 48, 40, 19, 86, 27, 90],
            fill: false,
            borderColor: '#565656'
        }
      ]
    }
  }
  selectData(event) {
    this.messageService.add({severity: 'info', summary: 'Data Selected', 'detail': this.data.datasets[event.element._datasetIndex].data[event.element._index]});
  }

  ngOnInit() {
    // this.eventService.getEvents().then(events => {this.events = events;});

    this.options = {
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      defaultDate: '2021-02-01',
      header: {
          left: 'prev,next',
          center: 'title',
          right: 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      editable: true,
      selectable:true,
      selectMirror: true,
      dayMaxEvents: true
    };
  }



}
