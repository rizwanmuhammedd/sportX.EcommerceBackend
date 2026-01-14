    //using Microsoft.EntityFrameworkCore;
    //using Sportex.Application.DTOs.Orders;
    //using Sportex.Application.Interfaces;
    //using Sportex.Domain.Entities;
    //using Sportex.Domain.Enums;
    //using Sportex.Infrastructure.Data;

    //namespace Sportex.Infrastructure.Services;

    //public class OrderService : IOrderService
    //{
    //    private readonly SportexDbContext _context;
    //    public OrderService(SportexDbContext context) => _context = context;

    //    // CART CHECKOUT
    //    public async Task<int> PlaceOrderAsync(int userId, CreateCartOrderDto dto)
    //    {
    //        var cartItems = await _context.CartItems
    //            .Include(c => c.Product)
    //            .Where(x => x.UserId == userId)
    //            .ToListAsync();

    //        if (!cartItems.Any())
    //            throw new Exception("Cart is empty");

    //        using var trx = await _context.Database.BeginTransactionAsync();

    //        decimal total = 0;
    //        var order = new Order
    //        {
    //            UserId = userId,
    //            ShippingAddress = dto.ShippingAddress,
    //            Status = OrderStatus.Pending,
    //            OrderDate = DateTime.UtcNow,
    //            IsPaid = false
    //        };

    //        _context.Orders.Add(order);
    //        await _context.SaveChangesAsync();

    //        foreach (var item in cartItems)
    //        {
    //            if (item.Quantity > item.Product.StockQuantity)
    //                throw new Exception($"Not enough stock for {item.Product.Name}");

    //            item.Product.StockQuantity -= item.Quantity;

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
    //        _context.CartItems.RemoveRange(cartItems);

    //        await _context.SaveChangesAsync();
    //        await trx.CommitAsync();
    //        return order.Id;
    //    }

    //    // BUY NOW
    //    public async Task<int> PlaceDirectOrderAsync(int userId, CreateDirectOrderDto dto)
    //    {
    //        if (!dto.Items.Any())
    //            throw new Exception("No items selected");

    //        using var trx = await _context.Database.BeginTransactionAsync();

    //        var order = new Order
    //        {
    //            UserId = userId,
    //            ShippingAddress = dto.ShippingAddress,
    //            Status = OrderStatus.Pending,
    //            IsPaid = false,
    //            OrderDate = DateTime.UtcNow
    //        };

    //        _context.Orders.Add(order);
    //        await _context.SaveChangesAsync();

    //        decimal total = 0;

    //        foreach (var item in dto.Items)
    //        {
    //            var product = await _context.Products.FindAsync(item.ProductId);
    //            if (product == null) throw new Exception("Product not found");

    //            if (item.Quantity < 1 || item.Quantity > product.StockQuantity)
    //                throw new Exception($"Invalid quantity for {product.Name}");

    //            product.StockQuantity -= item.Quantity;

    //            var oi = new OrderItem
    //            {
    //                OrderId = order.Id,
    //                ProductId = item.ProductId,
    //                Quantity = item.Quantity,
    //                UnitPrice = product.Price
    //            };

    //            total += product.Price * item.Quantity;
    //            _context.OrderItems.Add(oi);
    //        }

    //        order.TotalAmount = total;

    //        await _context.SaveChangesAsync();
    //        await trx.CommitAsync();
    //        return order.Id;
    //    }

    //    // MY ORDERS
    //    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
    //    {
    //        return await _context.Orders
    //            .Where(o => o.UserId == userId)
    //            .Include(o => o.Items).ThenInclude(i => i.Product)
    //            .Select(o => new OrderDto
    //            {
    //                Id = o.Id,
    //                TotalAmount = o.TotalAmount,
    //                ShippingAddress = o.ShippingAddress,
    //                Status = o.Status,
    //                IsPaid = o.IsPaid,
    //                OrderDate = o.OrderDate,
    //                Items = o.Items.Select(i => new OrderItemDto
    //                {
    //                    ProductId = i.ProductId,
    //                    ProductName = i.Product.Name,
    //                    ImageUrl = i.Product.ImageUrl,
    //                    Quantity = i.Quantity,
    //                    UnitPrice = i.UnitPrice
    //                }).ToList()
    //            }).ToListAsync();
    //    }

    //    public async Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId)
    //    {
    //        return await _context.Orders
    //            .Where(o => o.Id == orderId && o.UserId == userId)
    //            .Include(o => o.Items).ThenInclude(i => i.Product)
    //            .Select(o => new OrderDto
    //            {
    //                Id = o.Id,
    //                TotalAmount = o.TotalAmount,
    //                ShippingAddress = o.ShippingAddress,
    //                Status = o.Status,
    //                IsPaid = o.IsPaid,
    //                OrderDate = o.OrderDate,
    //                Items = o.Items.Select(i => new OrderItemDto
    //                {
    //                    ProductId = i.ProductId,
    //                    ProductName = i.Product.Name,
    //                    ImageUrl = i.Product.ImageUrl,
    //                    Quantity = i.Quantity,
    //                    UnitPrice = i.UnitPrice
    //                }).ToList()
    //            }).FirstOrDefaultAsync();
    //    }

    //    public async Task PayAsync(int userId, int orderId)
    //    {
    //        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
    //        if (order == null) throw new Exception("Order not found");

    //        order.IsPaid = true;
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task ToggleStatusAsync(int orderId)
    //    {
    //        var order = await _context.Orders.FindAsync(orderId);
    //        if (order == null) throw new Exception("Order not found");

    //        order.Status = order.Status == OrderStatus.Pending ? OrderStatus.Shipped : OrderStatus.Pending;
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task<List<OrderDto>> GetAllOrdersAsync()
    //    {
    //        return await _context.Orders
    //            .Include(o => o.Items).ThenInclude(i => i.Product)
    //            .Select(o => new OrderDto
    //            {
    //                Id = o.Id,
    //                TotalAmount = o.TotalAmount,
    //                ShippingAddress = o.ShippingAddress,
    //                Status = o.Status,
    //                IsPaid = o.IsPaid,
    //                OrderDate = o.OrderDate,
    //                Items = o.Items.Select(i => new OrderItemDto
    //                {
    //                    ProductId = i.ProductId,
    //                    ProductName = i.Product.Name,
    //                    ImageUrl = i.Product.ImageUrl,
    //                    Quantity = i.Quantity,
    //                    UnitPrice = i.UnitPrice
    //                }).ToList()
    //            }).ToListAsync();
    //    }


    //    public async Task<List<OrderDto>> GetAllOrdersAsync()
    //    {
    //        return await _context.Orders
    //            .Include(o => o.Items).ThenInclude(i => i.Product)
    //            .Select(o => new OrderDto
    //            {
    //                Id = o.Id,
    //                TotalAmount = o.TotalAmount,
    //                ShippingAddress = o.ShippingAddress,
    //                Status = o.Status,
    //                IsPaid = o.IsPaid,
    //                OrderDate = o.OrderDate,
    //                Items = o.Items.Select(i => new OrderItemDto
    //                {
    //                    ProductId = i.ProductId,
    //                    ProductName = i.Product.Name,
    //                    ImageUrl = i.Product.ImageUrl,
    //                    Quantity = i.Quantity,
    //                    UnitPrice = i.UnitPrice
    //                }).ToList()
    //            }).ToListAsync();
    //    }

    //    /* 👇 PASTE HERE */
    //    public async Task<OrderDto> CancelOrderAsync(int userId, int orderId)
    //    {
    //        var order = await _context.Orders
    //            .Include(o => o.Items)
    //            .ThenInclude(i => i.Product)
    //            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

    //        if (order == null)
    //            throw new Exception("Order not found");

    //        if (order.Status != OrderStatus.Pending)
    //            throw new Exception("Only pending orders can be cancelled");

    //        foreach (var item in order.Items)
    //        {
    //            var product = await _context.Products.FindAsync(item.ProductId);
    //            if (product != null)
    //                product.StockQuantity += item.Quantity;
    //        }

    //        order.Status = OrderStatus.Cancelled;
    //        await _context.SaveChangesAsync();

    //        return new OrderDto
    //        {
    //            Id = order.Id,
    //            TotalAmount = order.TotalAmount,
    //            ShippingAddress = order.ShippingAddress,
    //            Status = order.Status,
    //            IsPaid = order.IsPaid,
    //            OrderDate = order.OrderDate,
    //            Items = order.Items.Select(i => new OrderItemDto
    //            {
    //                ProductId = i.ProductId,
    //                ProductName = i.Product.Name,
    //                ImageUrl = i.Product.ImageUrl,
    //                Quantity = i.Quantity,
    //                UnitPrice = i.UnitPrice
    //            }).ToList()
    //        };
    //    }
    //    /* 👆 END PASTE */

    //}







    using Microsoft.EntityFrameworkCore;
    using Sportex.Application.DTOs.Orders;
    using Sportex.Application.Interfaces;
    using Sportex.Domain.Entities;
    using Sportex.Domain.Enums;
    using Sportex.Infrastructure.Data;

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
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                IsPaid = false
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product.StockQuantity)
                    throw new Exception($"Not enough stock for {item.Product.Name}");

       

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

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                Status = OrderStatus.Pending,
                IsPaid = false,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) throw new Exception("Product not found");

                if (item.Quantity < 1 || item.Quantity > product.StockQuantity)
                    throw new Exception($"Invalid quantity for {product.Name}");

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

        // MY ORDERS
        public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status.ToString(),

                    IsPaid = o.IsPaid,
                    OrderDate = o.OrderDate,
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
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status.ToString(),

                    IsPaid = o.IsPaid,
                    OrderDate = o.OrderDate,
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

        // ADMIN VIEW ALL
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status.ToString(),
                    IsPaid = o.IsPaid,
                    OrderDate = o.OrderDate,
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

        // USER CANCEL ORDER
        public async Task<OrderDto> CancelOrderAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Only pending orders can be cancelled");

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
                ShippingAddress = order.ShippingAddress,
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

            order.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    Status = status.ToString(),

                    IsPaid = o.IsPaid,
                    OrderDate = o.OrderDate,
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
            order.Status = OrderStatus.Processing;

            await _context.SaveChangesAsync();
        }


    }

