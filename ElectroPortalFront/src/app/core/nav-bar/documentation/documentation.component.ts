import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Documentation } from '../../../models/documentation.interface';
import { DocumentationService } from '../../../services/documentation.service';
import { AuthService } from '../../../services/auth.service';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-documentation',
  templateUrl: './documentation.component.html',
  styleUrls: ['./documentation.component.css']
})
export class DocumentationComponent implements OnInit {
  documentations: Documentation[] = [];
  selectedDocumentation: Documentation | null = null;
  isAdmin: boolean = false;
  showMenuForDocumentation: { [key: string]: boolean } = {}; // Track menu state

  constructor(private documentationService: DocumentationService, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadDocumentations();
    this.isAdminOrNot();
  }

  async isAdminOrNot() {
    try {
      this.isAdmin = await lastValueFrom(this.authService.checkIfUserIsAdmin());
    } catch (error) {
      this.isAdmin = false;
    }
  }

  loadDocumentations(): void {
    this.documentationService.getDocumentations().subscribe(
      data => {
        this.documentations = data;
      }
    );
  }

  selectDocumentation(documentation: Documentation): void {
    this.selectedDocumentation = { ...documentation };
  }

  saveDocumentation(documentation: Documentation): void {
    if (documentation.id) {
      this.documentationService.updateDocumentation(documentation.id, documentation).subscribe(
        () => {
          this.loadDocumentations();
          this.selectedDocumentation = null;
        },
      );
    } else {
      this.documentationService.createDocumentation(documentation).subscribe(
        () => {
          this.loadDocumentations();
          this.selectedDocumentation = null;
        },
      );
    }
  }

  deleteDocumentation(documentation: Documentation): void {
    const confirmDelete = window.confirm('Are you sure you want to delete this?');
    if (confirmDelete) {
      this.documentationService.deleteDocumentation(documentation.id).subscribe(
        () => this.loadDocumentations(),
      );
    }
  }

  cancel(): void {
    this.selectedDocumentation = null;
  }

  toggleMenu(docId: number): void {
    this.showMenuForDocumentation[docId] = !this.showMenuForDocumentation[docId];
  }
}
