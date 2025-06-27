

namespace OrderManagementSystem.exception
{

    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException() { }

        public OrderNotFoundException(string message) : base(message) { }
    }
}
