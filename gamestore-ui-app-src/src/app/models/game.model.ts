export interface Game {
  id: string;
  name: string;
  key: string;
  description?: string;
  price: number;
  unitInStock: number;
  discount: number;
  viewCount?: number;
  releaseDate?: string;
  quantityPerUnit?: string;
  reorderLevel?: number;
  discontinued?: boolean;
  publisherId?: string;
  publisherName?: string;
  genreIds?: string[];
  platformIds?: string[];
  genres?: { id: string; name: string }[];
  platforms?: { id: string; type: string }[];
  image?: string;
}

export interface CreateGameRequest {
  game: {
    name: string;
    key: string;
    description?: string;
    price: number;
    unitInStock: number;
    discount: number;
    publisherId?: string;
  };
  genres: string[];
  platforms: string[];
  publisher?: string;
}

export interface UpdateGameRequest {
  game: {
    id: string;
    name: string;
    key: string;
    description?: string;
    price: number;
    unitInStock: number;
    discount: number;
    publisherId?: string;
  };
  genres: string[];
  platforms: string[];
  publisher?: string;
}
