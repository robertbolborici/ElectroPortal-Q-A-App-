import { Component, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { QuestionsService } from '../services/questions.service';
import { filter, lastValueFrom } from 'rxjs';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { SearchService } from '../services/search.service';
import { Question } from '../models/question.interface';

@Component({
  selector: 'app-questions-grid',
  templateUrl: './questions-grid.component.html',
  styleUrls: ['./questions-grid.component.css']
})
export class QuestionsGridComponent implements OnInit {
  @Input() questions: Question[] = [];
  searchPerformed = false;
  searchQuery = '';
  expandedQuestions: Set<string> = new Set<string>(); 
  currentUserId: string | null = null;
  isAdmin = false;

  constructor(
    private questionService: QuestionsService,
    private router: Router,
    private authService: AuthService,
    private searchService: SearchService,
    private activatedRoute: ActivatedRoute,
  ) {}
 
  navigateToPostQuestion() {
    if (!this.currentUserId) {
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/post-question']);
    }
  }

  navigateToEditQuestion(questionId: string): void {
    this.router.navigate(['/edit-question', questionId]);
  }

  deleteQuestion(questionId: string): void {
    if(confirm("Are you sure you want to delete this question?")) {
      this.questionService.deleteQuestion(questionId).subscribe({
        next: () => {
          this.questions = this.questions.filter(question => question.id !== questionId);
        }
      });
    }
  }

  toggleAnswersVisibility(questionId: string): void {
    if (this.expandedQuestions.has(questionId)) {
      this.expandedQuestions.delete(questionId);
    } else {
      this.expandedQuestions.add(questionId);
    }
  }

  isExpanded(questionId: string): boolean {
    return this.expandedQuestions.has(questionId);
  }

  ngOnInit(): void {
    this.currentUserId = this.authService.getCurrentUserId();
    this.isAdminOrNot().then(() => {
      this.activatedRoute.params.subscribe(params => {
        const tagId = params['tagId'];
        if (tagId) {
          this.fetchQuestionsByTag(tagId);
        } else {
          this.activatedRoute.queryParams.subscribe(queryParams => {
            this.searchQuery = queryParams['query'];
            this.searchPerformed = !!this.searchQuery;
            if (this.searchQuery) {
              this.performSearch(this.searchQuery);
            } else {
              this.fetchAllQuestions();
            }
          });
        }
      });
    });
  
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      if (!this.searchPerformed && !this.activatedRoute.snapshot.params['tagId']) {
        this.fetchAllQuestions();
      }
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

  private fetchQuestionsByTag(tagId: number): void {
    this.questionService.getQuestionsByTag(tagId).subscribe(questions => {
      this.processQuestions(questions);
    });
  }

  performSearch(query: string): void {
    this.searchService.searchQuestions(query).subscribe({
      next: (response: any) => {
        const questions = response.$values || response;
        if (!questions || questions.length === 0) {
          this.questions = [];
        } else {
          this.processQuestions(questions);
        }
        this.searchPerformed = true;
      }
    });
  }
  
  fetchAllQuestions(): void {
    this.searchPerformed = false;
    this.questionService.getQuestions().subscribe({
      next: (questions: Question[]) => {
        this.processQuestions(questions);
      }
    });
  }

  private processQuestions(questions: Question[]): void {
    const extendedQuestions = questions.map(question => ({
      ...question,
      isOwner: question.userId === this.authService.getCurrentUserId(),
      questionTags: question.questionTags || []
    }));
    this.questions = extendedQuestions;
  }

  viewQuestionsByTag(tagId: number): void {
    this.router.navigate(['/questions/tag', tagId]);
  }
}
