using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Customers.Services;

public interface IAddressService
{
    Task<AddressDto> GetAddress(
        long id);

    Task<AddressDto> CreateAddress(
        AddressDto addressDto);

    Task<AddressDto> UpdateAddress(
        long id,
        AddressDto addressDto);

    Task DeleteAddress(
        long id);

    Task<AddressDto> SetMainAddress(
        long id);
}

public class AddressService : IAddressService
{
    private readonly IMapper mapper;
    private readonly IRepository<CustomerAddress> customerAddressRepository;
    private readonly ILacosDbContext dbContext;

    public AddressService(
        IMapper mapper,
        IRepository<CustomerAddress> customerAddressRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.customerAddressRepository = customerAddressRepository;
        this.dbContext = dbContext;
    }

    public async Task<AddressDto> GetAddress(
        long id)
    {
        var address = await customerAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(CustomerAddress), id);
        }

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> CreateAddress(
        AddressDto addressDto)
    {
        var address = addressDto.MapTo<CustomerAddress>(mapper);

        if (address.IsMainAddress)
            ResetAddresses(address.CustomerId, null);
        
        await customerAddressRepository.Insert(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> UpdateAddress(
        long id,
        AddressDto addressDto)
    {
        var address = await customerAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(CustomerAddress), id);
        }

        addressDto.MapTo(address, mapper);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task DeleteAddress(
        long id)
    {
        var address = await customerAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(CustomerAddress), id);
        }

        if (address.IsMainAddress)
        {
            ResetAddresses(address.CustomerId, address.Id);
            
            var firstAddress = await customerAddressRepository
                .Query()
                .FirstAsync(x => x.CustomerId == address.CustomerId && x.Id != address.Id);

            firstAddress.IsMainAddress = true;
        }
        
        customerAddressRepository.Delete(address);

        await dbContext.SaveChanges();
    }

    public async Task<AddressDto> SetMainAddress(
        long id)
    {
        var address = await customerAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(CustomerAddress), id);
        }

        ResetAddresses(address.CustomerId, address.Id);

        address.IsMainAddress = true;

        customerAddressRepository.Update(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }
    
    private void ResetAddresses(
        long contactId,
        long? addressId)
    {
        var addresses = customerAddressRepository
            .Query()
            .Where(x => x.CustomerId == contactId && x.Id != addressId)
            .ToArray();

        foreach (var a in addresses)
        {
            a.IsMainAddress = false;
            customerAddressRepository.Update(a);
        }
    }
}