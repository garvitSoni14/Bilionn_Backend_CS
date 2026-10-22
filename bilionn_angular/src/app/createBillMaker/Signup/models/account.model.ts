import {
  email,
  maxLength,
  minLength,
  required,
  validate,
  SchemaPathTree,
} from '@angular/forms/signals';

// 1. DATA shape
export interface AccountData {
  email: string;
  password: string;
  confirmPassword: string;
}

// 2. Empty starting data
export function createAccountData(): AccountData {
  return {
    email: '',
    password: '',
    confirmPassword: '',
  };
}

// 3. RULES
export function validateAccount(path: SchemaPathTree<AccountData>): void {
  required(path.email, { message: 'Email is required.' });
  email(path.email, { message: 'Enter a valid email address.' });
  maxLength(path.email, 120, { message: 'Email is too long.' });

  required(path.password, { message: 'Password is required.' });
  minLength(path.password, 8, { message: 'Use at least 8 characters in password.' });
  validate(path.password, ({ value }) => {
    const v = value();
    if (!v) return null; // khaali hone ka error upar wala required dega
    if (!/[A-Z]/.test(v)) return { kind: 'noUpper', message: 'Add at least one capital letter.' };
    if (!/[a-z]/.test(v)) return { kind: 'noLower', message: 'Add at least one small letter.' };
    if (!/\d/.test(v)) return { kind: 'noDigit', message: 'Add at least one number.' };
    if (!/[^A-Za-z0-9]/.test(v)) return { kind: 'noSymbol', message: 'Add at least one symbol (e.g. @ # $).' };
    return null;
  });

  required(path.confirmPassword, { message: 'Please confirm your password.' });
  validate(path.confirmPassword, ({ value, valueOf }) => {
    if (value() !== valueOf(path.password)) {
      return { kind: 'passwordMismatch', message: 'Passwords do not match.' };
    }
    return null;
  });
}