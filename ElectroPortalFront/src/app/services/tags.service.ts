import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Tag } from '../models/tag.interface';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/Tags`);
  }

  getTagsForQuestion(questionId: string): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/Tags/question/${questionId}`);
  }

  getTagsByUserId(userId: string): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/Tags/ByUser/${userId}`);
  }

  getTagsByCategory(categoryId: number): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/Tags/ByCategory/${categoryId}`);
  }
}