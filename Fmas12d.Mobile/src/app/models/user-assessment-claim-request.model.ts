export class UserAssessmentClaimRequest {
  assessmentId: number;
  ownPatient: boolean;
  startPostcode: string;
  endPostcode: string;
  previousAssessmentId?: number;
  nextAssessmentId?: number;
}
