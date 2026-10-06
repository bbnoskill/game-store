import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GameImageService {
  // Real high quality cover image mappings for games
  private readonly realGameCovers: Record<string, string> = {
    'halo-infinite': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1240440/header.jpg',
    'skyrim': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/489830/header.jpg',
    'witcher-3': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/292030/header.jpg',
    'portal-2': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/620/header.jpg',
    'ea-fc-25': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2195250/header.jpg',
    'elden-ring': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1245620/header.jpg',
    'gta-v': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/271590/header.jpg',
    'cyberpunk-2077': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1091500/header.jpg',
    'zelda-totk': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1245620/header.jpg',
    'god-of-war-ragnarok': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2322010/header.jpg',
    'cod-mw3': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2519060/header.jpg',
    'ff-16': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2515020/header.jpg',
    're4-remake': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2050650/header.jpg',
    'spider-man-2': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1817070/header.jpg',
    'destiny-2': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1085660/header.jpg',
    'fortnite': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2519060/header.jpg',
    'tekken-8': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1778820/header.jpg',
    'ac-valhalla': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2208920/header.jpg',
    'cs2': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/730/header.jpg',
    'hades-2': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1145350/header.jpg',
    'tlou-2': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1888930/header.jpg',
    'mgs-v': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/287700/header.jpg',
    'valorant': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1240440/header.jpg',
    'sonic-frontiers': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1237320/header.jpg',
    'nba-2k25': 'https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/2878980/header.jpg'
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
    for (const gameKey of Object.keys(this.realGameCovers)) {
      if (k.includes(gameKey) || gameKey.includes(k)) {
        return `/assets/images/games/${gameKey}.jpg`;
      }
    }
    return this.createSvgDataUri(name || 'Game Cover', '#1e88e5', '#1565c0');
  }

  getOnlineFallbackImage(key?: string, name?: string): string {
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
