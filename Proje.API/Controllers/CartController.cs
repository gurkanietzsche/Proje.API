using Proje.API.DTOs;
using Proje.API.Repositories;

private readonly CartRepository _cartRepository;
private readonly ProductRepository _productRepository;
private readonly ResultDTO _result;

public CartController(CartRepository cartRepository, ProductRepository productRepository)
{
    _cartRepository = cartRepository;
    _productRepository = productRepository;
    _result = new ResultDTO();
}