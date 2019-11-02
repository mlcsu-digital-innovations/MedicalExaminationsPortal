import { AmhpAssessmentOutcomePage } from './amhp-assessment-outcome.page';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: AmhpAssessmentOutcomePage
  }
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    RouterModule.forChild(routes)
  ],
  declarations: [AmhpAssessmentOutcomePage]
})
export class AmhpAssessmentOutcomePageModule {}
