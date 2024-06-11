import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reward } from '../models/reward.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RewardsService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUserRewards(userId: string): Observable<Reward[]> {
    return this.http.get<Reward[]>(`${this.apiUrl}/UserRewards/${userId}`);
  }
}
