import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/analytics.service';
import { UserService } from '../../../core/services/user.service';
import { OrderService } from '../../../core/services/order.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

  editUser: any = null;

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
