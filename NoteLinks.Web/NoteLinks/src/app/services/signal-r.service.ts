import { Injectable } from '@angular/core';  
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';   
import { Subject } from 'rxjs';
  
@Injectable({
  providedIn: 'root'
})  
export class SignalRService {  

  private updateSubject = new Subject<string>();
  onUpdate$ = this.updateSubject.asObservable();
   
  private hubConnection: HubConnection;  
  
  constructor() {  
    this.createConnection();  
    this.registerOnServerEvents();  
    this.startConnection();  
  }  
  
  change(message: string) {  
    this.hubConnection.invoke('Change', message);  
  }  
  
  private createConnection() {  
    this.hubConnection = new HubConnectionBuilder()  
      .withUrl(window.location.origin + '/calendarhub')  
      .build();  
  } 
  
  private registerOnServerEvents(): void {  
    this.hubConnection.on('Update', (data: any) => {  
      this.updateSubject.next(data);  
    });  
  }  
  
  private startConnection(): void {  
    this.hubConnection  
      .start();    
  }  
  
}  
