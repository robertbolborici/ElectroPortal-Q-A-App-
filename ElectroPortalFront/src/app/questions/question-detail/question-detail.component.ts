import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QuestionsService } from '../../services/questions.service';
import { UserService } from '../../services/user.service';
import { AnswersService } from '../../services/answers.service';
import { TagService } from '../../services/tags.service';
import { forkJoin, lastValueFrom, map } from 'rxjs';
import { Answer } from '../../models/answer.interface';
import { VotesService } from '../../services/votes.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-question-detail',
  templateUrl: './question-detail.component.html',
  styleUrls: ['./question-detail.component.css']
})
export class QuestionDetailComponent implements OnInit {
  question: any;
  expanded = true;
  editingAnswerId: string | null = null;
  originalAnswerContent: string = '';
  currentUserId: string | null = null;
  isAdmin = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private questionService: QuestionsService,
    private userService: UserService,
    private answersService: AnswersService,
    private authService: AuthService,
    private votesService: VotesService,
    private tagService: TagService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getCurrentUserId();
    this.isAdminOrNot().then(() => {
        this.activatedRoute.params.subscribe(params => {
            const questionId = params['id'];
            this.fetchQuestionDetail(questionId);
        });
    });
  }

  fetchQuestionDetail(questionId: string): void {
    this.questionService.getQuestionById(questionId).subscribe(question => {
        forkJoin({
            user: this.userService.getUserById(question.userId),
            answers: this.answersService.getAnswersByQuestionId(question.id),
            tags: this.tagService.getTagsForQuestion(question.id)
        }).pipe(
            map(({ user, answers, tags }) => ({
                ...question,
                userName: user.userName,
                isOwner: question.userId === this.currentUserId || this.isAdmin,
                answers: answers.map(answer => ({
                    ...answer,
                    upVotes: answer.upVotes || 0,
                    downVotes: answer.downVotes || 0,
                    userVote: answer.userVote || 0,
                    isOwner: answer.userId === this.currentUserId || this.isAdmin
                })),
                questionTags: tags
            }))
        ).subscribe(extendedQuestion => {
            this.question = extendedQuestion;
        });
    });
  }

  async isAdminOrNot() {
    try {
      this.isAdmin = await lastValueFrom(this.authService.checkIfUserIsAdmin());
    } catch (error) {
      this.isAdmin = false;
    }
  }

  canEditOrDelete(questionUserId: string): boolean {
    return this.isAdmin || this.currentUserId === questionUserId;
  }

  navigateToEditQuestion(questionId: string): void {
    this.router.navigate(['/edit-question', questionId]);
  }

  deleteQuestion(questionId: string): void {
    if(confirm("Are you sure you want to delete this question?")) {
      this.questionService.deleteQuestion(questionId).subscribe({
        next: () => {
          this.router.navigate(['/']);
        }
      });
    }
  }

  vote(answerId: string, voteValue: number): void {
    const voteDto = {
      AnswerId: answerId,
      Upvote: voteValue === 1
    };
    this.votesService.postVote(voteDto).subscribe({
      next: (voteUpdateResponse) => {
        this.fetchQuestionDetail(this.question.id);
      }
    });
  }  

  showMenuForAnswer: { [key: string]: boolean } = {};
  
  toggleMenu(answerId: string): void {
    this.showMenuForAnswer[answerId] = !this.showMenuForAnswer[answerId];
  }

  editAnswer(answerId: string): void {
    const answer = this.question.answers.find((a:Answer) => a.id === answerId);
    if (answer) {
      this.editingAnswerId = answerId;
      this.originalAnswerContent = answer.content;
    }
  }

  saveAnswer(answerId: string, newContent: string): void {
    const updatedData = { content: newContent };
    this.answersService.editAnswer(answerId, updatedData).subscribe({
      next: () => {
        this.editingAnswerId = null;
        const answerIndex = this.question.answers.findIndex((a:Answer) => a.id === answerId);
        if (answerIndex !== -1) {
          this.question.answers[answerIndex].content = newContent;
        }
      },
      error: (error) => {
        if (error.includes('inappropriate language')) {
          alert('Your answer contains inappropriate language.');
        } else {
          alert('An error occurred while updating the answer. Please try again.');
        }
      }
    });
  }
  
  cancelEdit(): void {
    if (this.editingAnswerId) {
      const answerIndex = this.question.answers.findIndex((a:Answer) => a.id === this.editingAnswerId);
      if (answerIndex !== -1) {
        this.question.answers[answerIndex].content = this.originalAnswerContent;
      }
    }
    this.editingAnswerId = null;
  }

  deleteAnswer(answerId: string): void {
    if(confirm("Are you sure you want to delete this answer?")) {
      this.answersService.deleteAnswer(answerId).subscribe({
        next: () => {
          this.fetchQuestionDetail(this.question.id);
        }
      });
    }
  }

  onAnswerPosted(): void {
    this.fetchQuestionDetail(this.question.id);
  }

  navigateToUserProfile(userId: string): void {
    this.router.navigate(['/user-profile', userId]);
  }

  viewQuestionsByTag(tagId: number): void {
    this.router.navigate(['/questions/tag', tagId]);
  }
}
