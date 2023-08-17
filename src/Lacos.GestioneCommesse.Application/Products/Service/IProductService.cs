using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lacos.GestioneCommesse.Application.Products.DTOs;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Application.Registry.DTOs;

namespace Lacos.GestioneCommesse.Application.Products.Service
{
     public interface IProductService
     {
         IQueryable<ProductDto> GetProducts();
         Task<ProductReadModel> GetProductDetail(long productId);
         Task UpdateProduct(long id, ProductDto productDto);
         Task<ProductDto> CreateProduct(ProductDto productDto);
         Task DeleteProduct(long productId);
         Task<IEnumerable<ProductTypeDto>> GetProductTypes();
         Task<string> CreateProductQrCode(long productId);
     }
    public class ProductService : IProductService
    {
        private readonly ILacosDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<ProductType> productTypeRepository;
        public ProductService(IRepository<Product> productRepository, IMapper mapper, ILacosDbContext dbContext, IRepository<ProductType> productTypeRepository)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.productTypeRepository = productTypeRepository;
        }

        public IQueryable<ProductDto> GetProducts()
        {
            var products = productRepository
                .Query()
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Project<ProductDto>(mapper);
            return products;
        }

        public async Task<ProductReadModel> GetProductDetail(long productId)
        {
            if (productId == 0)
                throw new ApplicationException("Impossibile recuperare un prodotto con id 0");

            var product = await productRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.ProductType)
                .Where(x => x.Id == productId)
                .SingleOrDefaultAsync();

            if (product == null)
                throw new ApplicationException($"Impossibile trovare l'articolo con id {productId}");

            return product.MapTo<ProductReadModel>(mapper);

        }

        public async Task UpdateProduct(long id, ProductDto productDto)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile aggiornare un operatore con id 0");

            var product = await productRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            
            if (product == null)
                throw new ApplicationException($"Impossibile trovare operatore con id {id}");

            productDto.MapTo(product, mapper);

            await dbContext.SaveChanges();
        }

        public async Task<ProductDto> CreateProduct(ProductDto productDto)
        {
            var product = productDto.MapTo<Product>(mapper);

            await productRepository.Insert(product);

         
            await dbContext.SaveChanges();

            return product.MapTo<ProductDto>(mapper);
        }

        public async Task DeleteProduct(long productId)
        {
            if (productId == 0)
                throw new ApplicationException("Impossible eliminare un prodotto con id 0");

            var product = await productRepository
                .Query()
                .Where(x => x.Id == productId)
                .SingleOrDefaultAsync();

            if (product == null)
                throw new ApplicationException($"Impossibile trovare il prodotto con id {productId}");

            productRepository.Delete(product);
            await dbContext.SaveChanges();
        }

        public async Task<IEnumerable<ProductTypeDto>> GetProductTypes()
        {
            var productTypes = await productTypeRepository
                .Query()
                .AsNoTracking()
                .ToListAsync();

            return productTypes.MapTo<IEnumerable<ProductTypeDto>>(mapper);
        }

        public async Task<string> CreateProductQrCode(long productId)
        {
            if (productId == 0)
                throw new ApplicationException("Impossible eliminare un prodotto con id 0");

            var product = await 
                productRepository
                .Query()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == productId);

            if (product == null)
                throw new ApplicationException($"Impossibile trovare il prodotto con id {productId}");


            if (product.QrCode == null)
            {
               //TODO Codice per creare QrCode

            }
           
            return product.QrCode;

        }
    }
}
