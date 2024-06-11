import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUserById(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/Users/${userId}`);
  }

  getAllUsers(): Observable<any>{
    return this.http.get(`${this.apiUrl}/Users`);
  }

  updateUserSettings(settings: any): Observable<any> {
    return this.http.patch(`${this.apiUrl}/Users/settings`, settings);
  }  
}
