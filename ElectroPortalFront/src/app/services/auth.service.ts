import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, firstValueFrom, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private apiUrl = environment.apiUrl;
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/Users/login`, { username, password });
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Users`, data);
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  getCurrentUserId(): string | null {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || null;
      } catch (error) {
        console.error('Failed to decode token', error);
        return null;
      }
    }
    return null;
  }
  private hasToken(): boolean {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      const decoded: any = jwtDecode(token);
      return true;
    }
    return false;
  }

  checkIfUserIsAdmin(): Observable<boolean> {
    const url = environment.apiUrl;
    return this.http.get<any>(`${url}/Users/IsAdmin`).pipe(
      map(response => response.isAdmin)
    );
  }

  public logout(): void {
    localStorage.removeItem('jwtToken');
    this.setLogout();
  }

  public setLogin() {
    this.loggedIn.next(true);
  }

  public setLogout() {
    this.loggedIn.next(false);
  }
}



