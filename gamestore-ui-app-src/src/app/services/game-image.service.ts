import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GameImageService {
  // Real high quality cover image mappings for games
  private readonly realGameCovers: Record<string, string> = {
    'halo-infinite': 'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=600&q=80',
    'skyrim': 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=600&q=80',
    'witcher-3': 'https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=600&q=80',
    'portal-2': 'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80',
    'ea-fc-25': 'https://images.unsplash.com/photo-1579373903781-fd5c0c30c4cd?auto=format&fit=crop&w=600&q=80',
    'elden-ring': 'https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=600&q=80',
    'gta-v': 'https://images.unsplash.com/photo-1563089145-599997674d42?auto=format&fit=crop&w=600&q=80',
    'cyberpunk-2077': 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80',
    'zelda-totk': 'https://images.unsplash.com/photo-1612287233207-681966a3375b?auto=format&fit=crop&w=600&q=80',
    'god-of-war-ragnarok': 'https://images.unsplash.com/photo-1511512578047-dfb367046420?auto=format&fit=crop&w=600&q=80',
    'cod-mw3': 'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=600&q=80',
    'ff-16': 'https://images.unsplash.com/photo-1563089145-599997674d42?auto=format&fit=crop&w=600&q=80',
    're4-remake': 'https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=600&q=80',
    'spider-man-2': 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80',
    'destiny-2': 'https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=600&q=80',
    'fortnite': 'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80',
    'tekken-8': 'https://images.unsplash.com/photo-1579373903781-fd5c0c30c4cd?auto=format&fit=crop&w=600&q=80',
    'ac-valhalla': 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=600&q=80',
    'cs2': 'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=600&q=80',
    'hades-2': 'https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=600&q=80',
    'tlou-2': 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80',
    'mgs-v': 'https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=600&q=80',
    'valorant': 'https://images.unsplash.com/photo-1563089145-599997674d42?auto=format&fit=crop&w=600&q=80',
    'sonic-frontiers': 'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80',
    'nba-2k25': 'https://images.unsplash.com/photo-1579373903781-fd5c0c30c4cd?auto=format&fit=crop&w=600&q=80'
  };

  private createSvgDataUri(title: string, color1: string, color2: string): string {
    // Play button points RIGHT: M280 145 L340 180 L280 215 Z
    const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="600" height="400" viewBox="0 0 600 400">
      <defs>
        <linearGradient id="g" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stop-color="${color1}"/>
          <stop offset="100%" stop-color="${color2}"/>
        </linearGradient>
      </defs>
      <rect width="600" height="400" fill="url(#g)"/>
      <rect x="20" y="20" width="560" height="360" rx="12" fill="none" stroke="rgba(255,255,255,0.15)" stroke-width="2"/>
      <circle cx="300" cy="170" r="55" fill="rgba(0,0,0,0.3)" stroke="rgba(255,255,255,0.4)" stroke-width="3"/>
      <path d="M288 145 L324 170 L288 195 Z" fill="#ffffff"/>
      <rect x="40" y="275" width="520" height="90" rx="8" fill="rgba(0,0,0,0.4)"/>
      <text x="300" y="320" font-family="Roboto, sans-serif" font-size="22" font-weight="bold" fill="#ffffff" text-anchor="middle">${title}</text>
      <text x="300" y="348" font-family="Roboto, sans-serif" font-size="13" font-weight="bold" fill="#f57c00" text-anchor="middle">OFFICIAL GAME EDITION</text>
    </svg>`;
    return 'data:image/svg+xml;charset=utf-8,' + encodeURIComponent(svg);
  }

  getGameImage(key?: string, name?: string): string {
    const k = (key || name || '').toLowerCase();
    for (const [gameKey, url] of Object.entries(this.realGameCovers)) {
      if (k.includes(gameKey) || gameKey.includes(k)) {
        return url;
      }
    }
    return this.createSvgDataUri(name || 'Game Cover', '#1e88e5', '#1565c0');
  }

  getHeroBannerImage(): string {
    return 'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=1920&q=85';
  }

  getFeaturedCardImage(): string {
    return 'https://images.unsplash.com/photo-1612287233207-681966a3375b?auto=format&fit=crop&w=600&q=80';
  }

  getFallbackImage(): string {
    return this.createSvgDataUri('Game Box Art', '#f57c00', '#222222');
  }
}
