using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Fmas12d.Business.Models
{
  public class ContactDetail : BaseModel
  {
    public ContactDetail() {}
    public ContactDetail(Data.Entities.ContactDetail entity)
    {
      if (entity == null) return;

      Address1 = entity.Address1;
      Address2 = entity.Address2;
      Address3 = entity.Address3;
      Ccg = new Ccg(entity.Ccg);
      CcgId = entity.CcgId;
      ContactDetailType = new ContactDetailType(entity.ContactDetailType);
      ContactDetailTypeId = entity.ContactDetailTypeId;
      EmailAddress = entity.EmailAddress;
      Latitude = entity.Latitude;
      Longitude = entity.Longitude;
      Id = entity.Id;
      Postcode = entity.Postcode;
      TelephoneNumber = entity.TelephoneNumber;
      Town = entity.Town;
      User = new User(entity.User);
      UserId = entity.UserId;
    }

    [MaxLength(200)]
    [Required]
    public string Address1 { get; set; }
    [MaxLength(200)]
    public string Address2 { get; set; }
    [MaxLength(200)]
    public string Address3 { get; set; }
    public virtual Ccg Ccg { get; set; }
    public int CcgId { get; set; }
    public virtual ContactDetailType ContactDetailType { get; set; }
    public int ContactDetailTypeId { get; set; }
    [MaxLength(100)]
    public string EmailAddress { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    [MaxLength(10)]
    public string Postcode { get; set; }
    public int? TelephoneNumber { get; set; }
    public string Town { get; set; }
    public virtual User User { get; set; }
    public int UserId { get; set; }

    public static Expression<Func<Data.Entities.ContactDetail, ContactDetail>> ProjectFromEntity
    {
      get
      {
        return entity => new ContactDetail(entity);
      }
    }

  }
}