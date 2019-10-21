using System;
using System.Collections.Generic;
using System.Linq;
using Mep.Business.Models.Interfaces;

namespace Mep.Business.Models
{
  public class Referral : BaseModel
  {
    public DateTimeOffset CreatedAt { get; set; }
    public virtual User CreatedByUser { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual IList<Examination> Examinations { get; set; }
    public virtual Patient Patient { get; set; }
    public int PatientId { get; set; }
    public virtual ReferralStatus ReferralStatus { get; set; }
    public int ReferralStatusId { get; set; }
    public virtual User LeadAmhpUser { get; set; }
    public int LeadAmhpUserId { get; set; }
    public bool IsPlannedExamination { get; set; }

    public string PatientCcgName 
    {
      get
      {
        return Patient.Ccg?.Name;
      }
    }

    public int DoctorsAllocated
    {
      get
      {
        return Examinations?.Where(e => e.IsActive)
                            .FirstOrDefault(e => e.IsCurrent)
                            ?.UserExaminationClaims
                            .Where(uec => uec.IsActive)
                            .Count() ?? 0;
      }
    }

    public string ExaminationLocationPostcode
    {
      get
      {
        return Examinations?.Where(e => e.IsActive)
                            .FirstOrDefault(e => e.IsCurrent)
                            ?.Postcode;
      }
    }

    public string LeadAmhp
    {
      get
      {
        return LeadAmhpUser?.DisplayName;
      }
    }

    public int NumberOfExaminationAttempts
    {
      get
      {
        return Examinations?.Where(e => e.IsActive)
                            .Count(e => !e.IsSuccessful ?? false) ?? 0;
      }
    }

    public string PatientGpNameAndPostcode
    {
      get
      {
        return Patient.GpPracticeId != null ?
                $"{Patient.GpPractice.Name}, {Patient.GpPractice.Postcode}" :
                null;
      }
    }

    public string PatientIdentifier
    {
      get
      {
        return string.IsNullOrWhiteSpace(Patient?.NhsNumber.ToString())
               ? Patient?.AlternativeIdentifier
               : Patient.NhsNumber.ToString();
      }
    }

    public int ReferralId
    {
      get
      {
        return Id;
      }
    }
    public int ResponsesReceived
    {
      get
      {
        return Examinations?.Where(e => e.IsActive)
                            .FirstOrDefault(e => e.IsCurrent)
                            ?.UserExaminationNotifications
                            .Where(uen => uen.IsActive)
                            .Count(uen => uen.RespondedAt != null) ?? 0;
      }
    }
    public string Speciality
    {
      get
      {
        return Examinations?.Where(e => e.IsActive)
                            .FirstOrDefault(e => e.IsCurrent)
                            ?.Speciality
                            ?.Name;
      }
    }
    public string Status
    {
      get
      {
        return ReferralStatus?.Name;
      }
    }

    public DateTimeOffset? Timescale
    {
      get
      {
        DateTimeOffset? timescale = Examinations
                            ?.Where(e => e.IsActive)
                            .FirstOrDefault(e => e.IsCurrent)
                            ?.MustBeCompletedBy;

        return timescale == default(DateTimeOffset)
               ? null
               : timescale;
      }
    }

    public DateTimeOffset DefaultToBeCompletedBy
    {
      //ToDo: Get the examination offset hours from the application config
      get
      {
        return Examinations.Count > 0 ? DateTimeOffset.Now.AddHours(3) : CreatedAt.AddHours(3);
      }
    }
  }
}