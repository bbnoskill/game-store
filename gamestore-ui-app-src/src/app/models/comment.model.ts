export interface Comment {
  id: string;
  name: string;
  body: string;
  childComments?: Comment[];
  likes?: number;
  rating?: number;
  date?: string | Date;
}

export interface CreateCommentRequest {
  comment: {
    name: string;
    body: string;
  };
  parentCommentId?: string | null;
  action?: string | null;
}
