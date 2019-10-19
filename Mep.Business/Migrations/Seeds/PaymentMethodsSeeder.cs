using Mep.Data.Entities;
using System.Linq;

namespace Mep.Business.Migrations.Seeds
{
  internal class PaymentMethodsSeeder : SeederBase<PaymentMethod>
  {
    internal void SeedData()
    {
      AddOrUpdate(
        CcgSeeder.NORTH_STAFFORDSHIRE,
        Models.PaymentMethodType.BACS,
        USER_DISPLAY_NAME_DOCTOR_FEMALE
      );

      AddOrUpdate(
        CcgSeeder.STOKE_ON_TRENT,
        Models.PaymentMethodType.CHEQUE,
        USER_DISPLAY_NAME_DOCTOR_MALE
      );      

    }

    private void AddOrUpdate(string ccgName, int paymentMethodTypeId, string userDisplayName)
    {
      PaymentMethod paymentMethod;

      if ((paymentMethod = _context.PaymentMethods
          .Where(p => p.CcgId == GetCcgByName(ccgName).Id)
          .Where(p => p.PaymentMethodTypeId == paymentMethodTypeId)
          .SingleOrDefault(g => g.UserId == GetUserByDisplayName(userDisplayName).Id)) == null)
      {
        paymentMethod = new PaymentMethod();
        _context.Add(paymentMethod);
      }
      paymentMethod.CcgId = GetCcgByName(ccgName).Id;
      paymentMethod.PaymentMethodTypeId = paymentMethodTypeId;
      paymentMethod.UserId = GetUserByDisplayName(userDisplayName).Id;
      PopulateActiveAndModifiedWithSystemUser(paymentMethod);      
    }
  }
}