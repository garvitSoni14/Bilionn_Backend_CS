import { maxLength, pattern, required, SchemaPathTree } from '@angular/forms/signals';

export interface AddressData {
  houseNo: string;
  street: string;
  area: string;
  city: string;
  state: string;
  pincode: string;
}

export function createAddressData(): AddressData {
  return { houseNo: '', street: '', area: '', city: '', state: '', pincode: '' };
}

export function validateAddress(path: SchemaPathTree<AddressData>): void {
  maxLength(path.houseNo, 50, { message: 'House number is too long (max 50).' });

  required(path.street, { message: 'Street is required.' });
  maxLength(path.street, 150, { message: 'Street is too long (max 150).' });

  maxLength(path.area, 150, { message: 'Area is too long (max 150).' });

  required(path.city, { message: 'City is required.' });
  maxLength(path.city, 100, { message: 'City is too long (max 100).' });

  required(path.state, { message: 'State is required.' });
  maxLength(path.state, 100, { message: 'State is too long (max 100).' });

  required(path.pincode, { message: 'Pincode is required.' });
  pattern(path.pincode, /^[1-9]\d{5}$/, { message: 'Pincode must be 6 digits.' });
}