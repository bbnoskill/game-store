export interface Role {
  id: string;
  name: string;
}

export interface RolePermissions {
  id: string;
  name: string;
  permissions: string[];
}

export interface CreateRoleRequest {
  role: {
    name: string;
  };
  permissions?: string[];
}

export interface UpdateRoleRequest {
  role: {
    id: string;
    name: string;
  };
  permissions?: string[];
}
