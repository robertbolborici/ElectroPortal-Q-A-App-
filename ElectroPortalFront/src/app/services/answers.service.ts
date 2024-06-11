import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Answer } from '../models/answer.interface';

@Injectable({
  providedIn: 'root'
})
export class AnswersService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  postAnswer(answerData: Answer): Observable<Answer> {
    const token = this.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<Answer>(`${this.apiUrl}/Answers`, answerData, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  getAnswersByQuestionId(questionId: string): Observable<Answer[]> {
    return this.http.get<Answer[]>(`${this.apiUrl}/Answers/ByQuestion/${questionId}`);
  }

  getAnswersByUsersId(userId: string): Observable<Answer[]> {
    return this.http.get<Answer[]>(`${this.apiUrl}/Answers/ByUser/${userId}`);
  }

  getResponsesToUserQuestions(userId: string): Observable<Answer[]> {
    return this.http.get<Answer[]>(`${this.apiUrl}/Answers/ResponsesToUserQuestions/${userId}`);
  }

  editAnswer(answerId: string, updatedData: any): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return this.http.put(`${this.apiUrl}/Answers/${answerId}`, updatedData, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  deleteAnswer(answerId: string): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return this.http.delete(`${this.apiUrl}/Answers/${answerId}`, { headers }).pipe(
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

  private getToken(): string | null {
    return localStorage.getItem('token');
  }
}
