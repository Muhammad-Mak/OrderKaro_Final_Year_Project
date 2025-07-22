// top-up-form.component.ts
// This component allows an admin or staff user to top up a student's balance using their Student ID.
// It includes form validation, submission logic, and success/error message handling.

import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-top-up-form',                     // Selector to use in HTML: <app-top-up-form>
  standalone: true,                                // Declares this as a standalone component
  imports: [CommonModule, ReactiveFormsModule],    // Enables Angular directives and form features
  templateUrl: './top-up-form.component.html',
  styleUrls: ['./top-up-form.component.scss']
})
export class TopUpFormComponent {
  form: FormGroup;        // Reactive form for studentId and amount
  message: string = '';   // Holds success message
  error: string = '';     // Holds error message

  constructor(private fb: FormBuilder, private userService: UserService) {
    // Initialize the form with validation
    this.form = this.fb.group({
      studentId: ['', Validators.required],                  // Student ID is required
      amount: [0, [Validators.required, Validators.min(1)]]  // Amount must be at least 1
    });
  }

  // Submit form handler
  submit() {
    if (this.form.invalid) return; // If form is invalid, do nothing

    const { studentId, amount } = this.form.value;

    // Call the UserService to top up balance
    this.userService.topUpBalance(studentId, amount).subscribe({
      next: (res) => {
        // On success, show success message and reset form
        this.message = `Balance updated! New Balance: Rs. ${res.balance}`;
        this.error = '';
        this.form.reset();
      },
      error: (err) => {
        // On error, show fallback or returned error message
        this.message = '';
        this.error = err?.error || 'Student not found or server error.';
      }
    });
  }
}
