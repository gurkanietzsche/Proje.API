using Proje.API.DTOs;
using Proje.API.Repositories;

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