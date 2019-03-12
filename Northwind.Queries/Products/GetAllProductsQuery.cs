﻿using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Persistence;

namespace Northwind.Application.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<ProductsListViewModel> { }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ProductsListViewModel>
    {
        private readonly NorthwindDbContext _context;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(NorthwindDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductsListViewModel> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            // TODO: Set view model state based on user permissions.
            var products = await _context.Products.OrderBy(p => p.ProductName).Include(p => p.Supplier).ToListAsync(cancellationToken);

            var model = new ProductsListViewModel
            {
                Products = _mapper.Map<IEnumerable<ProductDto>>(products),
                CreateEnabled = true
            };

            return model;
        }
    }
}
