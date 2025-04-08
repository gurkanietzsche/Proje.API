using Proje.API.DTOs;
using Proje.API.Repositories;

private readonly OrderRepository OorderRepository;
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