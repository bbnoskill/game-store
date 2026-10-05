import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';
import { Order, OrderItem, PaymentMethod, PaymentRequest, OrderHistoryItem } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) {}

  getAllOrders(): Observable<Order[]> {
    const url = this.configService.getApiUrl('ordersApiUrl');
    return this.http.get<Order[]>(url);
  }

  getOrderById(id: string): Observable<Order> {
    const url = this.configService.getApiUrl('orderApiUrl', { id });
    return this.http.get<Order>(url);
  }

  getOrderDetails(id: string): Observable<OrderItem[]> {
    const url = this.configService.getApiUrl('orderDetailsApiUrl', { id });
    return this.http.get<OrderItem[]>(url);
  }

  getCart(): Observable<any> {
    const url = this.configService.getApiUrl('basketApiUrl');
    return this.http.get<any>(url);
  }

  deleteFromCart(key: string): Observable<any> {
    const url = this.configService.getApiUrl('cancelGameBuyApiUrl', { key });
    return this.http.delete(url);
  }

  getPaymentMethods(): Observable<{ paymentMethods: PaymentMethod[] }> {
    const url = this.configService.getApiUrl('makeOrderInfoApiUrl');
    return this.http.get<{ paymentMethods: PaymentMethod[] }>(url);
  }

  pay(request: PaymentRequest): Observable<any> {
    const url = this.configService.getApiUrl('payApiUrl');
    return this.http.post(url, request, { responseType: 'blob' as 'json' });
  }

  getHistory(start?: string, end?: string): Observable<OrderHistoryItem[]> {
    const url = this.configService.getApiUrl('historyApiUrl');
    let params = new HttpParams();
    if (start) params = params.set('start', start);
    if (end) params = params.set('end', end);
    return this.http.get<OrderHistoryItem[]>(url, { params });
  }

  shipOrder(id: string): Observable<any> {
    const url = this.configService.getApiUrl('shipOrderApiUrl', { id });
    return this.http.post(url, {});
  }

  updateOrderDetailQuantity(id: string, count: number): Observable<any> {
    const url = this.configService.getApiUrl('updateOrderDetailCountApiUrl', { id });
    return this.http.patch(url, count);
  }

  deleteOrderDetail(id: string): Observable<any> {
    const url = this.configService.getApiUrl('deleteOrderDetailApiUrl', { id });
    return this.http.delete(url);
  }

  addGameToOrder(id: string, key: string): Observable<any> {
    const url = this.configService.getApiUrl('addOrderDetailApiUrl', { id, key });
    return this.http.post(url, {});
  }
}
