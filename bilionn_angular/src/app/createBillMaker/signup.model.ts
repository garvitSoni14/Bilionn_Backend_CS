import { SchemaPathTree } from '@angular/forms/signals';
import { AccountData, createAccountData, validateAccount } from './Signup/models/account.model';
import { AddressData, createAddressData, validateAddress } from './Signup/models/address.model';
import { BankData, createBankData, validateBank } from './Signup/models/bank.model';
import { EmergencyData, createEmergencyData, validateEmergency } from './Signup/models/emergency.model';
import { IdentityData, createIdentityData, validateIdentity } from './Signup/models/identity.model';
import { PersonalData, createPersonalData, validatePersonal } from './Signup/models/personal.model';
import { createEmploymentData, EmploymentData, validateEmployment } from './Signup/models/employee.model';

// 1. Poore form ka shape: har section ek hissa
export interface SignupFormData {
  personal: PersonalData;
  address: AddressData;
  employment: EmploymentData;
  identity: IdentityData;
  bank: BankData;
  emergency: EmergencyData;
  account: AccountData;
}

// 2. Empty starting data
export function createSignupFormData(): SignupFormData {
  return {
    personal: createPersonalData(),
    address: createAddressData(),
    employment: createEmploymentData(),
    identity: createIdentityData(),
    bank: createBankData(),
    emergency: createEmergencyData(),
    account: createAccountData(),
  };
}

// 3. Sab sections ke rules ek saath
export function validateSignup(path: SchemaPathTree<SignupFormData>): void {
  validatePersonal(path.personal);
  validateAddress(path.address);
  validateEmployment(path.employment);
  validateIdentity(path.identity);
  validateBank(path.bank);
  validateEmergency(path.emergency);
  validateAccount(path.account);
}