import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChatSocketService {
  private hubConnection: signalR.HubConnection | undefined;
  
  // Status observable
  private connectionStatus = new BehaviorSubject<boolean>(false);
  public connectionStatus$ = this.connectionStatus.asObservable();

  // Messages observable
  private incomingMessages = new BehaviorSubject<any>(null);
  public incomingMessages$ = this.incomingMessages.asObservable();

  // History observable
  private chatHistory = new BehaviorSubject<any[]>([]);
  public chatHistory$ = this.chatHistory.asObservable();

  constructor() {
    this.startConnection();
  }

  private startConnection() {
    const token = localStorage.getItem('jwtToken');
    
    // Adjust URL if needed (e.g. environment.apiUrl replacing 'api' with 'chathub')
    const hubUrl = environment.apiUrl.replace('/api', '') + '/chathub';

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection established');
        this.connectionStatus.next(true);
        this.registerServerEvents();
      })
      .catch(err => {
        console.error('Error while starting SignalR connection:', err);
        this.connectionStatus.next(false);
        // Retry logic could go here
      });
  }

  private registerServerEvents() {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveMessage', (msg: any) => {
      this.incomingMessages.next(msg);
    });

    this.hubConnection.on('MessagesRead', (receiverId: string) => {
      this.incomingMessages.next({ type: 'read-receipt', userId: receiverId });
    });

    this.hubConnection.on('UserStatusChanged', (userId: string, isOnline: boolean) => {
      this.incomingMessages.next({ type: 'status-change', userId, isOnline });
    });
  }

  // Send message through the socket
  sendMessage(to: string, text: string, image?: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessage', to, text, image)
        .catch(err => console.error(err));
    }
  }

  // Mark messages as read
  markAsRead(senderId: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('MarkAsRead', senderId)
        .catch(err => console.error(err));
    }
  }

  // Request history between two users
  getHistory(me: string, other: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('GetChatHistory', other)
        .catch(err => console.error(err));
    }
  }

  // Compatibility methods for old component
  onReceiveHistory(): Observable<any[]> {
    return this.chatHistory$;
  }

  onReceiveMessage(): Observable<any> {
    return this.incomingMessages$;
  }

  onStatusChange(): Observable<boolean> {
    return this.connectionStatus$;
  }
}
