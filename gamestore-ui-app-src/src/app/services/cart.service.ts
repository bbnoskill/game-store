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

  addToCart(key: string): Observable<any> {
    return this.gameService.buyGame(key).pipe(
      tap(() => {
        this.cartCountSubject.next(this.cartCountSubject.value + 1);
        this.refreshCart();
      })
    );
  }

  removeFromCart(key: string): Observable<any> {
    return this.orderService.deleteFromCart(key).pipe(
      tap(() => {
        this.refreshCart();
      })
    );
  }

  refreshCart(): void {
    this.orderService.getCart().pipe(
      catchError(() => of(null))
    ).subscribe((cart) => {
      if (cart && Array.isArray(cart.details)) {
        const count = cart.details.reduce((sum: number, item: any) => sum + (item.quantity || 1), 0);
        this.cartCountSubject.next(count);
      } else if (cart && Array.isArray(cart.orderGames)) {
        const count = cart.orderGames.reduce((sum: number, item: any) => sum + (item.quantity || 1), 0);
        this.cartCountSubject.next(count);
      }
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
