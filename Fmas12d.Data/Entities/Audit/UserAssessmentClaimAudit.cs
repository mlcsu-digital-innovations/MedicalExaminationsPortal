﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fmas12d.Data.Entities
{
  [Table("UserAssessmentClaimsAudit")]
  public partial class UserAssessmentClaimAudit : BaseAudit, IUserAssessmentClaim
  {
    public int? ClaimReference { get; set; }
    public int? ClaimStatusId { get; set; }
    [MaxLength(10)]
    [Required]
    public string EndPostcode { get; set; }
    public int AssessmentId { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? AssessmentPayment { get; set; }
    public bool IsAttendanceConfirmed { get; set; }
    public bool? IsClaimable { get; set; }
    public int? Mileage { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MileagePayment { get; set; }
    public DateTimeOffset? PaymentDate { get; set; }
    public int SelectedByUserId { get; set; }
    [MaxLength(10)]
    [Required]
    public string StartPostcode { get; set; }
    [MaxLength(2000)]
    [Required]
    public string TravelComments { get; set; }
    public int UserId { get; set; }
    public bool HasBeenDeallocated { get; set; }
    public int? NextAssessmentId { get; set; }
    public int? PreviousAssessmentId { get; set; }
  }
}
