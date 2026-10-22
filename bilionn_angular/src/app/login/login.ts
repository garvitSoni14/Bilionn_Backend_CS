import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../interceptors/auth-interceptor/auth.service';
import { LoadingService } from '../interceptors/loading-interceptor/loadin.service';

import { form, submit } from '@angular/forms/signals';
import { createLoginData, LoginFormData, validateLogin } from './models/login.model';
import { EmailSection } from './sections/email-section';
import { PasswordSection } from './sections/password-section';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [EmailSection, PasswordSection, RouterLink],
  templateUrl: './login.html',
  // changeDetection: ChangeDetectionStrategy.Eager,
  encapsulation: ViewEncapsulation.None,
  styleUrl: './login.css',
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly loadingService = inject(LoadingService);

  readonly loading = this.loadingService.isLoading;
  readonly errorMessage = signal('');

  readonly model = signal<LoginFormData>(createLoginData());

  readonly loginForm = form(this.model, (path) => validateLogin(path));

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();
    this.errorMessage.set('');

    await submit(this.loginForm, async () => {
      // Read the current form values from the model signal.
      const credentials = this.model();

      try {
        await firstValueFrom(this.authService.login(credentials));

        await this.router.navigateByUrl('/dashboard');
      } catch (error) {
        const httpError = error as HttpErrorResponse;

        this.errorMessage.set(
          httpError.status === 0
            ? 'Could not reach the server. Check that the backend is running and try again.'
            : httpError.status === 401 || httpError.status === 400
              ? 'Email or password is incorrect.'
              : 'We could not sign you in. Please try again.',
        );
      }
    });
  }
}
