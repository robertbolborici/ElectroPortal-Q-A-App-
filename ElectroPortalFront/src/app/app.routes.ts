import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { PostQuestionComponent } from './questions/post-question/post-question.component';
import { HomeComponent } from './home/home.component';
import { EditQuestionComponent } from './questions/edit-question/edit-question.component';
import { AboutComponent } from './core/nav-bar/about/about.component';
import { TagsComponent } from './tags/tags.component';
import { QuestionDetailComponent } from './questions/question-detail/question-detail.component';
import { UserProfileComponent } from './users/user-profile/user-profile.component';
import { QuestionsListComponent } from './questions/questions-list/questions-list.component';
import { DocumentationComponent } from './core/nav-bar/documentation/documentation.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'post-question', component: PostQuestionComponent },
  { path: 'home', component: HomeComponent },
  { path: 'edit-question/:id', component: EditQuestionComponent },
  { path: 'about', component: AboutComponent },
  { path: 'questions', component: QuestionsListComponent },
  { path: 'tags', component: TagsComponent },
  { path: 'question/:id', component: QuestionDetailComponent },
  { path: 'user-profile/:id', component: UserProfileComponent },
  { path: 'documentation', component: DocumentationComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' }
];
