using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mep.Data.Entities
{
  public class GenderType : NameDescription, IPaymentMethodType
  {
    [InverseProperty("PreferredDoctorGenderType")]
    public virtual IList<Examination> Examinations { get; set; }

    [InverseProperty("GenderType")]
    public virtual IList<User> Users { get; set; }
  }
}