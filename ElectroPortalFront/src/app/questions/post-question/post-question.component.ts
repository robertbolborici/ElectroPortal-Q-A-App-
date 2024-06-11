import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { QuestionsService } from '../../services/questions.service';
import { CategoriesService } from '../../services/categories.service';
import { TagService } from '../../services/tags.service';
import { Tag } from '../../models/tag.interface';
import { Router } from '@angular/router';
import { Category } from '../../models/category.interface';

@Component({
  selector: 'app-post-question',
  templateUrl: './post-question.component.html',
  styleUrls: ['./post-question.component.css'],
})
export class PostQuestionComponent implements OnInit {
  questionForm: FormGroup;
  categories: Category[] = [];
  tags: Tag[] = [];
  selectedTagIds: number[] = [];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private questionsService: QuestionsService,
    private categoriesService: CategoriesService,
    private tagService: TagService
  ) {
    this.questionForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      content: ['', [Validators.required, Validators.minLength(10)]],
      categoryId: [null, Validators.required],
      tagIds: [[], Validators.required]
    });
  }

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoriesService.getCategories().subscribe(categories => {
      this.categories = categories;
    });
  }

  loadTags(categoryId: number): void {
    this.tagService.getTagsByCategory(categoryId).subscribe({
      next: (tags) => {
        this.tags = tags;
      }
    });
  }

  onCategoryChange(): void {
    const categoryId = this.questionForm.get('categoryId')?.value;
    if (categoryId) {
      this.loadTags(categoryId);
    } else {
      this.tags = [];
    }
  }

  toggleTagSelection(tagId: number): void {
    const index = this.selectedTagIds.indexOf(tagId);
    if (index > -1) {
      this.selectedTagIds.splice(index, 1);
    } else {
      this.selectedTagIds.push(tagId);
    }
    this.questionForm.get('tagIds')?.setValue(this.selectedTagIds);
  }

  onSubmit(): void {
    if (this.questionForm.valid) {
      this.questionsService.postQuestion(this.questionForm.value).subscribe({
        next: (response) => {
          this.router.navigate(['/questions']);
        },
        error: (error) => {
          if (error.includes('inappropriate language')) {
            alert('Your question contains inappropriate language.');
          } else {
            alert('An error occurred while posting the question. Please try again.');
          }
        }
      });
    }
  }  
}
