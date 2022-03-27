using DD.Domain;
using DD.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DD.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatureTypesController : ControllerBase
    {
        private IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public CreatureTypesController(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet]
        public IEnumerable<CreatureType> Get()
        {
            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            return unitOfWork.CreatureTypes.GetAll();
        }
    }
}
