using DD.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DD.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatureTypesController : ControllerBase
    {
        private static List<CreatureType> creatureTypes = new List<CreatureType>()
        {
            new CreatureType
            {
                Id = 1,
                Name = "Goblin",
                MaxHitPoints = 8,
                ArmorClass = 7,
                Movement = 6,
                Thaco = 16
            },
            new CreatureType
            {
                Id = 2,
                Name = "Skeleton",
                MaxHitPoints = 5,
                ArmorClass = 5,
                Movement = 7,
                Thaco = 14
            }
        };

        [HttpGet]
        public IEnumerable<CreatureType> Get()
        {
            return creatureTypes;
        }
    }
}
