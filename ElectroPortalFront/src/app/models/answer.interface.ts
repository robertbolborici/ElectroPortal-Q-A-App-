export interface Answer {
  id?: string;
  content: string;
  questionId: string;
  userId: string;
  userName: string;
  upVotes?: number;
  downVotes?: number;
  userVote?: number;
}
