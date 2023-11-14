using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Registry.Services;

public interface IAddressService
{
    Task<AddressDto> GetAddress(
        long id);

    Task<IEnumerable<AddressDto>> GetAddresses();
    Task<IEnumerable<AddressDto>> GetSupplierAddresses(long supplierId);
    Task<IEnumerable<AddressDto>> GetCustomerAddresses(long customerId);

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
    private readonly IRepository<Address> addressRepository;
    private readonly ILacosDbContext dbContext;

    public AddressService(
        IMapper mapper,
        IRepository<Address> addressRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.addressRepository = addressRepository;
        this.dbContext = dbContext;
    }

    public async Task<AddressDto> GetAddress(
        long id)
    {
        var address = await addressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(Address), id);
        }

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> CreateAddress(
        AddressDto addressDto)
    {
        var address = addressDto.MapTo<Address>(mapper);

        if (address.IsMainAddress && address.SupplierId != null) ResetSupplierAddresses(address.SupplierId, null);
        if (address.IsMainAddress && address.CustomerId != null) ResetCustomerAddresses(address.CustomerId, null);

        await addressRepository.Insert(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> UpdateAddress(
        long id,
        AddressDto addressDto)
    {
        var address = await addressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(Address), id);
        }

        addressDto.MapTo(address, mapper);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task DeleteAddress(
        long id)
    {
        var address = await addressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(Address), id);
        }

        if (address.IsMainAddress && address.SupplierId != null)
        {
            ResetSupplierAddresses(address.SupplierId, address.Id);

            var firstAddress = await addressRepository
                .Query()
                .FirstAsync(x => x.SupplierId == address.SupplierId && x.Id != address.Id);

            firstAddress.IsMainAddress = true;
        }
        if (address.IsMainAddress && address.CustomerId != null)
        {
            ResetCustomerAddresses(address.CustomerId, address.Id);

            var firstAddress = await addressRepository
                .Query()
                .FirstAsync(x => x.CustomerId == address.CustomerId && x.Id != address.Id);

            firstAddress.IsMainAddress = true;
        }

        addressRepository.Delete(address);

        await dbContext.SaveChanges();
    }

    public async Task<AddressDto> SetMainAddress(
        long id)
    {
        var address = await addressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(Address), id);
        }

        if (address.SupplierId != null) ResetSupplierAddresses(address.SupplierId, address.Id);
        if (address.CustomerId != null) ResetCustomerAddresses(address.CustomerId, address.Id);

        address.IsMainAddress = true;

        addressRepository.Update(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    private void ResetSupplierAddresses(
        long? contactId,
        long? addressId)
    {
        var addresses = addressRepository
            .Query()
            .Where(x => x.SupplierId == contactId && x.Id != addressId)
            .ToArray();

        foreach (var a in addresses)
        {
            a.IsMainAddress = false;
            addressRepository.Update(a);
        }
    }
    private void ResetCustomerAddresses(
        long? contactId,
        long? addressId)
    {
        var addresses = addressRepository
            .Query()
            .Where(x => x.CustomerId == contactId && x.Id != addressId)
            .ToArray();

        foreach (var a in addresses)
        {
            a.IsMainAddress = false;
            addressRepository.Update(a);
        }
    }

    public async Task<IEnumerable<AddressDto>> GetAddresses()
    {

        var addresses = addressRepository
            .Query()
            .Where(x => x.SupplierId == null && x.CustomerId == null)
            .ToArray();

        if (addresses == null)
        {
            throw new NotFoundException(typeof(Address), 0);
        }

        return addresses.MapTo<IEnumerable<AddressDto>>(mapper);
    }

    public async Task<IEnumerable<AddressDto>> GetSupplierAddresses(
        long supplierId)
    {

        var addresses = addressRepository
            .Query()
            .Where(x => x.SupplierId == supplierId)
            .ToArray();

        if (addresses == null)
        {
            throw new NotFoundException(typeof(Address), supplierId);
        }

        return addresses.MapTo<IEnumerable<AddressDto>>(mapper);
    }
    public async Task<IEnumerable<AddressDto>> GetCustomerAddresses(
        long customerId)
    {

        var addresses = addressRepository
            .Query()
            .Where(x => x.CustomerId == customerId)
            .ToArray();

        if (addresses == null)
        {
            throw new NotFoundException(typeof(Address), customerId);
        }

        return addresses.MapTo<IEnumerable<AddressDto>>(mapper);
    }
}