// register.component.ts
// This component handles user registration using a reactive form.
// It includes field validation, form submission logic, and feedback messages.
// After successful registration, the user is redirected to the login page.

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',                            // Selector for use in templates
  standalone: true,                                    // Standalone component (no need for a module)
  imports: [CommonModule, ReactiveFormsModule],        // Imports common directives and reactive forms
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  form: FormGroup;                                     // Reactive form instance
  successMessage: string = '';                         // Feedback message on success
  errorMessage: string = '';                           // Feedback message on error

  constructor(
    private fb: FormBuilder,                           // Used to build form structure
    private auth: AuthService,                         // Handles HTTP request to register user
    private router: Router                             // Used to navigate after registration
  ) {
    // Define form controls and validators
    this.form = this.fb.group({
      firstName: ['', Validators.required],            // Required first name
      lastName: ['', Validators.required],             // Required last name
      phoneNumber: ['', Validators.required],          // Required phone number
      email: ['', Validators.required],                // Required email (basic validation)
      password: ['', Validators.required],             // Required password
      studentId: ['']                                  // Optional student ID (for RFID integration)
    });
  }

  // Handles form submission
  submit() {
    if (this.form.invalid) return;                     // Prevent submission if form is invalid

    this.auth.register(this.form.value).subscribe({
      next: () => {
        // Show success message and clear error
        this.successMessage = '✅ Registration successful!';
        this.errorMessage = '';
        this.form.reset();                             // Clear form fields

        // Redirect to login page after 1.5 seconds
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        // Show error message and clear success
        this.successMessage = '';
        this.errorMessage = '❌ Registration failed. Please try again.';
        console.error(err);
      }
    });
  }

  // Navigate to login page
  goToLogin() {
    this.router.navigate(['/login']);
  }
}
