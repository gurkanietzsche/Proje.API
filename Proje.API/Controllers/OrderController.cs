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
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;
        private readonly CartRepository _cartRepository;
        private readonly ProductRepository _productRepository;
        private readonly ResultDTO _result;

        public OrderController(
            OrderRepository orderRepository,
            CartRepository cartRepository,
            ProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _result = new ResultDTO();
        }

        // GET: api/Order
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Admin kullanıcılar tüm siparişleri görebilir
                if (User.IsInRole("Admin"))
                {
                    var allOrders = await _orderRepository.GetAllOrdersWithItemsAsync();
                    return Ok(allOrders);
                }

                // Normal kullanıcılar sadece kendi siparişlerini görebilir
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Siparişler getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(500, _result);
            }
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithItemsAsync(id);

                if (order == null)
                {
                    _result.Status = false;
                    _result.Message = "Sipariş bulunamadı";
                    return NotFound(_result);
                }

                // Admin değilse ve başkasının siparişine erişmeye çalışıyorsa 
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!User.IsInRole("Admin") && order.UserId != userId)
                {
                    _result.Status = false;
                    _result.Message = "Bu siparişe erişim izniniz yok";
                    return Forbid();
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Sipariş getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(500, _result);
            }
        }

        // POST: api/Order
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResultDTO>> CreateOrder(OrderCreateDTO orderDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Kullanıcının sepetini kontrol et
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    _result.Status = false;
                    _result.Message = "Sepetinizde ürün bulunmamaktadır";
                    return BadRequest(_result);
                }

                // Yeni sipariş oluştur
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Status = "Beklemede",
                    Address = orderDto.Address,
                    City = orderDto.City,
                    PostalCode = orderDto.PostalCode,
                    Country = orderDto.Country,
                    PaymentMethod = orderDto.PaymentMethod,
                    IsPaid = false,
                    TotalAmount = cart.CartItems.Sum(item => item.Quantity * item.Product.Price)
                };

                // Siparişi veritabanına ekle
                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync();

                // Sipariş öğelerini ekle
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    };

                    await _orderRepository.AddOrderItemAsync(orderItem);

                    // Ürün stoğunu güncelle
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        product.Stock -= cartItem.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }

                await _orderRepository.SaveChangesAsync();
                await _productRepository.SaveChangesAsync();

                // Sepeti temizle
                await _cartRepository.ClearCartAsync(cart.Id);

                _result.Status = true;
                _result.Message = "Siparişiniz başarıyla oluşturuldu";
                _result.Data = order.Id;

                return Ok(_result);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Sipariş oluşturulurken bir hata oluştu: " + ex.Message;
                return StatusCode(500, _result);
            }
        }

        // PUT: api/Order/5/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDTO>> UpdateOrderStatus(int id, OrderStatusUpdateDTO statusDto)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);

                if (order == null)
                {
                    _result.Status = false;
                    _result.Message = "Sipariş bulunamadı";
                    return NotFound(_result);
                }

                // Sipariş durumunu güncelle
                order.Status = statusDto.Status;

                // Teslim edildiyse ödeme durumunu da güncelle
                if (statusDto.Status == "Teslim Edildi")
                {
                    order.IsPaid = true;
                    order.PaymentDate = DateTime.Now;
                }

                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Sipariş durumu başarıyla güncellendi";

                return Ok(_result);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Sipariş durumu güncellenirken bir hata oluştu: " + ex.Message;
                return StatusCode(500, _result);
            }
        }

        // GET: api/Order/user
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetUserOrders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Siparişler getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(500, _result);
            }
        }
    }
}