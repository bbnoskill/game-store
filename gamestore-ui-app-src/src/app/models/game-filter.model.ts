import { Game } from './game.model';

export interface GameFilter {
  name?: string;
  sortBy?: string;
  publishDate?: string;
  pageSize?: string;
  page?: number;
  minPrice?: number | null;
  maxPrice?: number | null;
  publishers?: string[];
  platforms?: string[];
  genres?: string[];
}

export interface GameFilterResult {
  games: Game[];
  totalPages: number;
  currentPage: number;
}
