namespace Delivery_Service.Schemas
{
    public class DishPagedListDto
    {
        List<DishDto>? dishes {  get; set; }

        public PageInfoModel pagination { get; set; }
    }
}
