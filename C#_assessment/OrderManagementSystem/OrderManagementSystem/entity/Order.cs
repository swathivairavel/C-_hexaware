
namespace OrderManagementSystem.entity
{
    public class Order
    {
        public int OrderId { get; set; }               
        public int UserId { get; set; }                
        public DateTime OrderDate { get; set; }   
        public List<OrderItem> Items { get; set; }     

        public Order() { }

        public Order(int orderId, int userId, DateTime orderDate, List<OrderItem> items)
        {
            OrderId = orderId;
            UserId = userId;
            OrderDate = orderDate;
            Items = items; // No null check — may stay null
        }
    }
}
