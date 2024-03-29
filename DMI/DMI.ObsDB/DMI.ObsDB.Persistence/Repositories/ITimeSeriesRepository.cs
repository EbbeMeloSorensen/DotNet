﻿using System;
using Craft.Persistence;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.Repositories
{
    public interface ITimeSeriesRepository : IRepository<TimeSeries>
    {
        TimeSeries Get(
            int id);

        TimeSeries GetIncludingObservations(
            int id);

        TimeSeries GetIncludingObservations(
            int id,
            DateTime startTime);

        TimeSeries GetIncludingObservations(
            int id,
            DateTime startTime,
            DateTime endTime);
    }
}
