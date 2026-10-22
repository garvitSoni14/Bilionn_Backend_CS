import {
  maxLength,
  minLength,
  pattern,
  required,
  validate,
  SchemaPathTree,
} from '@angular/forms/signals';

export interface PersonalData {
  employeeCode: string;
  fullName: string;
  dateOfBirth: string; // 'yyyy-MM-dd' (date input yahi deta hai)
  mobileNumber: string;
}

export function createPersonalData(): PersonalData {
  return { employeeCode: '', fullName: '', dateOfBirth: '', mobileNumber: '' };
}

// Aaj ki date 'yyyy-MM-dd' mein (local timezone)
function todayIso(): string {
  const d = new Date();
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

export function validatePersonal(path: SchemaPathTree<PersonalData>): void {
  required(path.employeeCode, { message: 'Employee code is required.' });
  maxLength(path.employeeCode, 30, { message: 'Employee code is too long (max 30).' });
  pattern(path.employeeCode, /^[A-Za-z0-9-]+$/, {
    message: 'Use only letters, numbers and hyphens.',
  });

  required(path.fullName, { message: 'Please enter the full name.' });
  minLength(path.fullName, 2, { message: 'Name must be at least 2 characters.' });
  maxLength(path.fullName, 100, { message: 'Name is too long (max 100).' });

  required(path.dateOfBirth, { message: 'Date of birth is required.' });
  validate(path.dateOfBirth, ({ value }) => {
    const v = value();
    if (v && v > todayIso()) {
      return { kind: 'futureDate', message: 'Date of birth cannot be in the future.' };
    }
    return null;
  });

  

  required(path.mobileNumber, { message: 'Mobile number is required.' });
  pattern(path.mobileNumber, /^[6-9]\d{9}$/, {
    message: 'Enter a valid 10-digit mobile number.',
  });
}