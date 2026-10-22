  import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';import { provideHttpClient, withInterceptors, withXhr } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { AuthService } from './interceptors/auth-interceptor/auth.service';

import { routes } from './app.routes';
import { loadingInterceptor } from './interceptors/loading-interceptor/loading.interceptor';
import { authInterceptor } from './interceptors/auth-interceptor/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    // provideHttpClient(withXhr()),
    provideHttpClient(withInterceptors([authInterceptor, loadingInterceptor])),  //using loading interceptor, aut interceptor
    provideRouter(routes),
    provideAppInitializer(() => inject(AuthService).restoreSession()),
  ],
};
