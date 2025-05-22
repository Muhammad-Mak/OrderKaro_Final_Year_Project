import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-active-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './active-orders.component.html',
  styleUrls: ['./active-orders.component.scss']
})
export class ActiveOrdersComponent implements OnInit {
  orders: any[] = [];
  preparedMap: Record<number, boolean> = {};

  constructor(private orderService: OrderService, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadActiveOrders();
  }

  loadActiveOrders() {
    this.orderService.getAllOrders().subscribe(res => {
      this.orders = res.filter(order => order.status !== 'Completed');
    });
  }

  togglePrepared(orderId: number) {
    this.preparedMap[orderId] = !this.preparedMap[orderId];
  }

  markCompleted(order: any) {
    // Mark order as completed via backend
    this.http.post('/api/payments/confirm', { paymentIntentId: order.paymentIntentId })
      .subscribe(() => {
        this.orders = this.orders.filter(o => o.orderId !== order.orderId);
      });
  }
}
