using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Mep.Business.Models;
using Entities = Mep.Data.Entities;
using Mep.Business.Extensions;
using Mep.Business.Models.SearchModels;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace Mep.Business.Services
{
  public class PatientService
    : SearchServiceBase<Patient, Entities.Patient, PatientSearchModel>, IModelSearchService<Patient, PatientSearchModel>
  {
    public PatientService(ApplicationContext context, IMapper mapper)
      : base("Patient", context, mapper)
    {
    }

    public override async Task<IEnumerable<Patient>> SearchAsync(PatientSearchModel searchModel)
    {
      // build up the where statement
      var param = Expression.Parameter(typeof(Entities.Patient), "p");

      Expression defaultExpression = Expression.GreaterThan(Expression.Property(param, "Id"), Expression.Constant(0));

      Expression searchExpression = GetSearchExpression(searchModel, param); 

      if (searchExpression != null)
      {
        defaultExpression = Expression.And(defaultExpression, searchExpression);
      }

      var whereExpression = Expression.Lambda<Func<Entities.Patient, bool>>(
        defaultExpression, param
      );

      IEnumerable<Entities.Patient> entities =
        await _context.Patients
        .Where(whereExpression)
        .WhereIsActiveOrActiveOnly(true)
        .ToListAsync();

      IEnumerable<Models.Patient> models =
        _mapper.Map<IEnumerable<Models.Patient>>(entities);

      return models;
    }

    public async Task<IEnumerable<Models.Patient>> GetAllAsync(
      bool activeOnly)
    {

      IEnumerable<Entities.Patient> entities =
        await _context.Patients
                .Include(p => p.Ccg)
                .Include(p => p.GpPractice)
                .WhereIsActiveOrActiveOnly(activeOnly)
                .ToListAsync();

      IEnumerable<Models.Patient> models =
        _mapper.Map<IEnumerable<Models.Patient>>(entities);

      return models;
    }

    protected override async Task<Entities.Patient> GetEntityByIdAsync(
      int entityId,
      bool asNoTracking,
      bool activeOnly)
    {
      Entities.Patient entity = await
        _context.Patients
                .Include(p => p.Ccg)
                .Include(p => p.GpPractice)
                .WhereIsActiveOrActiveOnly(activeOnly)
                .AsNoTracking(asNoTracking)
                .SingleOrDefaultAsync(patient => patient.Id == entityId);

      return entity;
    }

    protected override async Task<Entities.Patient> GetEntityLinkedObjectsAsync(Patient model, Entities.Patient entity)
    {
      if (model.CcgId.HasValue)
      {
        entity.Ccg = await GetLinkedObjectAsync<Entities.Ccg>(_context.Ccgs, model.CcgId.Value);
      }

      if (model.GpPracticeId.HasValue)
      {
        entity.GpPractice = await GetLinkedObjectAsync<Entities.GpPractice>(_context.GpPractices, model.GpPracticeId.Value);
      }

      return entity;
    }

    protected override Task<bool> InternalCreateAsync(Patient model, Entities.Patient entity)
    {
      return Task.FromResult<bool>(true);
    }

    protected override Task<bool> InternalUpdateAsync(Patient model, Entities.Patient entity)
    {
      return Task.FromResult<bool>(true);
    }
  }
}