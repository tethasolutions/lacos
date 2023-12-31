import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { arrayExtensions } from './app/services/extensions/array-extensions';
import { dateExtensions } from './app/services/extensions/date-extensions';
import { numberExtensions } from './app/services/extensions/number-extensions';
import { stringExtensions } from './app/services/extensions/string-extensions';
import { angularExtensions } from './app/services/extensions/angular-extensions';

arrayExtensions();
dateExtensions();
stringExtensions();
numberExtensions();
angularExtensions();

platformBrowserDynamic().bootstrapModule(AppModule)
    .catch(err => console.error(err));
