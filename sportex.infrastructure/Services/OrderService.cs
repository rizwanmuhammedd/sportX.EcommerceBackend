


//    using Microsoft.EntityFrameworkCore;
//    using Sportex.Application.DTOs.Orders;
//    using Sportex.Application.Interfaces;
//    using Sportex.Domain.Entities;
//    using Sportex.Domain.Enums;
//    using Sportex.Infrastructure.Data;
//using System.Linq;

//    namespace Sportex.Infrastructure.Services;

//    public class OrderService : IOrderService
//    {
//        private readonly SportexDbContext _context;
//        public OrderService(SportexDbContext context) => _context = context;

//        // CART CHECKOUT
//        public async Task<int> PlaceOrderAsync(int userId, CreateCartOrderDto dto)
//        {
//            var cartItems = await _context.CartItems
//                .Include(c => c.Product)
//                .Where(x => x.UserId == userId)
//                .ToListAsync();

//            if (!cartItems.Any())
//                throw new Exception("Cart is empty");

//            using var trx = await _context.Database.BeginTransactionAsync();

//            decimal total = 0;
//        var order = new Order
//        {
//            UserId = userId,
//            ShippingAddress = $"{dto.ShippingAddress.FullName}, " +
//                  $"{dto.ShippingAddress.AddressLine}, " +
//                  $"{dto.ShippingAddress.City}, " +
//                  $"{dto.ShippingAddress.State} - {dto.ShippingAddress.Pincode}",
//            PaymentMode = dto.PaymentMode ?? "ONLINE",
//            Status = OrderStatus.Pending,
//            OrderDate = DateTime.UtcNow,
//            IsPaid = dto.PaymentMode == "COD"
//        };

//        // 🔥 ALSO SAVE ADDRESS IN ShippingAddresses TABLE
//        var shippingAddress = new ShippingAddress
//        {
//            UserId = userId,
//            FullName = dto.ShippingAddress.FullName,
//            Phone = dto.ShippingAddress.Phone,
//            AltPhone = dto.ShippingAddress.AltPhone,
//            AddressLine = dto.ShippingAddress.AddressLine,
//            Landmark = dto.ShippingAddress.Landmark,
//            City = dto.ShippingAddress.City,
//            State = dto.ShippingAddress.State,
//            Pincode = dto.ShippingAddress.Pincode,
//            IsDefault = true   // make this the default address
//        };

//        _context.ShippingAddresses.Add(shippingAddress);


//        _context.Orders.Add(order);
//            await _context.SaveChangesAsync();

//        //foreach (var item in cartItems)
//        //{
//        //    if (item.Quantity > item.Product.StockQuantity)
//        //        throw new Exception($"Not enough stock for {item.Product.Name}");



//        //    var oi = new OrderItem
//        //    {
//        //        OrderId = order.Id,
//        //        ProductId = item.ProductId,
//        //        Quantity = item.Quantity,
//        //        UnitPrice = item.Product.Price
//        //    };

//        //    total += item.Quantity * item.Product.Price;
//        //    _context.OrderItems.Add(oi);
//        //}



//        foreach (var item in cartItems)
//        {
//            if (item.Quantity > item.Product.StockQuantity)
//                throw new Exception($"Not enough stock for {item.Product.Name}");

//            // 🔥 REDUCE STOCK FOR COD
//            if (dto.PaymentMode == "COD")
//            {
//                item.Product.StockQuantity -= item.Quantity;
//            }

//            var oi = new OrderItem
//            {
//                OrderId = order.Id,
//                ProductId = item.ProductId,
//                Quantity = item.Quantity,
//                UnitPrice = item.Product.Price
//            };

//            total += item.Quantity * item.Product.Price;
//            _context.OrderItems.Add(oi);
//        }

//        order.TotalAmount = total;
//            _context.CartItems.RemoveRange(cartItems);

//            await _context.SaveChangesAsync();
//            await trx.CommitAsync();
//            return order.Id;
//        }

//        // BUY NOW
//        public async Task<int> PlaceDirectOrderAsync(int userId, CreateDirectOrderDto dto)
//        {
//            if (!dto.Items.Any())
//                throw new Exception("No items selected");

//            using var trx = await _context.Database.BeginTransactionAsync();

//        var order = new Order
//        {
//            UserId = userId,
//            ShippingAddress = $"{dto.ShippingAddress.FullName}, " +
//                  $"{dto.ShippingAddress.AddressLine}, " +
//                  $"{dto.ShippingAddress.City}, " +
//                  $"{dto.ShippingAddress.State} - {dto.ShippingAddress.Pincode}",
//            PaymentMode = dto.PaymentMode ?? "ONLINE",
//            Status = OrderStatus.Pending,
//            IsPaid = dto.PaymentMode == "COD",
//            OrderDate = DateTime.UtcNow
//        };
//        // 🔥 ALSO SAVE ADDRESS IN ShippingAddresses TABLE
//        var shippingAddress = new ShippingAddress
//        {
//            UserId = userId,
//            FullName = dto.ShippingAddress.FullName,
//            Phone = dto.ShippingAddress.Phone,
//            AltPhone = dto.ShippingAddress.AltPhone,
//            AddressLine = dto.ShippingAddress.AddressLine,
//            Landmark = dto.ShippingAddress.Landmark,
//            City = dto.ShippingAddress.City,
//            State = dto.ShippingAddress.State,
//            Pincode = dto.ShippingAddress.Pincode,
//            IsDefault = true   // make this the default address
//        };

//        _context.ShippingAddresses.Add(shippingAddress);





//        _context.Orders.Add(order);
//            await _context.SaveChangesAsync();

//            decimal total = 0;

//            foreach (var item in dto.Items)
//            {
//                var product = await _context.Products.FindAsync(item.ProductId);
//                if (product == null) throw new Exception("Product not found");

//                if (item.Quantity < 1 || item.Quantity > product.StockQuantity)
//                    throw new Exception($"Invalid quantity for {product.Name}");

//                var oi = new OrderItem
//                {
//                    OrderId = order.Id,
//                    ProductId = item.ProductId,
//                    Quantity = item.Quantity,
//                    UnitPrice = product.Price
//                };

//                total += product.Price * item.Quantity;
//                _context.OrderItems.Add(oi);
//            }


//            order.TotalAmount = total;

//            await _context.SaveChangesAsync();
//            await trx.CommitAsync();
//            return order.Id;
//        }

//        // MY ORDERS
//        public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
//        {
//        return await _context.Orders
// .Where(o => o.UserId == userId)
// .OrderByDescending(o => o.OrderDate)   // 🔥 THIS LINE FIXES EVERYTHING
// .Include(o => o.Items).ThenInclude(i => i.Product)
// .Select(o => new OrderDto
// {
//     Id = o.Id,
//     TotalAmount = o.TotalAmount,
//     ShippingAddress = o.ShippingAddress,
//     Status = o.Status.ToString(),
//     IsPaid = o.IsPaid,
//     OrderDate = o.OrderDate,
//     Items = o.Items.Select(i => new OrderItemDto
//     {
//         ProductId = i.ProductId,
//         ProductName = i.Product.Name,
//         ImageUrl = i.Product.ImageUrl,
//         Quantity = i.Quantity,
//         UnitPrice = i.UnitPrice
//     }).ToList()
// }).ToListAsync();

//    }

//    public async Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId)
//        {
//            return await _context.Orders
//                .Where(o => o.Id == orderId && o.UserId == userId)
//                .Include(o => o.Items).ThenInclude(i => i.Product)
//                .Select(o => new OrderDto
//                {
//                    Id = o.Id,
//                    TotalAmount = o.TotalAmount,
//                    ShippingAddress = o.ShippingAddress,
//                    Status = o.Status.ToString(),

//                    IsPaid = o.IsPaid,
//                    OrderDate = o.OrderDate,
//                    Items = o.Items.Select(i => new OrderItemDto
//                    {
//                        ProductId = i.ProductId,
//                        ProductName = i.Product.Name,
//                        ImageUrl = i.Product.ImageUrl,
//                        Quantity = i.Quantity,
//                        UnitPrice = i.UnitPrice
//                    }).ToList()
//                }).FirstOrDefaultAsync();
//        }

//        public async Task PayAsync(int userId, int orderId)
//        {
//            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
//            if (order == null) throw new Exception("Order not found");

//            order.IsPaid = true;
//            await _context.SaveChangesAsync();
//        }

//        public async Task ToggleStatusAsync(int orderId)
//        {
//            var order = await _context.Orders.FindAsync(orderId);
//            if (order == null) throw new Exception("Order not found");

//            order.Status = order.Status == OrderStatus.Pending ? OrderStatus.Shipped : OrderStatus.Pending;
//            await _context.SaveChangesAsync();
//        }




//    public async Task<List<OrderDto>> GetAllOrdersAsync()
//    {
//        return await _context.Orders
//            .OrderByDescending(o => o.OrderDate)

//            .Include(o => o.Items).ThenInclude(i => i.Product)
//            .Include(o => o.User)
//            .Select(o => new OrderDto
//            {
//                Id = o.Id,
//                UserId = o.UserId,

//                // 🔥 NULL-SAFE (prevents 500)
//                UserName = o.User != null
//                    ? (o.User.Name ?? $"User {o.UserId}")
//                    : $"User {o.UserId}",

//                UserEmail = o.User != null
//                    ? o.User.Email
//                    : "No email",

//                TotalAmount = o.TotalAmount,
//                Status = o.Status.ToString(),
//                OrderDate = o.OrderDate,
//                IsPaid = o.IsPaid,

//                ShippingAddress = o.ShippingAddress,

//                Items = o.Items.Select(i => new OrderItemDto
//                {
//                    ProductId = i.ProductId,
//                    ProductName = i.Product.Name,
//                    ImageUrl = i.Product.ImageUrl,
//                    Quantity = i.Quantity,
//                    UnitPrice = i.UnitPrice
//                }).ToList()
//            })
//            .ToListAsync();
//    }




//    // USER CANCEL ORDER
//    public async Task<OrderDto> CancelOrderAsync(int userId, int orderId)
//        {
//            var order = await _context.Orders
//                .Include(o => o.Items)
//                .ThenInclude(i => i.Product)
//                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

//            if (order == null)
//                throw new Exception("Order not found");

//            if (order.Status != OrderStatus.Pending)
//                throw new Exception("Only pending orders can be cancelled");

//            foreach (var item in order.Items)
//            {
//                var product = await _context.Products.FindAsync(item.ProductId);
//                if (product != null)
//                    product.StockQuantity += item.Quantity;
//            }

//            order.Status = OrderStatus.Cancelled;
//            await _context.SaveChangesAsync();

//            return new OrderDto
//            {
//                Id = order.Id,
//                TotalAmount = order.TotalAmount,
//                ShippingAddress = order.ShippingAddress,
//                Status = order.Status.ToString(),
//                IsPaid = order.IsPaid,
//                OrderDate = order.OrderDate,
//                Items = order.Items.Select(i => new OrderItemDto
//                {
//                    ProductId = i.ProductId,
//                    ProductName = i.Product.Name,
//                    ImageUrl = i.Product.ImageUrl,
//                    Quantity = i.Quantity,
//                    UnitPrice = i.UnitPrice
//                }).ToList()
//            };


//        }


//        // ---------------- ADMIN ORDER STATUS ----------------

//        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
//        {
//            var order = await _context.Orders.FindAsync(orderId)
//                ?? throw new Exception("Order not found");

//            order.Status = status;
//            await _context.SaveChangesAsync();
//        }

//        public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
//        {
//            return await _context.Orders
//                .Where(o => o.Status == status)
//                .Include(o => o.Items).ThenInclude(i => i.Product)
//                .Select(o => new OrderDto
//                {
//                    Id = o.Id,
//                    TotalAmount = o.TotalAmount,
//                    ShippingAddress = o.ShippingAddress,
//                    Status = status.ToString(),

//                    IsPaid = o.IsPaid,
//                    OrderDate = o.OrderDate,
//                    Items = o.Items.Select(i => new OrderItemDto
//                    {
//                        ProductId = i.ProductId,
//                        ProductName = i.Product.Name,
//                        ImageUrl = i.Product.ImageUrl,
//                        Quantity = i.Quantity,
//                        UnitPrice = i.UnitPrice
//                    }).ToList()
//                }).ToListAsync();
//        }
//    public async Task ConfirmPaymentAsync(int orderId)
//    {
//        var order = await _context.Orders
//            .Include(o => o.Items)
//            .ThenInclude(i => i.Product)
//            .FirstAsync(o => o.Id == orderId);

//        if (order.IsPaid) return;

//        foreach (var item in order.Items)
//        {
//            if (item.Product.StockQuantity < item.Quantity)
//                throw new Exception($"Out of stock: {item.Product.Name}");

//            item.Product.StockQuantity -= item.Quantity;
//        }

//        order.IsPaid = true;

//        // ✅ DO NOT OVERRIDE ADMIN STATUS
//        if (order.Status == OrderStatus.Pending)
//        {
//            order.Status = OrderStatus.Processing;
//        }

//        await _context.SaveChangesAsync();
//    }



//}


using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Orders;
using Sportex.Application.DTOs.Shipping; // Add this
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Domain.Enums;
using Sportex.Infrastructure.Data;
using System.Linq;

namespace Sportex.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly SportexDbContext _context;
    public OrderService(SportexDbContext context) => _context = context;

    // CART CHECKOUT
    public async Task<int> PlaceOrderAsync(int userId, CreateCartOrderDto dto)
    {
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            throw new Exception("Cart is empty");

        using var trx = await _context.Database.BeginTransactionAsync();

        decimal total = 0;

        // 🔥 1) CREATE SHIPPING ADDRESS FIRST
        var shippingAddress = new ShippingAddress
        {
            UserId = userId,
            FullName = dto.ShippingAddress.FullName,
            Phone = dto.ShippingAddress.Phone,
            AltPhone = dto.ShippingAddress.AltPhone,
            AddressLine = dto.ShippingAddress.AddressLine,
            Landmark = dto.ShippingAddress.Landmark,
            City = dto.ShippingAddress.City,
            State = dto.ShippingAddress.State,
            Pincode = dto.ShippingAddress.Pincode,
            IsDefault = true
        };

        _context.ShippingAddresses.Add(shippingAddress);
        await _context.SaveChangesAsync(); // 🔥 Save to get ID

        // 🔥 2) CREATE SNAPSHOT FROM SAVED ADDRESS
        var snapshot = $"{shippingAddress.FullName}, " +
                       $"{shippingAddress.AddressLine}, " +
                       $"{shippingAddress.City}, " +
                       $"{shippingAddress.State} - {shippingAddress.Pincode}";

        // 🔥 3) CREATE ORDER WITH BOTH IDs
        var order = new Order
        {
            UserId = userId,
            ShippingAddressId = shippingAddress.Id,
            ShippingSnapshot = snapshot,
            PaymentMode = dto.PaymentMode ?? "ONLINE",
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            IsPaid = dto.PaymentMode == "COD"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            if (item.Quantity > item.Product.StockQuantity)
                throw new Exception($"Not enough stock for {item.Product.Name}");

            if (dto.PaymentMode == "COD")
            {
                item.Product.StockQuantity -= item.Quantity;
            }

            var oi = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            };

            total += item.Quantity * item.Product.Price;
            _context.OrderItems.Add(oi);
        }

        order.TotalAmount = total;
        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();
        await trx.CommitAsync();
        return order.Id;
    }

    // BUY NOW
    public async Task<int> PlaceDirectOrderAsync(int userId, CreateDirectOrderDto dto)
    {
        if (!dto.Items.Any())
            throw new Exception("No items selected");

        using var trx = await _context.Database.BeginTransactionAsync();

        decimal total = 0;

        // 🔥 1) CREATE SHIPPING ADDRESS FIRST
        var shippingAddress = new ShippingAddress
        {
            UserId = userId,
            FullName = dto.ShippingAddress.FullName,
            Phone = dto.ShippingAddress.Phone,
            AltPhone = dto.ShippingAddress.AltPhone,
            AddressLine = dto.ShippingAddress.AddressLine,
            Landmark = dto.ShippingAddress.Landmark,
            City = dto.ShippingAddress.City,
            State = dto.ShippingAddress.State,
            Pincode = dto.ShippingAddress.Pincode,
            IsDefault = true
        };

        _context.ShippingAddresses.Add(shippingAddress);
        await _context.SaveChangesAsync();

        // 🔥 2) CREATE SNAPSHOT FROM SAVED ADDRESS
        var snapshot = $"{shippingAddress.FullName}, " +
                       $"{shippingAddress.AddressLine}, " +
                       $"{shippingAddress.City}, " +
                       $"{shippingAddress.State} - {shippingAddress.Pincode}";

        // 🔥 3) CREATE ORDER WITH BOTH IDs
        var order = new Order
        {
            UserId = userId,
            ShippingAddressId = shippingAddress.Id,
            ShippingSnapshot = snapshot,
            PaymentMode = dto.PaymentMode ?? "ONLINE",
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            IsPaid = dto.PaymentMode == "COD"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null) throw new Exception("Product not found");

            if (item.Quantity < 1 || item.Quantity > product.StockQuantity)
                throw new Exception($"Invalid quantity for {product.Name}");

            if (dto.PaymentMode == "COD")
            {
                product.StockQuantity -= item.Quantity;
            }

            var oi = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            total += product.Price * item.Quantity;
            _context.OrderItems.Add(oi);
        }

        order.TotalAmount = total;

        await _context.SaveChangesAsync();
        await trx.CommitAsync();
        return order.Id;
    }

    // MY ORDERS - Regular users get snapshot only
    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingSnapshot ?? string.Empty,
                Status = o.Status.ToString(),
                IsPaid = o.IsPaid,
                OrderDate = o.OrderDate,
                PaymentMode = o.PaymentMode,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId && o.UserId == userId)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingSnapshot ?? string.Empty,
                Status = o.Status.ToString(),
                IsPaid = o.IsPaid,
                OrderDate = o.OrderDate,
                PaymentMode = o.PaymentMode,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task PayAsync(int userId, int orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        if (order == null) throw new Exception("Order not found");

        order.IsPaid = true;
        await _context.SaveChangesAsync();
    }

    public async Task ToggleStatusAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) throw new Exception("Order not found");

        order.Status = order.Status == OrderStatus.Pending ? OrderStatus.Shipped : OrderStatus.Pending;
        await _context.SaveChangesAsync();
    }

    // ADMIN - Get all orders with full address details
    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .Include(o => o.ShippingAddress)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserName = o.User != null ? (o.User.Name ?? $"User {o.UserId}") : $"User {o.UserId}",
                UserEmail = o.User != null ? o.User.Email : "No email",
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                OrderDate = o.OrderDate,
                IsPaid = o.IsPaid,
                PaymentMode = o.PaymentMode,
                ShippingAddress = o.ShippingSnapshot ?? string.Empty,
                ShippingAddressDetails = o.ShippingAddress != null ? new ShippingAddressDto
                {
                    Id = o.ShippingAddress.Id,
                    FullName = o.ShippingAddress.FullName,
                    Phone = o.ShippingAddress.Phone,
                    AltPhone = o.ShippingAddress.AltPhone,
                    AddressLine = o.ShippingAddress.AddressLine,
                    Landmark = o.ShippingAddress.Landmark,
                    City = o.ShippingAddress.City,
                    State = o.ShippingAddress.State,
                    Pincode = o.ShippingAddress.Pincode,
                    IsDefault = o.ShippingAddress.IsDefault
                } : null,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .ToListAsync();
    }

    // USER CANCEL ORDER
    public async Task<OrderDto> CancelOrderAsync(int userId, int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
            throw new Exception("Order not found");

        if (order.Status != OrderStatus.Pending &&
      order.Status != OrderStatus.Processing)
        {
            throw new Exception("Only pending or processing orders can be cancelled");
        }

        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
                product.StockQuantity += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingSnapshot ?? string.Empty,
            Status = order.Status.ToString(),
            IsPaid = order.IsPaid,
            OrderDate = order.OrderDate,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }

    // ---------------- ADMIN ORDER STATUS ----------------
    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(orderId)
            ?? throw new Exception("Order not found");

        if (order.Status == OrderStatus.Cancelled)
            throw new Exception("Cannot update a cancelled order");

        if (order.Status == OrderStatus.Delivered)
            throw new Exception("Delivered orders cannot be updated");

        order.Status = status;
        await _context.SaveChangesAsync();
    }

    // ADMIN - Get orders by status with full address details
    public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.ShippingAddress)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingSnapshot ?? string.Empty,
                Status = status.ToString(),
                IsPaid = o.IsPaid,
                OrderDate = o.OrderDate,
                PaymentMode = o.PaymentMode,
                ShippingAddressDetails = o.ShippingAddress != null ? new ShippingAddressDto
                {
                    Id = o.ShippingAddress.Id,
                    FullName = o.ShippingAddress.FullName,
                    Phone = o.ShippingAddress.Phone,
                    AltPhone = o.ShippingAddress.AltPhone,
                    AddressLine = o.ShippingAddress.AddressLine,
                    Landmark = o.ShippingAddress.Landmark,
                    City = o.ShippingAddress.City,
                    State = o.ShippingAddress.State,
                    Pincode = o.ShippingAddress.Pincode,
                    IsDefault = o.ShippingAddress.IsDefault
                } : null,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToListAsync();
    }

    public async Task ConfirmPaymentAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstAsync(o => o.Id == orderId);

        if (order.IsPaid) return;

        foreach (var item in order.Items)
        {
            if (item.Product.StockQuantity < item.Quantity)
                throw new Exception($"Out of stock: {item.Product.Name}");

            item.Product.StockQuantity -= item.Quantity;
        }

        order.IsPaid = true;

        if (order.Status == OrderStatus.Pending)
        {
            order.Status = OrderStatus.Processing;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<OrderDto?> GetOrderByIdForPaymentAsync(int? userId, int orderId)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Id == orderId);

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        return await query.Select(o => new OrderDto
        {
            Id = o.Id,
            TotalAmount = o.TotalAmount,
            ShippingAddress = o.ShippingSnapshot ?? string.Empty,
            Status = o.Status.ToString(),
            IsPaid = o.IsPaid,
            OrderDate = o.OrderDate,
            PaymentMode = o.PaymentMode,
            Items = o.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).FirstOrDefaultAsync();
    }
}