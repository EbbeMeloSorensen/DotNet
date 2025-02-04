﻿using Craft.Persistence.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.SMS.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Repositories;

public class StationInformationRepository : Repository<StationInformation>, IStationInformationRepository
{
    private SMSDbContextBase SMSDbContext => Context as SMSDbContextBase;

    public StationInformationRepository(
        DbContext context) : base(context)
    {
    }

    public StationInformation GetByGlobalId(
        string globalId)
    {
        var stationInformation = (Context as SMSDbContextBase).StationInformations
            .Single(_ => _.GlobalId == globalId && _.GdbToDate.Year == 9999);

        return stationInformation;
    }

    public override async Task Clear()
    {
        await Task.Run(() =>
        {
            var context = Context as SMSDbContextBase;

            context!.RemoveRange(context.StationInformations);
            context.SaveChanges();
        });
    }

    public override async Task Update(
        StationInformation stationInformation)
    {
        var sRepo = Get(stationInformation.GdbArchiveOid);

        sRepo.CopyAttributes(stationInformation);
    }

    public override async Task UpdateRange(
        IEnumerable<StationInformation> stationInformations)
    {
        var ids = stationInformations.Select(p => p.GdbArchiveOid);
        var stationInformationsFromRepository = await Find(s => ids.Contains(s.GdbArchiveOid));

        stationInformationsFromRepository.ToList().ForEach(sRepo =>
        {
            var updatedStationInformation = stationInformations.Single(sUpd => sUpd.GdbArchiveOid == sRepo.GdbArchiveOid);

            sRepo.CopyAttributes(updatedStationInformation);
        });
    }

    public void RemoveLogically(
        StationInformation entity, 
        DateTime transactionTime)
    {
        throw new NotImplementedException();
    }

    public void Supersede(
        StationInformation entity, 
        DateTime transactionTime, 
        string user)
    {
        throw new NotImplementedException();
    }

    public StationInformation Get(
        int gdbArchiveOId)
    {
        var stationInformation = (Context as SMSDbContextBase).StationInformations
            .Single(_ => _.GdbArchiveOid == gdbArchiveOId);

        return stationInformation;
    }

    public StationInformation GetWithContactPersons(
        int gdbArchiveOId)
    {
        return SMSDbContext.StationInformations
            .Include(_ => _.ContactPersons)
            .SingleOrDefault(_ => _.GdbArchiveOid == gdbArchiveOId) ?? throw new InvalidOperationException();
    }

    public int GenerateUniqueObjectId()
    {
        var context = Context as SMSDbContextBase;

        if (context == null)
        {
            throw new InvalidCastException();
        }

        if (!context.StationInformations.Any())
        {
            return 1;
        }

        return context.StationInformations.Max(_ => _.ObjectId) + 1;
    }

    public string GenerateUniqueGlobalId()
    {
        return Guid.NewGuid().ToString();
    }
}