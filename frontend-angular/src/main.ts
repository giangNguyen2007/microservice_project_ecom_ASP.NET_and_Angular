
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import {ConfigService} from './app/services/config.service';

export function initConfig( configService : ConfigService) {
  // Placeholder for any initialization logic if needed in the future
  return () => configService.loadConfig()
}


bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
