import { Answer } from "./answer.interface";

export interface Vote {
  id: string;
  userId: string;
  answerId: string;
  upvote: boolean;
  answer?: Answer;
}