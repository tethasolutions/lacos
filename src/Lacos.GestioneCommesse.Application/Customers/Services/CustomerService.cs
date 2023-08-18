using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Customers.Services;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomer(CustomerDto dto);

    Task DeleteCustomer(long id);

    Task<CustomerDto> UpdateCustomer(long id,CustomerDto dto);

    Task<CustomerDto> GetCustomer(long id);

    Task<IEnumerable<CustomerDto>> GetCustomers();
}

public class CustomerService : ICustomerService
{
    private readonly IMapper mapper;
    private readonly IRepository<Customer> customerRepository;
    private readonly ILacosDbContext dbContext;

    public CustomerService(
        IMapper mapper,
        IRepository<Customer> customerRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.customerRepository = customerRepository;
        this.dbContext = dbContext;
    }

    public async Task<CustomerDto> CreateCustomer(CustomerDto dto)
    {
        if (dto.Id > 0)
            throw new LacosException("Impossibile creare un nuovo contatto con un id già esistente");

        var customer = dto.MapTo<Customer>(mapper);
        customerRepository.Insert(customer);
        await dbContext.SaveChanges();

        return customer.MapTo<CustomerDto>(mapper);
    }

    public async Task DeleteCustomer(long id)
    {
        if (id == 0)
            throw new LacosException("Impossible eliminare un contatto con id 0");

        var customer = await customerRepository
            .Query()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (customer == null)
            throw new LacosException($"Impossibile trovare il contatto con id {id}");

        customerRepository.Delete(customer);
        await dbContext.SaveChanges();
    }

    public async Task<CustomerDto> UpdateCustomer(long id,CustomerDto dto)
    {
        if (id == 0)
            throw new LacosException("Impossibile aggiornare un contatto con id 0");

        var customer = await customerRepository
            .Query()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (customer == null)
            throw new LacosException($"Impossibile trovare il contatto con id {id}");

        dto.MapTo(customer, mapper);
        customerRepository.Update(customer);
        await dbContext.SaveChanges();

        return customer.MapTo<CustomerDto>(mapper);
    }

    public async Task<CustomerDto> GetCustomer(long id)
    {
        if (id == 0)
            throw new LacosException("Impossibile recuperare un contatto con id 0");

        var customer = await customerRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (customer == null)
            throw new LacosException($"Impossibile trovare il contatto con id {id}");

        return customer.MapTo<CustomerDto>(mapper);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomers()
    {
        var customers = await customerRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Addresses)
            .OrderBy(x => x.Name)
            .ToArrayAsync();

        return customers.MapTo<IEnumerable<CustomerDto>>(mapper);
    }
}