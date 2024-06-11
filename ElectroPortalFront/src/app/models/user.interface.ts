export interface User {
  id: string;
  userName: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  country?: string;

  isFirstNameVisible: boolean;
  isLastNameVisible: boolean;
  isCountryVisible: boolean;
}
