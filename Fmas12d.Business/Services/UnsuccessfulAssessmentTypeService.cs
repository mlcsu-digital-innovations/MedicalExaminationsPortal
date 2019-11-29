using Fmas12d.Business.Models;

namespace Fmas12d.Business.Services
{
  public class UnsuccessfulAssessmentTypeService : 
    NameDescriptionBaseService<Data.Entities.UnsuccessfulAssessmentType>,
    IUnsuccessfulAssessmentTypeService
  {
    public UnsuccessfulAssessmentTypeService(
      ApplicationContext context,
      IAppClaimsPrincipal appClaimsPrincipal)
      : base(context, appClaimsPrincipal)
    {
    }
  }
}