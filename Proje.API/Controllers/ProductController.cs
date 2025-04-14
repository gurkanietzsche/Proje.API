using AutoMapper;
using Microsoft.IdentityModel.Tokens.Configuration;
using Proje.API.DTOs;
using Proje.API.Repositories;

namespace Proje.API.Controllers
{
    public class ProductController
    {
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public ProductController(ProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        // Add your action methods here
        // Make sure all methods return something
        public ResultDTO GetProducts()
        {
            // Your implementation
            return _result;
        }
    }
}