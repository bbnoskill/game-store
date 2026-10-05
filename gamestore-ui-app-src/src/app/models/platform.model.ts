export interface Platform {
  id: string;
  type: string;
}

export interface CreatePlatformRequest {
  platform: {
    type: string;
  };
}

export interface UpdatePlatformRequest {
  platform: {
    id: string;
    type: string;
  };
}
