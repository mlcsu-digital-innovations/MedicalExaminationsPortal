using System;

namespace Fmas12d.Business.Models
{
  public class AssessmentDoctor : BaseModel
  {
    public AssessmentDoctor() { }
    public AssessmentDoctor(Data.Entities.AssessmentDoctor entity) : base(entity)
    {
      if (entity == null) return;

      // TODO Assessment =
      AssessmentId = entity.AssessmentId;
      AttendanceConfirmedByUser = new User(entity.AttendanceConfirmedByUser);
      AttendanceConfirmedByUserId = entity.Id;
      ContactDetail = new ContactDetail(entity.ContactDetail);
      ContactDetailId = entity.ContactDetailId;
      Distance = null;
      DoctorUser = new User(entity.DoctorUser);
      DoctorUserId = entity.DoctorUserId;
      HasAccepted = entity.HasAccepted;
      Latitude = entity.Latitude;
      Longitude = entity.Longitude;
      Postcode = entity.Postcode;
      RespondedAt = entity.RespondedAt;
      // TODO Status =
      StatusId = entity.StatusId;
    }
    public virtual Assessment Assessment { get; set; }
    public int AssessmentId { get; set; }
    public virtual User AttendanceConfirmedByUser { get; set; }
    public int? AttendanceConfirmedByUserId { get; set; }
    public virtual ContactDetail ContactDetail { get; set; }
    public int? ContactDetailId { get; set; }
    public decimal? Distance { get; set; }
    public virtual User DoctorUser { get; set; }
    public int DoctorUserId { get; set; }
    public bool? HasAccepted { get; set; }
    public bool IsAvailable { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Postcode { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public virtual AssessmentDoctorStatus Status { get; set; }
    public int StatusId { get; set; }

    public Location KnownLocation
    {
      get
      {
        if (!Latitude.HasValue || !Longitude.HasValue)
        {
          return null;
        }
        else
        {
          return new Location()
          {
            ContactDetailId = ContactDetailId,
            ContactDetail = ContactDetail,
            Latitude = Latitude.Value,
            Longitude = Longitude.Value,
            Postcode = Postcode,
          };
        }
      }
    }
    public bool IsAllocated { get { return StatusId == AssessmentDoctorStatus.ALLOCATED; } }
    public bool IsSelected { get { return StatusId == AssessmentDoctorStatus.SELECTED; } }
  }
}