import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'discountPrice',
  standalone: true
})
export class DiscountPricePipe implements PipeTransform {
  transform(price: number | undefined | null, discount: number | undefined | null): number {
    if (price === undefined || price === null) return 0;
    if (!discount || discount <= 0) return price;
    const discounted = price * (1 - discount / 100);
    return Math.round(discounted * 100) / 100;
  }
}
