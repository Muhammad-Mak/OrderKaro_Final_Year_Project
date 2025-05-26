import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appSettings } from '../../app.settings';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) {}

  getAllUsers() {
    return this.http.get<any[]>(`${appSettings.apiBaseUrl}/users`);
  }

  deleteUser(userId: number) {
    return this.http.delete(`${appSettings.apiBaseUrl}/users/${userId}`);
  }

  topUpBalance(studentId: string, amount: number) {
  return this.http.post<{ balance: number }>(
    `${appSettings.apiBaseUrl}/users/topup?studentId=${studentId}&amount=${amount}`,
    {} 
  );
}


  updateUser(userId: number, data: any) {
    return this.http.put(`${appSettings.apiBaseUrl}/users/${userId}`, data);
  }
}
