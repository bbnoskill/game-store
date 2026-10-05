import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GameImageService {
  private readonly defaultImages: string[] = [
    'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=600&q=80', // esports/pc
    'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80', // retro tech
    'https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=600&q=80', // arcade neon
    'https://images.unsplash.com/photo-1511512578047-dfb367046420?auto=format&fit=crop&w=600&q=80', // gaming setup
    'https://images.unsplash.com/photo-1579373903781-fd5c0c30c4cd?auto=format&fit=crop&w=600&q=80', // abstract gaming
    'https://images.unsplash.com/photo-1612287233207-681966a3375b?auto=format&fit=crop&w=600&q=80', // vr headset
    'https://images.unsplash.com/photo-1563089145-599997674d42?auto=format&fit=crop&w=600&q=80', // cyberpunk neon
    'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80'  // vibrant 3d
  ];

  private readonly namedGameImages: Record<string, string> = {
    'ace-combat-7': 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=600&q=80',
    'resident-evil-2': 'https://images.unsplash.com/photo-1509198397868-475647b2a1e5?auto=format&fit=crop&w=600&q=80',
    'kingdom-hearts-iii': 'https://images.unsplash.com/photo-1563089145-599997674d42?auto=format&fit=crop&w=600&q=80',
    'subnautica': 'https://images.unsplash.com/photo-1682687220063-4742bd7fd538?auto=format&fit=crop&w=600&q=80',
    'beats-fever': 'https://images.unsplash.com/photo-1514525253161-7a46d19cd819?auto=format&fit=crop&w=600&q=80',
    'eden-tomorrow': 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80',
    'for-honor': 'https://images.unsplash.com/photo-1579373903781-fd5c0c30c4cd?auto=format&fit=crop&w=600&q=80',
    'warhammer': 'https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=600&q=80'
  };

  getGameImage(key?: string, name?: string): string {
    if (key) {
      const lowerKey = key.toLowerCase();
      for (const [k, url] of Object.entries(this.namedGameImages)) {
        if (lowerKey.includes(k)) {
          return url;
        }
      }
    }

    if (name) {
      const lowerName = name.toLowerCase().replace(/[^a-z0-9]/g, '');
      for (const [k, url] of Object.entries(this.namedGameImages)) {
        if (lowerName.includes(k.replace(/-/g, ''))) {
          return url;
        }
      }
    }

    // Hash the key/name to pick a consistent default image
    const str = key || name || 'default';
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      hash = (hash << 5) - hash + str.charCodeAt(i);
      hash |= 0;
    }
    const index = Math.abs(hash) % this.defaultImages.length;
    return this.defaultImages[index];
  }

  getHeroBannerImage(): string {
    return 'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=1920&q=85';
  }

  getFeaturedCardImage(): string {
    return 'https://images.unsplash.com/photo-1612287233207-681966a3375b?auto=format&fit=crop&w=600&q=80';
  }
}
