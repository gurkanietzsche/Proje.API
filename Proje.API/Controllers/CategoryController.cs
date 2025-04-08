using AutoMapper;
using Proje.API.DTOs;
using Proje.API.Repositories;

private readonly CategoryRepository _categoryRepository;
private readonly IMapper _mapper;
private readonly ResultDTO _result;

public CategoryController(
    CategoryRepository categoryRepository,
    IMapper mapper)
{
    _categoryRepository = categoryRepository;
    _mapper = mapper;
    _result = new ResultDTO();
}