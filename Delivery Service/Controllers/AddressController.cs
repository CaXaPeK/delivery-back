using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Delivery_Service.Schemas.Enums;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("api/address/[action]")]
    public class AddressController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public AddressController(DeliveryDbContext context)
        {
            _context = context;
        }

        private GarAddressLevel GetLevelName(string level)
        {
            switch (level)
            {
                case "1":
                    return GarAddressLevel.Region;
                case "2":
                    return GarAddressLevel.AdministrativeArea;
                case "3":
                    return GarAddressLevel.MunicipalArea;
                case "4":
                    return GarAddressLevel.RuralUrbanSettlement;
                case "5":
                    return GarAddressLevel.City;
                case "6":
                    return GarAddressLevel.Locality;
                case "7":
                    return GarAddressLevel.ElementOfPlanningStructure;
                case "8":
                    return GarAddressLevel.ElementOfRoadNetwork;
                case "9":
                    return GarAddressLevel.Land;
                case "10":
                    return GarAddressLevel.Building;
                case "11":
                    return GarAddressLevel.Room;
                case "12":
                    return GarAddressLevel.RoomInRooms;
                case "13":
                    return GarAddressLevel.AutonomousRegionLevel;
                case "14":
                    return GarAddressLevel.IntracityLevel;
                case "15":
                    return GarAddressLevel.AdditionalTerritoriesLevel;
                case "16":
                    return GarAddressLevel.LevelOfObjectsInAdditionalTerritories;
                case "17":
                    return GarAddressLevel.CarPlace;
                default:
                    return GarAddressLevel.Region;
            }
        }

        private string GetLevelDescription(string level)
        {
            switch (level)
            {
                case "1":
                    return "Регион";
                case "2":
                    return "Административное образование";
                case "3":
                    return "Муниципальное образование";
                case "4":
                    return "Населённый пункт";
                case "5":
                    return "Город";
                case "6":
                    return "Район";
                case "7":
                    return "Элемент планировочной структуры";
                case "8":
                    return "Элемент улично-дорожной сети";
                case "9":
                    return "Земельный участок";
                case "10":
                    return "Здание (строение)";
                case "11":
                    return "Помещение в пределах здания";
                case "12":
                    return "Помещение в пределах помещения";
                case "13":
                    return "Автономная область";
                case "14":
                    return "Внутригородской";
                case "15":
                    return "Прочие территории";
                case "16":
                    return "Элемент прочих территорий";
                case "17":
                    return "Машино-место";
                default:
                    return "Регион";
            }
        }

        [HttpGet]
        public IActionResult search(long parentObjectId, string? query)
        {
            var matchedChildren = _context.AsHouses
                .Where(house => _context.AsAdmHierarchies.Any(hierarchy => hierarchy.Objectid == house.Objectid && hierarchy.Parentobjid == parentObjectId)).ToList();

            var results = new List<SearchAddressModel>();

            foreach (AsHouse house in matchedChildren)
            {
                string level = _context.AsAddrObjs.Where(x => x.Objectid == parentObjectId).Select(x => x.Level).First();
                string houseNum = house.Housenum == null ? "" : house.Housenum;
                string queryUnnulled = query == null ? "" : query;

                if (houseNum.Contains(queryUnnulled)) 
                {
                    var model = new SearchAddressModel
                    {
                        objectId = house.Objectid,
                        objectGuid = house.Objectguid,
                        text = houseNum,
                        objectLevel = GetLevelName(level),
                        objectLevelText = GetLevelDescription(level)
                    };

                    results.Add(model);
                }
            }

            return Ok(results);
        }

        private List<SearchAddressModel> GetChain(Guid objectGuid)
        {
            var matchedAddresses = _context.AsAddrObjs
                .Where(address => address.Objectguid == objectGuid).ToList();

            var results = new List<SearchAddressModel>();

            foreach (AsAddrObj address in matchedAddresses)
            {
                var model = new SearchAddressModel
                {
                    objectId = address.Objectid,
                    objectGuid = address.Objectguid,
                    text = address.Name,
                    objectLevel = GetLevelName(address.Level),
                    objectLevelText = GetLevelDescription(address.Level)
                };

                results.Add(model);
            }

            return results;
        }

        [HttpGet]
        public IActionResult chain(Guid objectGuid)
        {
            return Ok(GetChain(objectGuid));
        }

        [HttpGet]
        public IActionResult getaddresschain(Guid objectGuid)
        {
            return Ok(GetChain(objectGuid));
        }
    }
}
