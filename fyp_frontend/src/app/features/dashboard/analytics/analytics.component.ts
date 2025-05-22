import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnalyticsService } from '../../../core/services/analytics.service';
import { UserService } from '../../../core/services/user.service';
import { OrderService } from '../../../core/services/order.service';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.scss']
})
export class AnalyticsComponent implements OnInit {
  totalOrders: number = 0;
  totalRevenue: number = 0;
  topItems: any[] = [];

  users: any[] = [];
  selectedUser: any = null;
  userOrders: any[] = [];

  allOrders: any[] = [];

  constructor(
    private analyticsService: AnalyticsService,
    private userService: UserService,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.loadAnalytics();
    this.loadUsers();
    this.loadAllOrders();
  }

  loadAnalytics() {
    this.analyticsService.getTotals().subscribe(res => {
      this.totalOrders = res.totalOrders;
      this.totalRevenue = res.totalRevenue;
    });

    this.analyticsService.getTopItems().subscribe(res => {
      this.topItems = res;
    });
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe(res => {
      this.users = res;
    });
  }

  loadAllOrders() {
    this.orderService.getAllOrders().subscribe(res => {
      this.allOrders = res;
    });
  }

  selectUser(user: any) {
    this.selectedUser = user;
    this.orderService.getOrdersByUser(user.userId).subscribe(res => {
      this.userOrders = res;
    });
  }

  deleteUser(userId: number) {
    this.userService.deleteUser(userId).subscribe(() => {
      this.users = this.users.filter(u => u.userId !== userId);
    });
  }
}
