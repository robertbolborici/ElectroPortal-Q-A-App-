import { Component, OnInit } from '@angular/core';
import { Tag } from '../models/tag.interface';
import { TagService } from '../services/tags.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrl: './tags.component.css'
})
export class TagsComponent implements OnInit {
  tags: Tag[] = [];

  constructor(
    private tagService: TagService,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.tagService.getTags().subscribe({
      next: tags => {
        this.tags = tags;
      }
    });
  }

  viewQuestionsByTag(tagId: number) {
    this.router.navigate(['/questions/tag', tagId]);
  }
}
