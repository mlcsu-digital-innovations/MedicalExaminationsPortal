import { ClaimListComponent } from './claim-list/claim-list.component';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FinanceRoutes } from './finance.routes';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedComponentsModule } from '../shared-components.module';

@NgModule({
  declarations: [
    ClaimListComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(FinanceRoutes),
    SharedComponentsModule
  ],
  providers: [
    DecimalPipe
  ]
})
export class FinanceModule {}
