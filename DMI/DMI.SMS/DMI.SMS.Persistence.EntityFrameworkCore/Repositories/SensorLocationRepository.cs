﻿using Craft.Persistence.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Repositories;

public class SensorLocationRepository : Repository<SensorLocation>, ISensorLocationRepository
{
    public SensorLocationRepository(DbContext context) : base(context)
    {
    }

    public SensorLocation Get(
        int gdbArchiveOId)
    {
        var sensorLocation = (Context as SMSDbContextBase).SensorLocations
            .Single(_ => _.GdbArchiveOid == gdbArchiveOId);

        return sensorLocation;
    }

    public SensorLocation GetByGlobalId(
        string globalId)
    {
        var sensorLocation = (Context as SMSDbContextBase).SensorLocations
            .Single(_ => _.GlobalId == globalId && _.GdbToDate.Year == 9999);

        return sensorLocation;
    }

    public override Task Clear()
    {
        throw new NotImplementedException();
    }

    public override Task Update(SensorLocation entity)
    {
        throw new NotImplementedException();
    }

    public override Task UpdateRange(IEnumerable<SensorLocation> entities)
    {
        throw new NotImplementedException();
    }

    public int GenerateUniqueObjectId()
    {
        var context = Context as SMSDbContextBase;

        if (context == null)
        {
            throw new InvalidCastException();
        }

        if (!context.SensorLocations.Any())
        {
            return 1;
        }

        return context.SensorLocations.Max(_ => _.ObjectId) + 1;
    }

    public string GenerateUniqueGlobalId()
    {
        return Guid.NewGuid().ToString();
    }
}