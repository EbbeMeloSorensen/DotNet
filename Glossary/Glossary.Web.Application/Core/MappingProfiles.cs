using AutoMapper;
using Glossary.Domain.Entities;
using Glossary.Web.Application.Records;

namespace Glossary.Web.Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Record, Record>();
            CreateMap<Record, RecordDto>();
        }
    }
}
