import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuestionsGridComponent } from './questions-grid.component';

describe('QuestionsGridComponent', () => {
  let component: QuestionsGridComponent;
  let fixture: ComponentFixture<QuestionsGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionsGridComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionsGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
