import { Component, OnInit } from '@angular/core';
import { VotesService } from '../../services/votes.service';

@Component({
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.css']
})
export class LeaderboardComponent implements OnInit {
  userScores: any[] = [];

  constructor(private votesService: VotesService) {}

  ngOnInit() {
    this.votesService.getLeaderboard().subscribe(data => {
      this.userScores = data;
    });
  }
}
