import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { translations } from '../shared/translations';

export type Language = 'en' | 'uk' | 'ja';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly document = inject(DOCUMENT);
  private readonly router = inject(Router);
  private readonly selected = signal<Language>(this.restore());
  readonly language = this.selected.asReadonly();
  readonly languages = [
    { code: 'en', name: 'English' },
    { code: 'uk', name: 'Українська' },
    { code: 'ja', name: '日本語' }
  ];

  constructor() {
    this.document.documentElement.lang = this.language();
  }

  async select(value: string): Promise<void> {
    if (!this.isLanguage(value) || value === this.language()) return;
    this.selected.set(value);
    this.document.documentElement.lang = value;
    try { this.document.defaultView?.localStorage.setItem('gamestore.language', value); } catch { /* Storage is optional. */ }
    // Recreate the active page so entity requests and form values use the new language.
    await this.router.navigateByUrl(this.router.url, {
      onSameUrlNavigation: 'reload', info: { languageChange: true }
    });
  }

  translate(text: string): string {
    return translations[this.language()]?.[text] ?? text;
  }

  private restore(): Language {
    try {
      const value = this.document.defaultView?.localStorage.getItem('gamestore.language');
      return this.isLanguage(value) ? value : 'en';
    } catch { return 'en'; }
  }

  private isLanguage(value: unknown): value is Language {
    return value === 'en' || value === 'uk' || value === 'ja';
  }
}
