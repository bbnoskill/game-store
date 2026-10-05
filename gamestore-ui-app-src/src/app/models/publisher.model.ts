export interface Publisher {
  id: string;
  companyName: string;
  homePage?: string;
  description?: string;
}

export interface CreatePublisherRequest {
  publisher: {
    companyName: string;
    homePage?: string;
    description?: string;
  };
}

export interface UpdatePublisherRequest {
  publisher: {
    id: string;
    companyName: string;
    homePage?: string;
    description?: string;
  };
}
