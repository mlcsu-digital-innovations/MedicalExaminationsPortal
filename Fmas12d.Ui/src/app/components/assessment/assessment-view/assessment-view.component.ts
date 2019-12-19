import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of } from 'rxjs';
import { ParamMap, ActivatedRoute } from '@angular/router';
import { Referral } from 'src/app/interfaces/referral';
import { REFERRAL_STATUS_AWAITING_REVIEW, REFERRAL_STATUS_OPEN } from 'src/app/constants/Constants';
import { ReferralService } from 'src/app/services/referral/referral.service';
import { ReferralView } from 'src/app/interfaces/referral-view';
import { RouterService } from 'src/app/services/router/router.service';
import { switchMap, map, catchError } from 'rxjs/operators';
import { ToastService } from 'src/app/services/toast/toast.service';
import * as moment from 'moment';

@Component({
  selector: 'app-assessment-view',
  templateUrl: './assessment-view.component.html',
  styleUrls: ['./assessment-view.component.css']
})
export class AssessmentViewComponent implements OnInit {

  closeModal: NgbModalRef;
  currentAssessmentForm: FormGroup;
  isPatientIdValidated: boolean;
  pageSize: number;
  referral$: Observable<Referral | any>;
  referralCreated: Date;
  referralId: number;
  referralStatusId: number;

  @ViewChild('confirmClosure', null) closeTemplate;

  constructor(
    private formBuilder: FormBuilder,
    private modalService: NgbModal,
    private referralService: ReferralService,
    private route: ActivatedRoute,
    private routerService: RouterService,
    private toastService: ToastService,
  ) { }

  ngOnInit() {
    this.referral$ = this.route.paramMap.pipe(
      switchMap(
        (params: ParamMap) => {
          return this.referralService.getReferralView(+params.get('referralId'))
            .pipe(
              map(referral => {
                this.InitialiseForm(referral);
                return referral;
              })
            );
        }
      ),
      catchError((err) => {
        this.toastService.displayError({
          title: 'Error',
          message: 'Error Retrieving Referral Information'
        });

        const emptyReferral = {} as Referral;
        return of(emptyReferral);
      })
    );

    this.currentAssessmentForm = this.formBuilder.group({
      amhpUserName: [
        ''
      ],
      currentAssessment: [
        ''
      ],
      doctorNamesAccepted: [
        ''
      ],
      doctorNamesAllocated: [
        ''
      ],
      assessmentDetails: [
        ''
      ],
      fullAddress: [
        ''
      ],
      meetingArrangementComment: [
        ''
      ],
      mustBeCompletedBy: [
        ''
      ],
      postCode: [
        ''
      ],
      preferredDoctorGenderTypeName: [
        ''
      ],
      specialityName: [
        ''
      ],
    });
  }

  CancelView() {
    this.routerService.navigatePrevious();
  }

  CancelModal() {
    this.closeModal.close();
  }

  CloseReferral() {
    this.closeModal.close();

    let forceClose = false;

    if (this.referralStatusId !== REFERRAL_STATUS_AWAITING_REVIEW
        && this.referralStatusId !== REFERRAL_STATUS_OPEN ) {
          forceClose = true;
    }

    this.referralService.closeReferral(this.referralId, forceClose).subscribe(
      () => {
        this.toastService.displaySuccess({
          message: 'Referral Status Updated'
        });
        this.routerService.navigateByUrl('/referral/list');
      },
      error => {
        this.toastService.displayError({
          title: 'Server Error',
          message: 'Unable to update referral! Please try again in a few moments'
        });
      }
    );

  }

  CloseReferralConfirmation() {
    this.closeModal = this.modalService.open(this.closeTemplate, {
      size: 'lg'
    });
  }

  EditAssessment() {
    this.routerService.navigateByUrl(`/assessment/edit/${this.referralId}`);
  }

  InitialiseForm(referral: ReferralView) {
    this.currentAssessmentForm.controls.amhpUserName.setValue(referral.currentAssessment.amhpUser.displayName);
    this.currentAssessmentForm.controls.doctorNamesAccepted.setValue(referral.currentAssessment.doctorsSelected);
    this.currentAssessmentForm.controls.doctorNamesAllocated.setValue(referral.currentAssessment.doctorsAllocated);
    this.currentAssessmentForm.controls.fullAddress.setValue(referral.currentAssessment.fullAddress);
    this.currentAssessmentForm.controls.meetingArrangementComment.setValue(referral.currentAssessment.meetingArrangementComment);

    const mustBeCompletedBy = moment(referral.currentAssessment.mustBeCompletedBy).format('DD/MM/YYYY HH:mm');

    this.currentAssessmentForm.controls.mustBeCompletedBy.setValue(mustBeCompletedBy);
    this.currentAssessmentForm.controls.postCode.setValue(referral.currentAssessment.postcode);
    this.currentAssessmentForm.controls.preferredDoctorGenderTypeName.setValue(referral.currentAssessment.preferredDoctorGenderType.name);
    this.currentAssessmentForm.controls.specialityName.setValue(referral.currentAssessment.speciality.name);
    this.currentAssessmentForm.disable();
    this.referralId = referral.id;
    this.referralStatusId = referral.referralStatusId;
  }

  OnModalAction(event: any) {
    if (event) {
      this.CloseReferral();
    } else {
      this.CancelModal();
    }
  }
}
