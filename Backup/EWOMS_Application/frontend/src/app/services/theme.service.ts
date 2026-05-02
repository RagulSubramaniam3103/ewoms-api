import { Injectable, signal, effect } from '@angular/core';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private themeSignal = signal<Theme>(this.getInitialTheme());

  constructor() {
    // Sync theme to document body whenever it changes
    effect(() => {
      const currentTheme = this.themeSignal();
      this.applyTheme(currentTheme);
      localStorage.setItem('theme', currentTheme);
    });
  }

  get theme() {
    return this.themeSignal.asReadonly();
  }

  toggleTheme() {
    this.themeSignal.update(t => t === 'light' ? 'dark' : 'light');
  }

  setTheme(theme: Theme) {
    this.themeSignal.set(theme);
  }

  private getInitialTheme(): Theme {
    const saved = localStorage.getItem('theme') as Theme;
    if (saved) return saved;
    
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  private applyTheme(theme: Theme) {
    const body = document.body;
    if (theme === 'dark') {
      body.classList.add('dark-theme');
      body.classList.remove('light-theme');
    } else {
      body.classList.add('light-theme');
      body.classList.remove('dark-theme');
    }
  }
}
