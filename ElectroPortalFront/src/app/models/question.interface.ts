import { Answer } from "./answer.interface";
import { Category } from "./category.interface";
import { Tag } from "./tag.interface";
import { User } from "./user.interface";

export interface Question {
  id: string;
  title: string;
  content: string;
  createdAt: Date;
  userId: string;
  categoryId: number;
  user?: User;
  category?: Category;
  answers?: Answer[];
  questionTags?: Tag[];
  showAllAnswers?: boolean;
  userName: string;
}

export interface SearchResponse {
  $id: string;
  $values: Question[];
}