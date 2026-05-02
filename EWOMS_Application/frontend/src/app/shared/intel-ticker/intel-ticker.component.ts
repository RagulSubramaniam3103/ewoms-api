import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-intel-ticker',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="pulse-ticker-container">
      <div class="pulse-indicator">
        <div class="dot"></div>
        <span>LIVE PULSE</span>
      </div>
      <div class="ticker-wrap">
        <div class="ticker-move">
          <div class="ticker-item" *ngFor="let msg of messages">
            <span class="t-prefix">[{{ msg.time }}]</span>
            <span class="t-content">{{ msg.text }}</span>
            <span class="t-separator">///</span>
          </div>
          <!-- Duplicate for infinite scroll -->
          <div class="ticker-item" *ngFor="let msg of messages">
            <span class="t-prefix">[{{ msg.time }}]</span>
            <span class="t-content">{{ msg.text }}</span>
            <span class="t-separator">///</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .pulse-ticker-container {
      height: 34px;
      background: #0f172a;
      border-bottom: 1px solid rgba(99, 102, 241, 0.2);
      display: flex;
      align-items: center;
      overflow: hidden;
      position: relative;
      z-index: 1000;
    }

    .pulse-indicator {
      background: #1e293b;
      height: 100%;
      padding: 0 15px;
      display: flex;
      align-items: center;
      gap: 8px;
      border-right: 1px solid rgba(99, 102, 241, 0.3);
      flex-shrink: 0;
      z-index: 2;
    }

    .pulse-indicator .dot {
      width: 6px;
      height: 6px;
      background: #22d3ee;
      border-radius: 50%;
      box-shadow: 0 0 10px #22d3ee;
      animation: pulseGlow 1.5s infinite;
    }

    .pulse-indicator span {
      color: #94a3b8;
      font-size: 0.65rem;
      font-weight: 800;
      letter-spacing: 2px;
    }

    .ticker-wrap {
      flex: 1;
      overflow: hidden;
    }

    .ticker-move {
      display: flex;
      white-space: nowrap;
      animation: tickerScroll 60s linear infinite;
    }

    .ticker-item {
      display: inline-flex;
      align-items: center;
      padding: 0 40px;
      gap: 10px;
      font-family: 'Space Mono', monospace;
      font-size: 0.75rem;
    }

    .t-prefix { color: #6366f1; font-weight: 800; }
    .t-content { color: #f8fafc; letter-spacing: 0.5px; }
    .t-separator { color: #334155; margin-left: 20px; font-weight: 200; }

    @keyframes tickerScroll {
      0% { transform: translateX(0); }
      100% { transform: translateX(-50%); }
    }

    @keyframes pulseGlow {
      0%, 100% { opacity: 1; transform: scale(1); }
      50% { opacity: 0.5; transform: scale(1.2); }
    }
  `]
})
export class IntelTickerComponent implements OnInit {
  messages = [
    { time: '09:42', text: "STRATEGIC BROADCAST: Sector 7 visual intelligence synchronized." },
    { time: '10:15', text: "PROTOCOL: New personnel identity verified in Unified Stream." },
    { time: '11:03', text: "DATA PULSE: 24 Validations recorded on Personnel Aishu's recent asset." },
    { time: '11:45', text: "SECURITY: Encryption layer reinforced on private communications." },
    { time: '12:20', text: "INTEL REPORT: User Rashmika shared a high-fidelity personnel file." },
    { time: '13:05', text: "SYSTEM: Cross-departmental bridge established for Sector 4." }
  ];

  ngOnInit() {}
}
