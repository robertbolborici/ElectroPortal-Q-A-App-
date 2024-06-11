import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PostAnswerComponent } from './post-answer.component';

describe('PostAnswerComponent', () => {
  let component: PostAnswerComponent;
  let fixture: ComponentFixture<PostAnswerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PostAnswerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PostAnswerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
