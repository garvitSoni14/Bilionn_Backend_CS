import { maxLength, pattern, required, SchemaPathTree } from '@angular/forms/signals';

export interface EmergencyData {
  name: string;
  mobileNumber: string;
  relationship: string;
}

export function createEmergencyData(): EmergencyData {
  return { name: '', mobileNumber: '', relationship: '' };
}

export function validateEmergency(path: SchemaPathTree<EmergencyData>): void {
  required(path.name, { message: 'Contact name is required.' });
  maxLength(path.name, 100, { message: 'Name is too long (max 100).' });

  required(path.mobileNumber, { message: 'Contact mobile number is required.' });
  pattern(path.mobileNumber, /^[6-9]\d{9}$/, {
    message: 'Enter a valid 10-digit mobile number.',
  });

  required(path.relationship, { message: 'Relationship is required.' });
  maxLength(path.relationship, 50, { message: 'Relationship is too long (max 50).' });
}