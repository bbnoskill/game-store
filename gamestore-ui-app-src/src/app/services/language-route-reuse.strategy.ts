import { Injectable, Injector, inject } from '@angular/core';
import { ActivatedRouteSnapshot, BaseRouteReuseStrategy, Router } from '@angular/router';

@Injectable()
export class LanguageRouteReuseStrategy extends BaseRouteReuseStrategy {
  private readonly injector = inject(Injector);

  override shouldReuseRoute(future: ActivatedRouteSnapshot, current: ActivatedRouteSnapshot): boolean {
    const info = this.injector.get(Router).getCurrentNavigation()?.extras.info as { languageChange?: boolean } | undefined;
    return !info?.languageChange && super.shouldReuseRoute(future, current);
  }
}
