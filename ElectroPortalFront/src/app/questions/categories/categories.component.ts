import { Component, OnInit } from '@angular/core';
import { CategoriesService } from '../../services/categories.service';
import { Category } from '../../models/category.interface';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css'
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];

  constructor(private categoriesService: CategoriesService) {}
  ngOnInit() {
    this.categoriesService.getCategories().subscribe({
      next: (data) => {
        this.categories = Array.isArray(data) ? data : [data];
      }
    });
  }
}
