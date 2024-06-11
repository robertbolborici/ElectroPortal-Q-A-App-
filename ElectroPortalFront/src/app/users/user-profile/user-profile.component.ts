import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user.interface';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Question } from '../../models/question.interface';
import { QuestionsService } from '../../services/questions.service';
import { AnswersService } from '../../services/answers.service';
import { Answer } from '../../models/answer.interface';
import { TagService } from '../../services/tags.service';
import { Tag } from '../../models/tag.interface';
import { VotesService } from '../../services/votes.service';
import { Vote } from '../../models/vote.interface';
import { UserScore } from '../../models/userscore.interface';
import { AuthService } from '../../services/auth.service';
import { RewardsService } from '../../services/rewards.service';
import { Reward } from '../../models/reward.interface';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  user: User | undefined;
  questions: Question[] = [];
  answers: Answer[] = [];
  userTags: Tag[] = [];
  responses: Answer[] = [];
  votes: Vote[] = [];
  rewards: Reward[] = [];
  show: string = '';
  userScore: number = 0;  
  isProfileOwner: boolean = false;
  showPrivacySettings: boolean = false;

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    private questionsService: QuestionsService,
    private answersService: AnswersService,
    private tagService: TagService,
    private votesService: VotesService,
    private authService: AuthService,
    private rewardsService: RewardsService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const userId = params['id'];
      if (userId) {
        this.userService.getUserById(userId).subscribe(user => {
          this.user = user;
          this.isProfileOwner = this.authService.getCurrentUserId() === userId;
          this.fetchUserQuestions(userId);
          this.fetchUserAnswers(userId);
          this.fetchUserTags(userId);
          this.fetchUserResponses(userId);
          this.fetchUserVotes(userId);
          this.fetchUserRewards(userId);
        });
      }
    });
  }

  updateVisibilitySetting(setting: string, value: boolean): void {
    if (this.user) {
      this.userService.updateUserSettings({ [setting]: value })
      .subscribe({
        next: () => {
          this.refreshUserProfile();
        }
      });
    }
  }

  refreshUserProfile(): void {
    this.userService.getUserById(this.user!.id).subscribe(user => {
        this.user = user;
    });
  }

  fetchUserQuestions(userId: string) {
    this.questionsService.getQuestionsByUser(userId).subscribe(
      questions => this.questions = questions
    );
  }

  fetchUserAnswers(userId: string) {
    this.answersService.getAnswersByUsersId(userId).subscribe(
      answers => this.answers = answers
    );
  }

  fetchUserTags(userId: string) {
    this.tagService.getTagsByUserId(userId).subscribe(
      tags => this.userTags = tags
    );
  }

  fetchUserResponses(userId: string) {
    this.answersService.getResponsesToUserQuestions(userId).subscribe(
      responses => this.responses = responses
    );
  }

  fetchUserVotes(userId: string) {
    this.votesService.getVotesByUserId(userId).subscribe(
      votes => this.votes = votes
    );
  }

  fetchUserRewards(userId: string) {
    this.rewardsService.getUserRewards(userId).subscribe(
      rewards => this.rewards = rewards
    );
  }

  getProfileClass() {
    return this.isProfileOwner ? 'owner-profile' : 'visitor-profile';
  }

  viewQuestionsByTag(tagId: number) {
    this.router.navigate(['/questions/tag', tagId]);
  }

  setActiveTab(tabName: string) {
    this.show = tabName;
  }

  togglePrivacySettings(): void {
    this.showPrivacySettings = !this.showPrivacySettings;
  }
}
