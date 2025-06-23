import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpRequestInterceptor } from './_helpers/http.interceptor';
import { AuthService } from './_services/auth/auth.service';
import { HelperService } from './_helpers/helper-service/helper.service';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(withInterceptors([HttpRequestInterceptor])),
    provideRouter(routes),
    provideAnimations(),
    provideCharts(withDefaultRegisterables()),
    AuthService,
    HelperService
  ]
};
