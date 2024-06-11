import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { QuestionsService } from '../../services/questions.service';
import { CategoriesService } from '../../services/categories.service';
import { TagService } from '../../services/tags.service';
import { Tag } from '../../models/tag.interface';
import { Question } from '../../models/question.interface';
import { Category } from '../../models/category.interface';

@Component({
  selector: 'app-edit-question',
  templateUrl: './edit-question.component.html',
  styleUrls: ['./edit-question.component.css'],
})
export class EditQuestionComponent implements OnInit {
  questionForm: FormGroup;
  categories: Category[] = [];
  tags: Tag[] = []; // Define the tags property
  selectedTagIds: number[] = []; // Track selected tags
  questionId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private questionService: QuestionsService,
    private categoriesService: CategoriesService,
    private tagService: TagService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.questionForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      content: ['', [Validators.required, Validators.minLength(10)]],
      categoryId: [null, Validators.required],
      tagIds: [[], Validators.required]
    });
  }

  ngOnInit(): void {
    this.questionId = this.route.snapshot.paramMap.get('id');
    if (this.questionId) {
      this.questionService.getQuestionById(this.questionId).subscribe((question: Question) => {
        this.questionForm.patchValue({
          title: question.title,
          content: question.content,
          categoryId: question.categoryId,
          tagIds: question.questionTags?.map((tag: Tag) => tag.id) || []
        });
        this.selectedTagIds = question.questionTags?.map((tag: Tag) => tag.id) || [];
        this.loadTags(question.categoryId);
      });
    }
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoriesService.getCategories().subscribe((data) => {
      this.categories = data;
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
    if (this.questionForm.valid && this.questionId !== null) {
      const updatedQuestion = {
        ...this.questionForm.value,
        tagIds: this.selectedTagIds
      };
      this.questionService.putQuestion(this.questionId, updatedQuestion).subscribe({
        next: () => {
          this.router.navigate(['/questions']);
        },
        error: (error) => {
          if (error.includes('inappropriate language')) {
            alert('Your question contains inappropriate language.');
          } else {
            alert('An error occurred while updating the question. Please try again.');
          }
        }
      });
    }
  }
}
