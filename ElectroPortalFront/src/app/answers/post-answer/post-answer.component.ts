import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AnswersService } from '../../services/answers.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-post-answer',
  templateUrl: './post-answer.component.html',
  styleUrls: ['./post-answer.component.css']
})
export class PostAnswerComponent {
  @Input() questionId: string;
  @Output() answerPosted = new EventEmitter<void>();
  answerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private answersService: AnswersService,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {
    this.questionId = this.route.snapshot.paramMap.get('id')!;
    this.answerForm = this.fb.group({
      content: ['', [Validators.required, Validators.minLength(5)]],
    });
  }

  onSubmit(): void {
    const userId = this.authService.getCurrentUserId();
    if (this.answerForm.valid) {
      const answerData = {
        ...this.answerForm.value,
        questionId: this.questionId,
        userId: userId,
      };
      this.answersService.postAnswer(answerData).subscribe({
        next: () => {
          this.answerForm.reset();
          this.answerPosted.emit();
        },
        error: (error) => {
          if (error.includes('inappropriate language')) {
            alert('Your answer contains inappropriate language.');
          } else {
            alert('An error occurred while posting the answer. Please try again.');
          }
        }
      });
    }
  }
}
