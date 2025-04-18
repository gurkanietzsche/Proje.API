﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;
        private readonly UserRepository _userRepository;
        private readonly ResultDTO _result;

        public DashboardController(
            OrderRepository orderRepository,
            ProductRepository productRepository,
            UserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _result = new ResultDTO();
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetDashboardSummary()
        {
            try
            {
                var allOrders = await _orderRepository.GetAllOrdersWithItemsAsync();
                var products = await _productRepository.GetAllAsync();
                var users = await _userRepository.GetAllUsersAsync();

                // Son 30 günün siparişleri - UTC zamanı kullanarak
                var last30DaysOrders = allOrders.Where(o => o.OrderDate >= DateTime.UtcNow.AddDays(-30));

                var summary = new DashboardSummaryDTO
                {
                    TotalOrders = allOrders.Count(),
                    TotalProducts = products.Count(),
                    TotalUsers = users.Count(),
                    TotalRevenue = allOrders.Sum(o => o.TotalAmount),
                    RecentOrders = last30DaysOrders.OrderByDescending(o => o.OrderDate)
                        .Take(5)
                        .Select(o => new OrderDTO
                        {
                            Id = o.Id,
                            UserId = o.UserId,
                            OrderDate = o.OrderDate,
                            TotalAmount = o.TotalAmount,
                            Status = o.Status
                        }).ToList(),
                    MonthlyRevenue = last30DaysOrders
                        .GroupBy(o => o.OrderDate.ToString("yyyy-MM-dd"))
                        .Select(g => new ChartDataDTO
                        {
                            Label = g.Key,
                            Value = g.Sum(o => o.TotalAmount)
                        }).ToList(),
                    TopSellingProducts = allOrders.SelectMany(o => o.OrderItems)
                        .GroupBy(oi => oi.ProductId)
                        .Select(g => new TopProductDTO
                        {
                            ProductId = g.Key,
                            ProductName = g.First().Product.Name,
                            TotalSales = g.Sum(oi => oi.Quantity),
                            Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                        })
                        .OrderByDescending(p => p.Revenue)
                        .Take(5)
                        .ToList()
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Dashboard özeti alınırken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }
    }
}
    