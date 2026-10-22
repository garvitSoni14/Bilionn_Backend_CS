import { validate, SchemaPathTree, maxLength } from '@angular/forms/signals';

export interface IdentityData {
  panNumber: string;
  aadhaarNumber: string;
}

export function createIdentityData(): IdentityData {
  return { panNumber: '', aadhaarNumber: '' };
}

const PAN_REGEX = /^[A-Za-z]{5}\d{4}[A-Za-z]$/;
const AADHAAR_REGEX = /^\d{12}$/;

// Optional: it can beempty other wise inthe correct format.....
export function validateIdentity(path: SchemaPathTree<IdentityData>): void {
  maxLength(path.panNumber, 10, { message: 'PAN must be 10 characters.' });
  maxLength(path.aadhaarNumber, 12, { message: 'Aadhaar must be 12 digits.' });
  validate(path.panNumber, ({ value }) => {
    const v = value();
    if (v === '' || PAN_REGEX.test(v)) return null;
    return { kind: 'pattern', message: 'PAN must look like ABCDE1234F.' };
  });

  validate(path.aadhaarNumber, ({ value }) => {
    const v = value();
    if (v === '' || AADHAAR_REGEX.test(v)) return null;
    return { kind: 'pattern', message: 'Aadhaar must be 12 digits.' };
  });
}
