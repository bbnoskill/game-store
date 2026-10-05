export interface User {
  id: string;
  name: string;
  roles?: string[];
}

export interface LoginRequest {
  model: {
    login: string;
    password?: string;
  };
}

export interface LoginResponse {
  token: string;
}

export interface CreateUserRequest {
  user: {
    name: string;
    password?: string;
  };
  roles?: string[];
}

export interface UpdateUserRequest {
  user: {
    id: string;
    name: string;
    password?: string;
  };
  roles?: string[];
}
