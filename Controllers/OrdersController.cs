using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrackersStore.API.Data;
using CrackersStore.API.Models;

namespace CrackersStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] int? userId = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/orders/track/{orderNumber}
        [HttpGet("track/{orderNumber}")]
        public async Task<ActionResult<Order>> TrackOrder(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderCreateDto orderDto)
        {
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = orderDto.UserId,
                Subtotal = orderDto.Subtotal,
                Tax = orderDto.Tax,
                Shipping = orderDto.Shipping,
                Total = orderDto.Total,
                Status = "Pending",
                PaymentMethod = orderDto.PaymentMethod,
                PaymentStatus = "Pending"
            };

            // Add shipping address
            var shippingAddress = new ShippingAddress
            {
                FirstName = orderDto.ShippingAddress.FirstName,
                LastName = orderDto.ShippingAddress.LastName,
                Email = orderDto.ShippingAddress.Email,
                Phone = orderDto.ShippingAddress.Phone,
                Address = orderDto.ShippingAddress.Address,
                City = orderDto.ShippingAddress.City,
                State = orderDto.ShippingAddress.State,
                Pincode = orderDto.ShippingAddress.Pincode,
                Landmark = orderDto.ShippingAddress.Landmark
            };

            _context.ShippingAddresses.Add(shippingAddress);
            await _context.SaveChangesAsync();

            order.ShippingAddress = shippingAddress;

            // Add order items
            foreach (var item in orderDto.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                order.OrderItems.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GenerateOrderNumber()
        {
            return $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }

    // DTOs
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public ShippingAddressDto ShippingAddress { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ShippingAddressDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string? Landmark { get; set; }
    }
}
