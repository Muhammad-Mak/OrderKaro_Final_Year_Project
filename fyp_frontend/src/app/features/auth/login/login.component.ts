// login.component.ts
// This component provides the login functionality for Admin and Staff users only.
// It includes a reactive login form, form validation, success/error feedback,
// and routing to different dashboards based on user role.

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',                           // Component selector used in templates
  standalone: true,                                // Declares as a standalone Angular component
  imports: [CommonModule, ReactiveFormsModule],    // Imports for Angular features
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  form: FormGroup;                                  // Reactive form for email and password
  successMessage: string = '';                      // Holds login success feedback
  errorMessage: string = '';                        // Holds login error feedback

  constructor(
    private fb: FormBuilder,                        // Used to build the form
    private auth: AuthService,                      // Handles login logic via API
    private router: Router                          // Handles navigation on success
  ) {
    // Initialize login form with validators
    this.form = this.fb.group({
      email: ['', Validators.required],             // Email is required
      password: ['', Validators.required]           // Password is required
    });
  }

  // Called when the login form is submitted
  submit() {
    if (this.form.invalid) return;                  // Exit if the form is invalid

    const credentials = this.form.value as { email: string; password: string };

    this.auth.login(credentials).subscribe({
      next: (res) => {
        // Prevent Customers from logging in (UI is only for Admin/Staff)
        if (res.user.role === 'Customer') {
          this.successMessage = '';
          this.errorMessage = 'Only Admins and Staff can log in.';
          return;
        }

        // Save token and user info in local storage/session
        this.auth.saveAuthData(res.token, res.user);

        this.successMessage = '✅ Login Successful!';
        this.errorMessage = '';

        // Navigate to role-specific dashboard after a brief delay
        setTimeout(() => {
          const role = res.user.role;
          if (role === 'Admin') {
            this.router.navigate(['/dashboard/analytics']);
          } else {
            this.router.navigate(['/orders/active']);
          }
        }, 1000);
      },
      error: () => {
        // On failed login attempt
        this.successMessage = '';
        this.errorMessage = '❌ Invalid email or password.';
      }
    });
  }

  // Navigate to the registration page
  goToRegister() {
    this.router.navigate(['/register']);
  }
}
