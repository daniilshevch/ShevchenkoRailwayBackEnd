public enum FoodType
{
    Food,
    Beverage
}
public class FoodItem
{
    public FoodType Type { get; set; }
    public double Price { get; set; }
}

public class FoodOrder
{
    public FoodItem Food { get; set; } = null!;
    public int Amount { get; set; }
    public double Total_Price { get => Food.Price * Amount; }
}

public class InternalTicketAdditionalServicesDto
{
    public bool Bedding { get; set; }
    public bool Luggage { get; set; }
    List<FoodOrder> Food_Orders { get; set; } = new List<FoodOrder>();

}

