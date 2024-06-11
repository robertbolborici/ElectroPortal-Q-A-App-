import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AuthService } from './services/auth.service';
import { PostQuestionComponent } from './questions/post-question/post-question.component';
import { AuthInterceptor } from './auth.interceptor';
import { HomeComponent } from './home/home.component';
import { NavBarComponent } from './core/nav-bar/nav-bar.component';
import { CategoriesComponent } from './questions/categories/categories.component';
import { SideBarComponent } from './core/side-bar/side-bar.component';
import { QuestionsGridComponent } from './questions-grid/questions-grid.component';
import { EditQuestionComponent } from './questions/edit-question/edit-question.component';
import { AboutComponent } from './core/nav-bar/about/about.component';
import { PostAnswerComponent } from './answers/post-answer/post-answer.component';
import { TagsComponent } from './tags/tags.component';
import { QuestionDetailComponent } from './questions/question-detail/question-detail.component';
import { UserProfileComponent } from './users/user-profile/user-profile.component';
import { LeaderboardComponent } from './core/leaderboard/leaderboard.component';
import { QuestionsListComponent } from './questions/questions-list/questions-list.component';
import { DocumentationComponent } from './core/nav-bar/documentation/documentation.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, data: { showNavbar: false } },
  { path: 'register', component: RegisterComponent },
  { path: 'post-question', component: PostQuestionComponent },
  { path: 'home', component: HomeComponent},
  { path: 'edit-question/:id', component: EditQuestionComponent },
  { path: 'about', component: AboutComponent },
  { path: 'home', component: SideBarComponent },
  { path: 'questions', component: QuestionsListComponent },
  { path: 'tags', component: TagsComponent },
  { path: 'question/:id', component: QuestionDetailComponent },
  { path: 'questions/tag/:tagId', component: QuestionsGridComponent },
  { path: 'user-profile/:id', component: UserProfileComponent },
  { path: 'documentation', component: DocumentationComponent },
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    PostQuestionComponent,
    NavBarComponent,
    HomeComponent,
    CategoriesComponent,
    SideBarComponent,
    QuestionsGridComponent,
    EditQuestionComponent,
    PostAnswerComponent,
    TagsComponent,
    QuestionDetailComponent,
    UserProfileComponent,
    LeaderboardComponent,
    QuestionsListComponent,
    DocumentationComponent,
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(routes),
  ],
  providers: [AuthService, { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }],
  
  bootstrap: [AppComponent],
})
export class AppModule { }
