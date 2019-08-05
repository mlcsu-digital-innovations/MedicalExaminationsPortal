﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mep.Data.Entities
{
  public interface IDoctorStatus
  {
    DateTimeOffset AvailabilityEnd { get; set; }
    DateTimeOffset AvailabilityStart { get; set; }
    DateTimeOffset? ExtendedAvailabilityEnd1 { get; set; }
    DateTimeOffset? ExtendedAvailabilityEnd2 { get; set; }
    DateTimeOffset? ExtendedAvailabilityEnd3 { get; set; }
    DateTimeOffset? ExtendedAvailabilityStart1 { get; set; }
    DateTimeOffset? ExtendedAvailabilityStart2 { get; set; }
    DateTimeOffset? ExtendedAvailabilityStart3 { get; set; }
    [Column(TypeName = "decimal(8,6)")]
    decimal Latitude { get; set; }
    [Column(TypeName = "decimal(9,6)")]
    decimal Longitude { get; set; }
  }
}