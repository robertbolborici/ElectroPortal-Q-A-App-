import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Question } from '../../models/question.interface';
import { Tag } from '../../models/tag.interface';
import { QuestionsService } from '../../services/questions.service';
import { CategoriesService } from '../../services/categories.service';
import { TagService } from '../../services/tags.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-questions-list',
  templateUrl: './questions-list.component.html',
  styleUrl: './questions-list.component.css'
})
export class QuestionsListComponent implements OnInit {
  questions: Question[] = [];
  tags: Tag[] = [];

  constructor(
    private questionsService: QuestionsService,
    private categoriesService: CategoriesService,
    private tagService: TagService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
    this.tagService.getTags().subscribe({
      next: (data) => {
        this.tags = data;
      }
    });

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.loadQuestions();
    });
  }

  loadQuestions(): void {
    this.questionsService.getQuestions().subscribe({
      next: (questions) => {
        this.questions = questions;
      }
    });
  }
}
