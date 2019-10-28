using System;
using BusinessModels = Mep.Business.Models.SearchModels;

namespace Mep.Api.SearchModels
{
  public class GeneralSearchResult
  {
    public int Id { get; set; }
    public string ResultText { get; set; }

    public static Func<BusinessModels.GeneralSearchResult, GeneralSearchResult> ProjectFromModel
    {
      get
      {
        return generalSearchResult => new GeneralSearchResult()
        {
          Id = generalSearchResult.Id,
          ResultText = generalSearchResult.ResultText
        };
      }
    } 
  }
}