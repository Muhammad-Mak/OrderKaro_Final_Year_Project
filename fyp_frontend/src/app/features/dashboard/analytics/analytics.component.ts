import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/analytics.service';
import { UserService } from '../../../core/services/user.service';
import { OrderService } from '../../../core/services/order.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.scss']
})
export class AnalyticsComponent implements OnInit {
  totalUsers: number = 0;
  totalOrders: number = 0;
  totalRevenue: number = 0;
  topItems: any[] = [];

  users: any[] = [];
  selectedUser: any = null;
  userOrders: any[] = [];
  allOrders: any[] = [];
  editUser: any = null;

  // Chart Data
  ordersWeekData: ChartData<'line'> = { labels: [], datasets: [] };
  topItemsData: ChartData<'bar'> = { labels: [], datasets: [] };
  orderTypeData: ChartData<'pie'> = { labels: ['Pickup', 'Delivery'], datasets: [] };

  // Chart Options
  lineChartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false
  };

  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false
  };

  pieChartOptions: ChartConfiguration<'pie'>['options'] = {
    responsive: true,
    maintainAspectRatio: false
  };

  constructor(
    private analyticsService: AnalyticsService,
    private userService: UserService,
    private orderService: OrderService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const role = this.authService.getUserRole();
    if (role !== 'Admin') {
      this.router.navigate(['/dashboard/orders']);
      return;
    }

    this.loadAnalytics();
    this.loadUsers();
    this.loadAllOrders();
  }

  loadAnalytics() {
    this.analyticsService.getTotals().subscribe(res => {
      this.totalOrders = res.totalOrders;
      this.totalRevenue = res.totalRevenue;
    });

    this.analyticsService.getUserCount().subscribe(res => {
      this.totalUsers = res.totalUsers;
    });

    this.analyticsService.getTopItems().subscribe(res => {
      this.topItems = res;

      this.topItemsData = {
        labels: res.map((item: any) => item.name),
        datasets: [{
          data: res.map((item: any) => item.orderCount),
          label: 'Orders',
          backgroundColor: '#1e88e5'
        }]
      };
    });

    this.analyticsService.getOrdersPerWeek().subscribe(res => {
      this.ordersWeekData = {
        labels: res.map((r: any) => r.date),
        datasets: [{
          data: res.map((r: any) => r.count),
          label: 'Orders',
          fill: true,
          tension: 0.4,
          borderColor: '#1e88e5',
          backgroundColor: 'rgba(30, 136, 229, 0.2)'
        }]
      };
    });

    this.analyticsService.getOrderTypeRatio().subscribe(res => {
      this.orderTypeData = {
        labels: ['Pickup', 'Delivery'],
        datasets: [{
          data: [res.pickup, res.delivery],
          backgroundColor: ['#42a5f5', '#66bb6a']
        }]
      };
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

  edit(user: any) {
    this.editUser = { ...user };
  }

  cancelEdit() {
    this.editUser = null;
  }

  saveEdit() {
    this.userService.updateUser(this.editUser.userId, this.editUser).subscribe(() => {
      const index = this.users.findIndex(u => u.userId === this.editUser.userId);
      if (index !== -1) {
        this.users[index] = { ...this.editUser };
      }
      this.editUser = null;
    });
  }

  confirmDelete(user: any) {
    const confirmed = confirm(`Are you sure you want to delete ${user.firstName} ${user.lastName}?`);
    if (confirmed) {
      this.deleteUser(user.userId);
    }
  }

  deleteUser(userId: number) {
    this.userService.deleteUser(userId).subscribe(() => {
      this.users = this.users.filter(u => u.userId !== userId);
      if (this.selectedUser?.userId === userId) {
        this.selectedUser = null;
      }
    });
  }
}
