export interface CustomerDto {
  id: string;
  name: string | null;
  email: string | null;
  isActive: boolean;
  creationDate: string;
}
