using System.Text.Json.Serialization;

namespace Delivery_Service.Schemas
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DishCategory
    {
        Wok,
        Pizza,
        Soup,
        Dessert,
        Drink
    }
}
