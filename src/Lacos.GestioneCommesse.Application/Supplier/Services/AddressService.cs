using AutoMapper;
using Lacos.GestioneCommesse.Application.Suppliers.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Suppliers.Services;

public interface IAddressSupplierService
{
    Task<AddressDto> GetAddress(
        long id);

    Task<IEnumerable<AddressDto>> GetAddresses(long supplierId);

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

public class AddressSupplierService : IAddressSupplierService
{
    private readonly IMapper mapper;
    private readonly IRepository<Address> addressRepository;
    private readonly ILacosDbContext dbContext;

    public AddressSupplierService(
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

        if (address.IsMainAddress)
            ResetAddresses(address.SupplierId, null);
        
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

        if (address.IsMainAddress)
        {
            ResetAddresses(address.SupplierId, address.Id);
            
            var firstAddress = await addressRepository
                .Query()
                .FirstAsync(x => x.SupplierId == address.SupplierId && x.Id != address.Id);

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

        ResetAddresses(address.SupplierId, address.Id);

        address.IsMainAddress = true;

        addressRepository.Update(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }
    
    private void ResetAddresses(
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

    public async Task<IEnumerable<AddressDto>> GetAddresses(
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
}