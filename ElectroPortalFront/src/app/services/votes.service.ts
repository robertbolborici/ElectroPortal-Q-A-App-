import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserScore } from '../models/userscore.interface';

@Injectable({
  providedIn: 'root'
})
export class VotesService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  postVote(voteDto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Votes`, voteDto);
  }

  updateVote(id: string, voteUpdateDto: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/Votes/${id}`, voteUpdateDto);
  }

  getVoteByAnswerIdAndUserId(answerId: string, userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/Votes?answerId=${answerId}&userId=${userId}`);
  }

  getVotesByUserId(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/Votes/ByUserId?userId=${userId}`);
  }

  getLeaderboard(): Observable<any> {
    return this.http.get(`${this.apiUrl}/Votes/Leaderboard`);
  }

  getScore(userId: string): Observable<UserScore> {
    return this.http.get<UserScore>(`${this.apiUrl}/Votes/Score/${userId}`);
  }

  deleteVote(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Votes/${id}`);
  }
}
