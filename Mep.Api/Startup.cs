﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Mep.Business;
using Mep.Business.Models;
using Mep.Business.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Mep.Api
{
  public class Startup
  {
    private const string ENV_AZURE_DEVELOPMENT = "AzureDevelopment";

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(o =>
      {
        o.Authority = "https://login.microsoftonline.com/df7baf74-a29e-4c5e-abee-0f073b7a5b91/v2.0";
        o.Audience = "c898ea46-4e6e-4e55-b53b-8ae61c825507";
        o.RequireHttpsMetadata = false;
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
          o.SaveToken = true;
        };
      });

      services.AddAuthorization(
        options =>
      {
        options.AddPolicy("Admin", policy => 
          policy.RequireClaim("JobTitle", "Admin", "SystemAdmin"));        

        options.AddPolicy("AMHP", policy => 
        policy.RequireClaim("JobTitle", "Admin", "AMHP", "SystemAdmin"));

        options.AddPolicy("Doctor", policy => 
          policy.RequireClaim("JobTitle", "Admin", "Doctor", "SystemAdmin"));

        options.AddPolicy("Finance", policy => 
          policy.RequireClaim("JobTitle", "Admin", "Finance", "SystemAdmin"));

        options.AddPolicy("SystemAdmin", policy => 
          policy.RequireClaim("JobTitle", "SystemAdmin"));

        options.AddPolicy("User", policy => 
          policy.RequireClaim("JobTitle", "Admin", "AMHP", "Doctor", "Finance", "SystemAdmin"));
      });

      services.AddMvc()
              .AddNewtonsoftJson(options =>
              {
                options.SerializerSettings.PreserveReferencesHandling =
                  PreserveReferencesHandling.Objects;
              });

      // log all api bad requests
      services.PostConfigure<ApiBehaviorOptions>(options =>
      {
        var builtInFactory = options.InvalidModelStateResponseFactory;
        options.InvalidModelStateResponseFactory = context =>
        {
          Serilog.Log.Warning(
            "Bad Request {ActionName}: {ModelStateErrors}",
            context.ActionDescriptor.DisplayName,
            context.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
          return builtInFactory(context);
        };
      });

      services.AddDbContext<ApplicationContext>
      (options =>
      {
        options.UseSqlServer(Configuration.GetConnectionString("MedicalExaminationsPortal"),
                             // https://docs.microsoft.com/en-us/azure/architecture/best-practices/retry-service-specific#sql-database-using-entity-framework-core
                             opt => opt.EnableRetryOnFailure());

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
          options.EnableSensitiveDataLogging();
          options.EnableDetailedErrors();
        }
      });

      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

      services.AddScoped<ICcgService, CcgService>();
      services.AddScoped<IModelService<DoctorStatus>, DoctorStatusService>();
      services.AddScoped<IModelService<Examination>, ExaminationService>();
      services.AddScoped<IExaminationDetailTypeService, ExaminationDetailTypeService>();
      services.AddScoped<IGenderTypeService, GenderTypeService>();
      services.AddScoped<IGpPracticeService, GpPracticeService>();
      services.AddScoped<IPatientService, PatientService>();
      services.AddScoped<IReferralService, ReferralService>();
      services.AddScoped<IModelService<ReferralStatus>, ReferralStatusService>();
      services.AddScoped<ISpecialityService, SpecialityService>();
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<IUnsuccessfulExaminationTypeService, UnsuccessfulExaminationTypeService>();

      services.AddScoped<IModelSimpleSearchService<AvailableDoctor, Business.Models.SearchModels.AvailableDoctorSearch>, AvailableDoctorService>();

      services.AddScoped<IModelGeneralSearchService<Ccg>, CcgSearchService>();
      services.AddScoped<IModelGeneralSearchService<GpPractice>, GpPracticeSearchService>();
      services.AddScoped<IModelGeneralSearchService<UserAmhp>, UserAmhpSearchService>();
      services.AddScoped<IModelGeneralSearchService<UserDoctor>, UserDoctorSearchService>();

      services.AddCors(options =>
      {
        options.AddPolicy("AllowAnyOrigin",
          builder =>
          {
            builder.AllowAnyOrigin();
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
          }
        );
      });
    }

    // This method gets called by the runtime. 
    // Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment() || env.IsEnvironment(ENV_AZURE_DEVELOPMENT))
      {
        app.UseDeveloperExceptionPage();
        IdentityModelEventSource.ShowPII = true;
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, 
        // see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseExceptionHandler("/Error");
      app.UseSerilogRequestLogging();
      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseCors("AllowAnyOrigin");
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
