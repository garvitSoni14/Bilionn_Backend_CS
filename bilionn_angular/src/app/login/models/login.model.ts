
import {
  email,
  maxLength,
  required,
  SchemaPathTree,
} from '@angular/forms/signals';

export interface LoginFormData {
  email: string;
  password: string;
}

export function createLoginData(): LoginFormData {
  return {
    email: '',
    password: '',
  };
}

export function validateLogin(
  path: SchemaPathTree<LoginFormData>,
): void {
  required(path.email, {
    message: 'Enter your email address.',
  });

  email(path.email, {
    message: 'Enter a valid email address.',
  });

  maxLength(path.email, 120, {
    message: 'Email address is too long.',
  });

  required(path.password, {
    message: 'Enter your password.',
  });
}
