import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../../../environments/environment';

@Pipe({
  name: 'tax',
  standalone: true
})
export class TaxPipe implements PipeTransform {
  transform(price: number | undefined | null, rate: number = environment.taxRate): number {
    if (price === undefined || price === null) return 0;
    const taxAmount = price * (rate !== undefined ? rate : environment.taxRate);
    return Math.round(taxAmount * 100) / 100;
  }
}
