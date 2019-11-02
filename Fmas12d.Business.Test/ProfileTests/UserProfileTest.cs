using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fmas12d.Business.Test.ProfileTests;
using Entities = Fmas12d.Data.Entities;
using System;
namespace Fmas12d.Business.Test
{
  [TestClass]
  public class UserProfileTest : GenericProfileTest<Business.Models.User, Entities.User>
  {
    private readonly String[] ignoredMappings = new string[18] 
    { 
      "BankDetails", 
      "CompletedAssessments", 
      "CompletionConfirmationAssessments", 
      "ContactDetails",
      "CreatedAssessments",
      "DoctorStatuses",
      "OnCallUsers",
      "Organisation",
      "PaymentMethods",
      "ProfileType",
      "Referrals",
      "Section12ApprovalStatus",
      "UserSpecialities",
      "UserAssessmentClaims",
      "UserAssessmentClaimSelections",
      "UserAssessmentNotifications",
      "GenderType",
      "AmhpReferrals"
    };
   
    [TestMethod]
    public void UserProfileBusiness2EntityIsValid()
    { 
      AssertBusiness2EntityMappingIsValid(ignoredMappings);
    }

    [TestMethod]
    public void UserProfileEntity2BusinessIsValid()
    {
      AssertEntity2BusinessMappingIsValid(ignoredMappings);
    }
  }
}