import { Injectable } from '@angular/core';
declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertyfyService {

  constructor() { }

  confirm(message: string, okCallBack: () => any) {
    alertify.confirm(message, function (e) {
      if (e) {
        okCallBack();
      } else { }
    });
  }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }

  info(message: string) {
    alertify.message(message);
  }

}
