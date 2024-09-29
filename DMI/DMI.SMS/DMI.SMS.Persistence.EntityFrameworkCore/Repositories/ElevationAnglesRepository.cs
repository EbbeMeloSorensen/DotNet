using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Repositories;

public class ElevationAnglesRepository : Repository<ElevationAngles>, IElevationAnglesRepository
{
    public ElevationAnglesRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(ElevationAngles entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<ElevationAngles> entities)
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

        if (!context.ElevationAngles.Any())
        {
            return 1;
        }

        return context.ElevationAngles.Max(_ => _.ObjectId) + 1;
    }

    public string GenerateUniqueGlobalId()
    {
        return Guid.NewGuid().ToString();
    }
}