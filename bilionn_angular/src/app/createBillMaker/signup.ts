import { ChangeDetectionStrategy, Component, signal, inject, HostListener } from '@angular/core';
import { form, submit } from '@angular/forms/signals';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { PersonalSection } from './Signup/sections/Personal/personal-section';
import { AddressSection } from './Signup/sections/Address/address-section';
import { EmploymentSection } from './Signup/sections/Employee/employment-section';
import { IdentitySection } from './Signup/sections/Identity/identity-section';
import { BankSection } from './Signup/sections/Bank/bank-section';
import { EmergencySection } from './Signup/sections/Emergency/emergency-section';
import { AccountSection } from './Signup/sections/Account/account-section';
import { createSignupFormData, SignupFormData, validateSignup } from './signup.model';
import { CashierService } from './cashier.service';
import { LoadingService } from '../interceptors/loading-interceptor/loadin.service';

@Component({
  selector: 'app-signup',
  imports: [
    PersonalSection,
    AddressSection,
    EmploymentSection,
    IdentitySection,
    BankSection,
    EmergencySection,
    AccountSection,
    RouterLink,
  ],
  templateUrl: './signup.html',
  // changeDetection: ChangeDetectionStrategy.Eager,    using default:  OnPush
  styleUrl: './signup.css',
})
export class CreateBillMaker {
  // readonly loading = signal(false);

  private readonly cashierService = inject(CashierService);
  private readonly loadingService = inject(LoadingService);
  readonly loading = this.loadingService.isLoading;

  readonly successMessage = signal('');
  readonly errorMessage = signal('');
  readonly adminLoginRequired = signal(false);

  // removed because we dont need it since we are using loading interceptor
  // private readonly http = inject(HttpClient);

  readonly model = signal<SignupFormData>(createSignupFormData());
  readonly signupForm = form(this.model, (path) => validateSignup(path));

// This signal will set the formSubmitted as true after successfull registration..
      // removed because we dont neet it since we are using loading interceptor

  // readonly formSubmittedSuccessfully = signal(false);

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    this.successMessage.set('');
    this.errorMessage.set('');
    this.adminLoginRequired.set(false);

    await submit(this.signupForm, async () => {
      const formData = this.model();

      const payload = {
        employeeCode: formData.personal.employeeCode,
        fullName: formData.personal.fullName,
        dateOfBirth: formData.personal.dateOfBirth,
        mobileNumber: formData.personal.mobileNumber,
        address: formData.address,
        joiningDate: formData.employment.joiningDate,
        salary: Number(formData.employment.salary),
        panNumber: formData.identity.panNumber || null,
        aadhaarNumber: formData.identity.aadhaarNumber || null,
        bankDetails: formData.bank,
        emergencyContact: formData.emergency,
        email: formData.account.email,
        password: formData.account.password,
      };

            // removed because we dont neet it since we are using loading interceptor

      // this.loading.set(true); // loading interceptor + Auth inter...

      try {
      //   await firstValueFrom(
      //     this.http.post(
      //       'http://localhost:8080/api/v1.0/admin/cashiers',
      //       payload,
      //     { withCredentials: true },
      //   )
      // );
      await firstValueFrom(this.cashierService.createCashier(payload));

      // SUCCESS: on success show this message...
        this.successMessage.set('Cashier account created successfully.');        
        
        // (reset model to initial state without reseting the touched so validaiton were showing red error on whole form)
        // this.model.set(createSignupFormData());   

        // this.formSubmittedSuccessfully.set(true);

        this.signupForm().reset(createSignupFormData());

        console.log('Cashier account created successfully.', payload);

      } catch (error) {
        const httpError = error as HttpErrorResponse;
        if (httpError.status === 401 || httpError.status === 403) {
          this.adminLoginRequired.set(true);

          this.errorMessage.set('Sign in as an administrator in this browser, then try again.');
        } else {
          const message = httpError.error?.message ?? httpError.error?.detail;
          this.errorMessage.set(message || (httpError.status === 0
            ? 'Could not reach the server. Check that the backend is running and try again.'
            : 'We could not create the cashier. Check the details and try again.'));
        }
      }
      // removed because we dont neet it since we are using loading interceptor
      // } finally {
      //   this.loading.set(false);
      // }
      return null;
    });
  }
  hasUnsavedChanges(): boolean {
    return this.signupForm().dirty() && !this.loading();
  }
  // Covers refresh / tab close / typed URL
  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent): void {
    if (this.hasUnsavedChanges()) {
      event.preventDefault();
    }
  }
}


