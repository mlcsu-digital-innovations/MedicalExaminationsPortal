using Entities = Fmas12d.Data.Entities;
using Fmas12d.Business.Exceptions;
using Fmas12d.Business.Extensions;
using Fmas12d.Business.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fmas12d.Business.Services
{
  public class UserAvailabilityService :
    ServiceBase<Entities.UserAvailability>,
    IUserAvailabilityService
  {
    private readonly IContactDetailsService _contactDetailsService;
    private readonly ILocationDetailService _locationDetailService;
    public UserAvailabilityService(
      ApplicationContext context,
      IContactDetailsService contactDetailsService,
      ILocationDetailService locationDetailService,
      IUserClaimsService userClaimsService)
      : base(context, userClaimsService)
    {
      _contactDetailsService = contactDetailsService;
      _locationDetailService = locationDetailService;
    }

    /// <summary>
    /// TODO Check for overlapping availabilities
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IUserAvailability> CreateAsync(IUserAvailability model)
    {
      await SetLatitudeLongitudeAsync(model);
      await CheckForOverlappingAvailabilityAsync(model);

      Entities.UserAvailability entity = new Entities.UserAvailability();
      model.MapToEntity(entity);
      entity.IsActive = true;
      UpdateModified(entity);

      _context.Add(entity);
      await _context.SaveChangesAsync();

      model = await _context.UserAvailabilities
                      .Where(u => u.IsActive)
                      .Where(u => u.Id == entity.Id)
                      .Select(UserAvailability.ProjectFromEntity)
                      .SingleAsync();

      return model;
    }

    public async Task<IEnumerable<IUserAvailability>> GetAsync(
      int userId,
      DateTimeOffset from,
      bool asNoTracking,
      bool activeOnly)
    {
      IEnumerable<IUserAvailability> models = await _context
        .UserAvailabilities
        .Include(ua => ua.ContactDetail)
          .ThenInclude(cd => cd.ContactDetailType)
        .Where(ua => ua.UserId == userId)
        .Where(ua => ua.End >= from)
        .WhereIsActiveOrActiveOnly(activeOnly)
        .AsNoTracking(asNoTracking)
        .Select(UserAvailability.ProjectFromEntity)
        .ToListAsync();

      return models;
    }

    public async Task<IUserAvailability> GetAtAsync(
      int userId,
      DateTimeOffset at,
      bool asNoTracking,
      bool activeOnly)
    {
      IUserAvailability model = await _context
        .UserAvailabilities
        .Where(ua => ua.UserId == userId)
        .Where(ua => ua.End >= at)
        .Where(ua => ua.Start <= at)
        .WhereIsActiveOrActiveOnly(activeOnly)
        .AsNoTracking(asNoTracking)
        .Select(UserAvailability.ProjectFromEntity)
        .SingleOrDefaultAsync();

      return model;
    }    

    public async Task<IEnumerable<IUserAvailabilityDoctor>> GetAvailableDoctorsAsync(
      DateTimeOffset requiredDateTime,
      bool asNoTracking,
      bool activeOnly)
    {
      IEnumerable<IUserAvailabilityDoctor> models = await
      _context.UserAvailabilities
              .Include(u => u.User)
                .ThenInclude(u => u.DoctorAssessments)
              .Include(u => u.User)
                .ThenInclude(u => u.ProfileType)
              .Include(u => u.User)
                .ThenInclude(u => u.GenderType)
              .Include(u => u.User)
                .ThenInclude(u => u.UserSpecialities)
                  .ThenInclude(us => us.Speciality)
              .Where(u => u.Start <= requiredDateTime)
              .Where(u => u.End >= requiredDateTime)
              .Where(u => u.User.ProfileTypeId == ProfileType.DOCTOR)
              .Where(u => u.UserAvailabilityStatusId == UserAvailabilityStatus.AVAILABLE)
              .WhereIsActiveOrActiveOnly(activeOnly)
              .AsNoTracking(asNoTracking)
              .Select(entity => new UserAvailabilityDoctor()
              {
                ActiveAssessments = entity.User
                  .DoctorAssessments
                  .Where(da => da.IsActive)
                  .Where(da => da.Assessment.IsActive)
                  .Where(da => da.Assessment.IsSuccessful == null)
                  .Where(da => da.StatusId == AssessmentDoctorStatus.ALLOCATED)
                  .Where(da => da.Assessment.CompletedTime == null)
                  .Where(da => da.Assessment.ScheduledTime == null ||
                               da.Assessment.ScheduledTime <= requiredDateTime.AddHours(-8) ||
                               da.Assessment.ScheduledTime >= requiredDateTime.AddHours(8))
                  .Select(da => new Models.Assessment()
                  {
                    MustBeCompletedBy = da.Assessment.MustBeCompletedBy,
                    Postcode = da.Assessment.Postcode,
                    ScheduledTime = da.Assessment.ScheduledTime
                  })
                  .ToList(),

                End = entity.End,
                Location = new Location
                {
                  ContactDetailId = entity.ContactDetailId,
                  Latitude = entity.Latitude,
                  Longitude = entity.Longitude,
                  Postcode = entity.Postcode
                },
                GenderName = entity.User.GenderType.Name,
                Name = entity.User.DisplayName,
                SpecialityNames =
                  entity.User.UserSpecialities.Select(s => s.Speciality.Name).ToList(),
                Start = entity.Start,
                Type = entity.User.ProfileType.Name,
                UserId = entity.User.Id,
              })
              .ToListAsync();

      return models;
    }

    public async Task<Dictionary<int, Location>> GetDoctorsPostcodeAtAsync(
      List<int> userIds, DateTimeOffset dateTime, bool asNoTracking, bool activeOnly)
    {
      Dictionary<int, Location> doctorsPostcode =
        await _context.UserAvailabilities
                      .Where(ua => ua.Start <= dateTime)
                      .Where(ua => ua.End >= dateTime)
                      .Where(ua => ua.User.ProfileTypeId == ProfileType.DOCTOR)
                      .Where(ua => ua.UserAvailabilityStatusId == UserAvailabilityStatus.AVAILABLE)
                      .Where(ua => userIds.Any(id => id == ua.UserId))
                      .WhereIsActiveOrActiveOnly(activeOnly)
                      .AsNoTracking(asNoTracking)
                      .ToDictionaryAsync(
                        ua => ua.UserId,
                        ua => new Location()
                        {
                          Postcode = ua.Postcode,
                          Latitude = ua.Latitude,
                          Longitude = ua.Longitude
                        }
                      );

      return doctorsPostcode;
    }

    public async Task<IUserAvailability> UpdateAsync(IUserAvailability model)
    {
      await SetLatitudeLongitudeAsync(model);
      await CheckForOverlappingAvailabilityAsync(model, model.Id);

      Entities.UserAvailability entity = _context
        .UserAvailabilities
        .WhereIsActiveOrActiveOnly(true)
        .AsNoTracking(false)
        .SingleOrDefault(ua => ua.Id == model.Id);

      if (entity == null)
      {
        throw new ModelStateException("id",
          $"Unable to find a UserAvailability with an Id of {model.Id}");
      }

      model.MapToEntity(entity);
      UpdateModified(entity);

      await _context.SaveChangesAsync();

      model = await _context.UserAvailabilities
                      .Where(u => u.IsActive)
                      .Where(u => u.Id == entity.Id)
                      .Select(UserAvailability.ProjectFromEntity)
                      .SingleAsync();

      return model;
    }

    protected override void CheckUserCanSetActiveStatus(
      Entities.UserAvailability entity,
      int userId
    )
    {
      if (entity == null)
      {
        throw new Exception("Unable to CheckUserCanSetActiveStatus because the entity is null");
      }
      if (entity.UserId != userId)
      {
        throw new UnauthorizedAccessException(
          $"User Id {userId} cannot update the active status of the UserAvailability Id " +
          $"{entity.Id} because its associated with User Id {entity.UserId}."
        );
      }
    }

    private async Task<bool> CheckForOverlappingAvailabilityAsync(
      IUserAvailability model,
      int? currentUserAvailabilityId
    )
    {
      IQueryable<Entities.UserAvailability> query =
        _context.UserAvailabilities
                .Where(u => model.UserId == u.UserId)
                .Where(u => model.Start <= u.End)
                .Where(u => model.End >= u.Start)
                .WhereIsActiveOrActiveOnly(true)
                .AsNoTracking(true);

      if (currentUserAvailabilityId.HasValue)
      {
        query = query.Where(ua => ua.Id != currentUserAvailabilityId.Value);
      }

      IEnumerable<int> existingAvailabilitiesIds = await query
        .Select(u => u.Id)
        .ToListAsync();

      if (existingAvailabilitiesIds.Count() > 0)
      {
        throw new ModelStateException(new string[] { "Start", "End" },
          $"There are existing availability records for the User with an id of {model.UserId} " +
          $"between {model.Start} and {model.End} " +
          $"UserAvailabilities Ids [{string.Join(",", existingAvailabilitiesIds)}]");
      }
      return true;
    }

    private async Task<bool> CheckForOverlappingAvailabilityAsync(IUserAvailability model)
    {
      return await CheckForOverlappingAvailabilityAsync(model, null);
    }

    /// <summary>
    /// Gets the latitude and longitude from either the postcode or contact details
    /// </summary>
    private async Task<bool> SetLatitudeLongitudeAsync(IUserAvailability model)
    {
      if (model.Location.HasPostcode)
      {
        model.Location =
          await _locationDetailService.GetPostcodeDetailsAsync(model.Location.Postcode);
      }
      else if (model.Location.HasContactDetailId)
      {
        ContactDetail contactDetail = await _contactDetailsService.GetByIdAndUserIdAsync(
          model.Location.ContactDetailId.Value, model.UserId, true, true
        );
        if (contactDetail == null)
        {
          throw new ModelStateException("ContactDetailId",
            $"Unable to find a ContactDetail Id of {model.Location.ContactDetailId} for User Id " +
            $"{model.UserId}.");
        }
        if (contactDetail.UserId != model.UserId)
        {
          throw new ModelStateException("ContactDetailId",
            $"A User with an id of {model.UserId} does not have an active " +
            $"ContactDetailId of {model.Location.ContactDetailId}");
        }
        model.Location.ContactDetail = contactDetail;
        model.Location.Latitude = contactDetail.Latitude;
        model.Location.Longitude = contactDetail.Longitude;
      }

      return true;
    }

  }
}