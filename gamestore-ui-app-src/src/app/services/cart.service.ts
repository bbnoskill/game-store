import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, catchError, of } from 'rxjs';
import { GameService } from './game.service';
import { OrderService } from './order.service';
import { CartItem, CartSummary } from '../models/cart.model';
import { environment } from '../../environments/environment';
import { GameImageService } from './game-image.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  private localCartItems: CartItem[] = [];

  constructor(
    private gameService: GameService,
    private orderService: OrderService,
    private gameImageService: GameImageService
  ) {
    this.refreshCart();
  }

  getCartCount(): number {
    return this.cartCountSubject.value;
  }

  setCartCount(count: number): void {
    this.cartCountSubject.next(count);
  }

  getLocalCartItems(): CartItem[] {
    return [...this.localCartItems];
  }

  addToCart(key: string): Observable<any> {
    return this.gameService.getGameByKey(key).pipe(
      tap((game) => {
        if (game) {
          const existing = this.localCartItems.find(i => i.gameKey === game.key || i.productId === game.id);
          if (existing) {
            existing.quantity++;
          } else {
            this.localCartItems.push({
              id: game.id || game.key,
              productId: game.id || game.key,
              gameKey: game.key,
              gameName: game.name,
              price: game.price,
              quantity: 1,
              discount: game.discount || 0,
              platform: 'PC / Console',
              fileSize: '15 GB'
            });
          }
          this.updateCount();
        }
      }),
      tap(() => {
        this.gameService.buyGame(key).pipe(catchError(() => of(null))).subscribe();
      })
    );
  }

  removeFromCart(key: string): Observable<any> {
    this.localCartItems = this.localCartItems.filter(i => i.gameKey !== key && i.id !== key && i.productId !== key);
    this.updateCount();
    return this.orderService.deleteFromCart(key).pipe(
      catchError(() => of(null))
    );
  }

  updateLocalQuantity(id: string, count: number): void {
    const item = this.localCartItems.find(i => i.id === id || i.gameKey === id || i.productId === id);
    if (item) {
      item.quantity = count;
      if (item.quantity <= 0) {
        this.localCartItems = this.localCartItems.filter(i => i !== item);
      }
      this.updateCount();
    }
  }

  clearCart(): void {
    this.localCartItems = [];
    this.updateCount();
  }

  private updateCount(): void {
    const totalCount = this.localCartItems.reduce((sum, item) => sum + item.quantity, 0);
    this.cartCountSubject.next(totalCount);
  }

  refreshCart(): void {
    this.orderService.getCart().pipe(
      catchError(() => of(null))
    ).subscribe((cart) => {
      if (cart && (Array.isArray(cart) || Array.isArray(cart.details) || Array.isArray(cart.orderGames))) {
        const rawDetails = Array.isArray(cart) ? cart : (cart.details || cart.orderGames || []);
        if (rawDetails.length > 0) {
          this.localCartItems = rawDetails.map((detail: any) => ({
            id: detail.id || detail.productId || detail.gameKey,
            productId: detail.productId || detail.gameKey,
            gameKey: detail.gameKey || detail.productId,
            gameName: detail.gameName || detail.productName || detail.name || 'Game',
            price: detail.price || 0,
            quantity: detail.quantity || 1,
            discount: detail.discount !== undefined ? detail.discount : (detail.discountPercentage || 0),
            platform: detail.platform || 'PC',
            fileSize: detail.fileSize || '15 GB'
          }));
          this.updateCount();
          return;
        }
      }
      this.updateCount();
    });
  }

  calculateSummary(items: CartItem[]): CartSummary {
    const subtotal = items.reduce((sum, item) => {
      const unitPrice = item.discount > 0
        ? item.price * (1 - item.discount / 100)
        : item.price;
      return sum + unitPrice * item.quantity;
    }, 0);

    const taxes = subtotal * environment.taxRate;
    const total = subtotal + taxes;
    const itemCount = items.reduce((sum, item) => sum + item.quantity, 0);

    return {
      items,
      subtotal,
      taxes,
      total,
      itemCount
    };
  }
}
