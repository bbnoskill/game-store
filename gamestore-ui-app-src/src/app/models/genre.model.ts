export interface Genre {
  id: string;
  name: string;
  parentGenreId?: string | null;
}

export interface CreateGenreRequest {
  genre: {
    name: string;
    parentGenreId?: string | null;
  };
}

export interface UpdateGenreRequest {
  genre: {
    id: string;
    name: string;
    parentGenreId?: string | null;
  };
}
