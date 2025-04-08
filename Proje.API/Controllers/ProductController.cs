using AutoMapper;
using Proje.API.DTOs;
using Proje.API.Repositories;

private readonly ProductRepository _productRepository;
private readonly IMapper _mapper;
private readonly ResultDTO _result;

public ProductController(
    ProductRepository productRepository,
    IMapper mapper)
{
    _productRepository = productRepository;
    _mapper = mapper;
    _result = new ResultDTO();
}