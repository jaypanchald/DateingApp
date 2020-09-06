/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { AlertyfyService } from './alertify.service';

describe('Service: Alertyfy', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AlertyfyService]
    });
  });

  it('should ...', inject([AlertyfyService], (service: AlertyfyService) => {
    expect(service).toBeTruthy();
  }));
});
