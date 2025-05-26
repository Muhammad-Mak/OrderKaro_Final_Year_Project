import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  form: FormGroup;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  submit() {
    if (this.form.invalid) return;

    const credentials = this.form.value as { email: string; password: string };

    this.auth.login(credentials).subscribe({
      next: (res) => {
        if (res.user.role === 'Customer') {
          this.successMessage = '';
          this.errorMessage = 'Only Admins and Staff can log in.';
          return;
        }

        this.auth.saveAuthData(res.token, res.user);
        this.successMessage = '✅ Login Successful!';
        this.errorMessage = '';

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
        this.successMessage = '';
        this.errorMessage = '❌ Invalid email or password.';
      }
    });
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}
