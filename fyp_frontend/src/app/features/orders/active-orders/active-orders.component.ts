import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-active-orders',
  standalone: true,
  imports: [CommonModule, MatSnackBarModule, FormsModule],
  templateUrl: './active-orders.component.html',
  styleUrls: ['./active-orders.component.scss']
})
export class ActiveOrdersComponent implements OnInit {
  orders: any[] = [];
  preparedMap: Record<number, boolean> = {};

  constructor(private orderService: OrderService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadActiveOrders();
  }

  loadActiveOrders() {
    this.orderService.getActiveOrders().subscribe(res => {
      this.orders = res;
    });
  }

  togglePrepared(orderId: number) {
    this.preparedMap[orderId] = !this.preparedMap[orderId];
  }

  markCompleted(order: any) {
    this.orderService.markAsCompleted(order.orderId).subscribe(() => {
      this.orders = this.orders.filter(o => o.orderId !== order.orderId);
      this.snackBar.open('Order marked as completed ✅', 'Close', {
        duration: 3000,
        panelClass: 'snack-success'
      });
    });
  }
  searchOrderQuery: string = '';

  filteredOrders(): any[] {
    const query = this.searchOrderQuery.toLowerCase();
    return this.orders.filter(order =>
      order.orderNumber.toLowerCase().includes(query) ||
      `${order.firstName ?? ''} ${order.lastName ?? ''}`.toLowerCase().includes(query) ||
      order.status.toLowerCase().includes(query)
    );
  }

}
