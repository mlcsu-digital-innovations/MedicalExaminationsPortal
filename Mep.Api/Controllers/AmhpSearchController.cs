using AutoMapper;
using Mep.Business.Services;
using Microsoft.AspNetCore.Mvc;
using BusinessModels = Mep.Business.Models;

namespace Mep.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AmhpSearchController : GeneralSearchController<BusinessModels.UserAmhp>
  {
    public AmhpSearchController(
      IModelGeneralSearchService<BusinessModels.UserAmhp> service,
      IMapper mapper)
      : base(service, mapper)
    {      
    }
  }
}