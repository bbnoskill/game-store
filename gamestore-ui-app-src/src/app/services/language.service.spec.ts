import { Component, inject } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, RouteReuseStrategy, Router } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { LanguageService } from './language.service';
import { LanguageRouteReuseStrategy } from './language-route-reuse.strategy';
import { languageInterceptor } from '../interceptors/language.interceptor';
import { TranslatePipe } from '../shared/pipes/translate.pipe';
import { translations } from '../shared/translations';

@Component({ standalone: true, imports: [TranslatePipe], template: `<h1>{{ 'Games' | translate }}</h1><p>{{ name }}</p>` })
class LocalizedPage {
  name = '';
  constructor() { inject(HttpClient).get<{ name: string }>('/games/test').subscribe(game => this.name = game.name); }
}

describe('Localization', () => {
  beforeEach(() => {
    localStorage.removeItem('gamestore.language');
    TestBed.configureTestingModule({ providers: [
      provideRouter([{ path: 'game/test', component: LocalizedPage }]),
      { provide: RouteReuseStrategy, useClass: LanguageRouteReuseStrategy },
      provideHttpClient(withInterceptors([languageInterceptor])), provideHttpClientTesting()
    ] });
  });

  afterEach(() => {
    TestBed.inject(HttpTestingController).verify();
    localStorage.removeItem('gamestore.language');
    document.documentElement.lang = 'en';
  });

  it('uses English by default and attaches the header to reads and writes', () => {
    const http = TestBed.inject(HttpClient);
    const requests = TestBed.inject(HttpTestingController);
    http.get('/games').subscribe();
    http.put('/games', { game: {} }).subscribe();
    for (const request of requests.match('/games')) {
      expect(request.request.headers.get('Accept-Language')).toBe('en');
      request.flush({});
    }
    expect(document.documentElement.lang).toBe('en');
  });

  it('restores the selected language and falls back to English text for unknown keys', () => {
    localStorage.setItem('gamestore.language', 'ja');
    const service = TestBed.inject(LanguageService);
    expect(service.language()).toBe('ja');
    expect(service.translate('Games')).toBe('ゲーム');
    expect(service.translate('Untranslated text')).toBe('Untranslated text');
  });

  it('rejects unsupported persisted languages', () => {
    localStorage.setItem('gamestore.language', 'de');
    expect(TestBed.inject(LanguageService).language()).toBe('en');
  });

  it('works when local storage cannot be accessed', () => {
    spyOn(Storage.prototype, 'getItem').and.throwError('Blocked');
    expect(TestBed.inject(LanguageService).language()).toBe('en');
  });

  it('switches labels, refetches entity data, preserves URL, and persists preference', async () => {
    const requests = TestBed.inject(HttpTestingController);
    const harness = await RouterTestingHarness.create('/game/test');
    requests.expectOne('/games/test').flush({ name: 'English game' });
    harness.detectChanges();
    expect(harness.routeNativeElement?.textContent).toContain('Games');
    const service = TestBed.inject(LanguageService);
    await service.select('uk');
    const translated = requests.expectOne('/games/test');
    expect(translated.request.headers.get('Accept-Language')).toBe('uk');
    translated.flush({ name: 'Українська гра' });
    harness.detectChanges();
    expect(harness.routeNativeElement?.textContent).toContain('Ігри');
    expect(harness.routeNativeElement?.textContent).toContain('Українська гра');
    expect(TestBed.inject(Router).url).toBe('/game/test');
    expect(localStorage.getItem('gamestore.language')).toBe('uk');
    expect(document.documentElement.lang).toBe('uk');
    await service.select('uk');
    requests.expectNone('/games/test');
  });

  it('has matching Ukrainian and Japanese dictionary coverage', () => {
    expect(Object.keys(translations['uk']).sort()).toEqual(Object.keys(translations['ja']).sort());
    expect(Object.values(translations['uk']).every(value => value.length > 0)).toBeTrue();
    expect(Object.values(translations['ja']).every(value => value.length > 0)).toBeTrue();
  });
});
