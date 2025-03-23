using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ResultDTO _result;

        public OrderController(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _result = new ResultDTO();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            var orderDtos = orders.Select(MapOrderToDto).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderRepository.GetOrderWithItemsAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Kullanıcı sadece kendi siparişlerini görebilir (Admin dışında)
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var orderDto = MapOrderToDto(order);

            return Ok(orderDto);
        }

        [HttpPost("create-from-cart")]
        public async Task<ActionResult<ResultDTO>> CreateOrderFromCart([FromBody] OrderCreateDTO orderCreateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcının sepetini al
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                _result.Status = false;
                _result.Message = "Sepetiniz boş";
                return BadRequest(_result);
            }

            // Yeni sipariş oluştur
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Beklemede",
                Address = orderCreateDto.Address,
                City = orderCreateDto.City,
                PostalCode = orderCreateDto.PostalCode,
                Country = orderCreateDto.Country,
                PaymentMethod = orderCreateDto.PaymentMethod,
                TotalAmount = cart.CartItems.Sum(item => item.Quantity * item.Product.Price)
            };

            await _orderRepository.AddAsync(order);

            // Sipariş detaylarını ekle
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };

                // Ürün stoğunu güncelle
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                order.OrderItems ??= new List<OrderItem>();
                order.OrderItems.Add(orderItem);
            }

            await _orderRepository.UpdateAsync(order);

            // Sepeti temizle
            await _cartRepository.ClearCartAsync(cart.Id);

            _result.Status = true;
            _result.Message = "Sipariş başarıyla oluşturuldu";
            _result.Data = order.Id;

            return _result;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersWithItemsAsync();
            var orderDtos = orders.Select(MapOrderToDto).ToList();

            return Ok(orderDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<ResultDTO>> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateDTO statusUpdateDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                _result.Status = false;
                _result.Message = "Sipariş bulunamadı";
                return NotFound(_result);
            }

            order.Status = statusUpdateDto.Status;

            if (statusUpdateDto.Status == "Teslim Edildi" && !order.IsPaid)
            {
                order.IsPaid = true;
                order.PaymentDate = DateTime.Now;
            }

            await _orderRepository.UpdateAsync(order);

            _result.Status = true;
            _result.Message = "Sipariş durumu güncellendi";
            return _result;
        }

        private OrderDTO MapOrderToDto(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Address = order.Address,
                City = order.City,
                PostalCode = order.PostalCode,
                Country = order.Country,
                PaymentMethod = order.PaymentMethod,
                IsPaid = order.IsPaid,
                PaymentDate = order.PaymentDate,
                OrderItems = order.OrderItems?.Select(item => new OrderItemDTO
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList() ?? new List<OrderItemDTO>()
            };
        }
    }
}