import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { SearchResponse } from '../models/question.interface';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  searchQuestions(query: string): Observable<SearchResponse> {
    const params = new HttpParams().set('query', query);
    return this.http.get<SearchResponse>(`${this.apiUrl}/Questions/search`, { params })
        .pipe(
            tap(data => data),
            catchError(error => {
                console.error('Search error:', error);
                return throwError(() => new Error('Search error'));
            })
        );
  }

  private searchResultsSource = new BehaviorSubject<any[]>([]);
  public searchResults$ = this.searchResultsSource.asObservable();

  setSearchResults(results: any[]): void {
    this.searchResultsSource.next(results);
  }

  clearSearchResults(): void {
    this.searchResultsSource.next([]);
  }
}
