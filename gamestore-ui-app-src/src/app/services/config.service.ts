import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config: Record<string, string> = {};
  private routes: Record<string, string> = {};

  constructor(private http: HttpClient) {}

  async loadConfig(): Promise<void> {
    try {
      const configData = await firstValueFrom(
        this.http.get<Record<string, string>>('assets/configuration/configuration.json')
      );
      this.config = configData || {};
    } catch {
      this.config = {
        baseApiUrl: environment.apiBaseUrl
      };
    }

    try {
      const routeData = await firstValueFrom(
        this.http.get<Record<string, string>>('assets/configuration/route-configuration.json')
      );
      this.routes = routeData || {};
    } catch {
      this.routes = {};
    }
  }

  getBaseApiUrl(): string {
    const url = this.config['baseApiUrl'] || environment.apiBaseUrl;
    return url.endsWith('/') ? url : `${url}/`;
  }

  getApiUrl(key: string, params: Record<string, string | number> = {}): string {
    let template = this.config[key];
    if (!template) {
      return this.getBaseApiUrl();
    }

    for (const [pKey, pVal] of Object.entries(params)) {
      template = template.replace(`{${pKey}}`, encodeURIComponent(String(pVal)));
    }

    if (template.startsWith('http://') || template.startsWith('https://')) {
      return template;
    }

    return `${this.getBaseApiUrl()}${template}`;
  }

  getRoute(key: string): string {
    return this.routes[key] || `/${key.toLowerCase()}`;
  }
}
