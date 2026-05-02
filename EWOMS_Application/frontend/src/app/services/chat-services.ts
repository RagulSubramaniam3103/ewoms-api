import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ChatServices {

  private hub!: signalR.HubConnection;

  // 🔌 CONNECT TO SIGNALR
  startConnection(token: string) {

    // Construct hub URL from environment
    const hubUrl = environment.apiUrl.replace('/api', '') + '/chatHub';

    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
      .build();

    this.hub.start()
      .then(() => console.log('✅ SignalR Connected'))
      .catch(err => console.error('❌ SignalR Error:', err));
  }

  // ✉ SEND MESSAGE
  sendMessage(receiverId: string, message: string, image?: string, video?: string, document?: string, fileName?: string) {
    if (!this.hub || this.hub.state !== signalR.HubConnectionState.Connected) {
      console.warn("SignalR not connected");
      return;
    }

    return this.hub.invoke('SendMessage', receiverId, message, image, video, document, fileName);
  }

  // 📩 RECEIVE MESSAGE
  onMessage(callback: (msg: any) => void) {
    if (!this.hub) return;
    this.hub.off('ReceiveMessage'); // Clear previous to prevent duplicates
    this.hub.on('ReceiveMessage', callback);
  }

  // 🟢 ONLINE / OFFLINE STATUS
  onUserStatus(callback: (userId: string, isOnline: boolean) => void) {
    if (!this.hub) return;
    this.hub.off('UserStatusChanged');
    this.hub.on('UserStatusChanged', callback);
  }

  // 🔵 READ RECEIPTS
  onMessagesRead(callback: (receiverId: string) => void) {
    if (!this.hub) return;
    this.hub.off('MessagesRead');
    this.hub.on('MessagesRead', callback);
  }

  markAsRead(senderId: string) {
    if (!this.hub || this.hub.state !== signalR.HubConnectionState.Connected) return;
    this.hub.invoke('MarkAsRead', senderId).catch(err => console.error(err));
  }

  // 🔌 CONNECTION STATE (optional but useful)
  onConnectionState(callback: (state: string) => void) {
    if (!this.hub) return;
    this.hub.onreconnecting(() => callback('reconnecting'));
    this.hub.onreconnected(() => callback('connected'));
    this.hub.onclose(() => callback('disconnected'));
  }

  // ❌ STOP CONNECTION (logout)
  stopConnection() {
    if (this.hub) {
      this.hub.stop();
    }
  }

  // 🔍 CHECK IF CONNECTED
  isConnected(): boolean {
    return this.hub?.state === signalR.HubConnectionState.Connected;
  }

  // ==================== GROUP CHAT ====================

  joinGroup(groupId: number) {
    if (!this.hub || this.hub.state !== signalR.HubConnectionState.Connected) return;
    return this.hub.invoke('JoinGroup', groupId.toString());
  }

  leaveGroup(groupId: number) {
    if (!this.hub || this.hub.state !== signalR.HubConnectionState.Connected) return;
    return this.hub.invoke('LeaveGroup', groupId.toString());
  }

  sendGroupMessage(groupId: number, message: string, image?: string, video?: string, document?: string, fileName?: string) {
    if (!this.hub || this.hub.state !== signalR.HubConnectionState.Connected) {
      console.warn('SignalR not connected');
      return;
    }
    return this.hub.invoke('SendGroupMessage', groupId, message, image, video, document, fileName);
  }

  onGroupMessage(callback: (msg: any) => void) {
    if (!this.hub) return;
    this.hub.off('ReceiveGroupMessage');
    this.hub.on('ReceiveGroupMessage', callback);
  }
}
