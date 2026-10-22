import { pattern, required, validate, SchemaPathTree } from '@angular/forms/signals';

export interface EmploymentData {
  joiningDate: string; // 'yyyy-MM-dd'
  salary: string; // string rakha hai, API bhejte waqt number banayenge
}

export function createEmploymentData(): EmploymentData {
  return { joiningDate: '', salary: '' };
}

export function validateEmployment(path: SchemaPathTree<EmploymentData>): void {
  required(path.joiningDate, { message: 'Joining date is required.' });

  required(path.salary, { message: 'Salary is required.' });
  pattern(path.salary, /^\d{1,10}(\.\d{1,2})?$/, {
    message: 'Enter a valid amount (max 2 decimals).',
  });
  validate(path.salary, ({ value }) => {
    const v = value();
    if (v && Number(v) <= 0) {
      return { kind: 'min', message: 'Salary must be greater than 0.' };
    }
    return null;
  });
}