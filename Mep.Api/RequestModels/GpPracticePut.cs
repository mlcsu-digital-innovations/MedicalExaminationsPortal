using System.ComponentModel.DataAnnotations;

namespace Mep.Api.RequestModels
{
  public class GpPracticePut : GpPractice
  {
    [Required]
    public bool? IsActive { get; set; }
  }
}