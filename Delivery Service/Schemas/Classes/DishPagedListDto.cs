namespace Delivery_Service.Schemas.Classes
{
    public class DishPagedListDto
    {
        public List<DishDto>? dishes { get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
