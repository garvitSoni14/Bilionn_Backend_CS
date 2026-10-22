import { maxLength, pattern, required, SchemaPathTree } from '@angular/forms/signals';

export interface BankData {
  accountHolderName: string;
  accountNumber: string;
  ifscCode: string;
  bankName: string;
}

export function createBankData(): BankData {
  return { accountHolderName: '', accountNumber: '', ifscCode: '', bankName: '' };
}

export function validateBank(path: SchemaPathTree<BankData>): void {
  required(path.accountHolderName, { message: 'Account holder name is required.' });
  maxLength(path.accountHolderName, 100, { message: 'Name is too long (max 100).' });

  required(path.accountNumber, { message: 'Account number is required.' });
  pattern(path.accountNumber, /^\d{9,18}$/, {
    message: 'Account number must be 9 to 18 digits.',
  });

  required(path.ifscCode, { message: 'IFSC code is required.' });
  maxLength(path.ifscCode, 11, { message: 'IFSC must be 11 characters.' });
  pattern(path.ifscCode, /^[A-Za-z]{4}0[A-Za-z0-9]{6}$/, {
    message: 'IFSC must look like ABCD0123456.',
  });

  required(path.bankName, { message: 'Bank name is required.' });
  maxLength(path.bankName, 100, { message: 'Bank name is too long (max 100).' });
}