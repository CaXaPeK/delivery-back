using Delivery_Service.Schemas.Enums;

namespace Delivery_Service.Schemas.Classes
{
    public class SearchAddressModel
    {
        public long objectId {  get; set; }

        public Guid objectGuid { get; set; }

        public string text { get; set; }

        public GarAddressLevel objectLevel { get; set; }
        
        public string objectLevelText { get; set; }
    }
}
