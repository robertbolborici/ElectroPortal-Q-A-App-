import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Question } from '../models/question.interface';

@Injectable({
  providedIn: 'root'
})
export class QuestionsService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  postQuestion(questionData: any): Observable<any> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/Questions`, questionData, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  putQuestion(questionId: string, questionData: any): Observable<any> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put(`${this.apiUrl}/Questions/${questionId}`, questionData, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = error.error;
    }
    return throwError(errorMessage);
  }

  getQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(`${this.apiUrl}/Questions`);
  }

  getQuestionsByTag(tagId: number): Observable<Question[]> {
    return this.http.get<Question[]>(`${this.apiUrl}/Questions/byTag/${tagId}`);
  }

  getQuestionById(questionId: string): Observable<Question> {
    return this.http.get<Question>(`${this.apiUrl}/Questions/${questionId}`);
  }

  getQuestionsByUser(userId: string): Observable<Question[]> {
    return this.http.get<Question[]>(`${this.apiUrl}/Questions/user/${userId}`);
  }

  deleteQuestion(questionId: string): Observable<any> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/Questions/${questionId}`, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  private getToken(): string | null {
    return localStorage.getItem('token');
  }
}
