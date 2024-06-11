import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Documentation } from '../models/documentation.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DocumentationService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDocumentations(): Observable<Documentation[]> {
    return this.http.get<Documentation[]>(`${this.apiUrl}/Documentation`);
  }

  getDocumentation(id: number): Observable<Documentation> {
    return this.http.get<Documentation>(`${this.apiUrl}/Documentation/${id}`);
  }

  createDocumentation(documentation: Documentation): Observable<Documentation> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });
    return this.http.post<Documentation>(`${this.apiUrl}/Documentation`, documentation, { headers });
  }

  updateDocumentation(id: number, documentation: Documentation): Observable<void> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });
    return this.http.put<void>(`${this.apiUrl}/Documentation/${id}`, documentation, { headers });
  }

  deleteDocumentation(id: number): Observable<void> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.getToken()}`
    });
    return this.http.delete<void>(`${this.apiUrl}/Documentation/${id}`, { headers });
  }

  private getToken(): string | null {
    return localStorage.getItem('token');
  }
}
