import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-top-up-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './top-up-form.component.html',
  styleUrls: ['./top-up-form.component.scss']
})
export class TopUpFormComponent {
  form: FormGroup;
  message: string = '';
  error: string = '';

  constructor(private fb: FormBuilder, private userService: UserService) {
    this.form = this.fb.group({
      studentId: ['', Validators.required],
      amount: [0, [Validators.required, Validators.min(1)]]
    });
  }

  submit() {
  if (this.form.invalid) return;

  const { studentId, amount } = this.form.value;
  this.userService.topUpBalance(studentId, amount).subscribe({
  next: (res) => {
    this.message = `Balance updated! New Balance: Rs. ${res.balance}`;
    this.error = '';
    this.form.reset();
  },
  error: (err) => {
    this.message = '';
    this.error = err?.error || 'Student not found or server error.';
  }
});
}

}
