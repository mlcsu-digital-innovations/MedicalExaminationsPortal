using Mep.Data.Entities;
using System.Linq;

namespace Mep.Business.Migrations.Seeds
{
  internal class OnCallUsersSeeder : SeederBase
  {

    internal void SeedData()
    {
      OnCallUser onCallUser;

      if ((onCallUser = _context
        .OnCallUsers
          .SingleOrDefault(g => g.UserId ==
            GetUserByDisplayName(USER_DISPLAY_NAME_DOCTOR_MALE).Id))
              == null)
      {
        onCallUser = new OnCallUser();
        _context.Add(onCallUser);
      }
      onCallUser.DateTimeEnd = _now.AddHours(1);
      onCallUser.DateTimeStart = _now;
      onCallUser.IsActive = true;
      onCallUser.ModifiedAt = _now;
      onCallUser.ModifiedByUser = GetSystemAdminUser();
      onCallUser.UserId =
        GetUserByDisplayName(USER_DISPLAY_NAME_DOCTOR_MALE).Id;

      if ((onCallUser = _context
        .OnCallUsers
          .SingleOrDefault(g => g.UserId ==
            GetUserByDisplayName(USER_DISPLAY_NAME_DOCTOR_FEMALE).Id))
              == null)
      {
        onCallUser = new OnCallUser();
        _context.Add(onCallUser);
      }
      onCallUser.DateTimeEnd = _now.AddHours(1);
      onCallUser.DateTimeStart = _now;
      onCallUser.IsActive = true;
      onCallUser.ModifiedAt = _now;
      onCallUser.ModifiedByUser = GetSystemAdminUser();
      onCallUser.UserId =
        GetUserByDisplayName(USER_DISPLAY_NAME_DOCTOR_FEMALE).Id;
    }
  }
}