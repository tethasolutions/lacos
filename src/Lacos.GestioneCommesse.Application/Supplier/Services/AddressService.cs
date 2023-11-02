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

    Task<IEnumerable<AddressDto>> GetSupplierAddresses(long supplierId);

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
    private readonly IRepository<SupplierAddress> supplierAddressRepository;
    private readonly ILacosDbContext dbContext;

    public AddressSupplierService(
        IMapper mapper,
        IRepository<SupplierAddress> supplierAddressRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.supplierAddressRepository = supplierAddressRepository;
        this.dbContext = dbContext;
    }

    public async Task<AddressDto> GetAddress(
        long id)
    {
        var address = await supplierAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(SupplierAddress), id);
        }

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> CreateAddress(
        AddressDto addressDto)
    {
        var address = addressDto.MapTo<SupplierAddress>(mapper);

        if (address.IsMainAddress)
            ResetAddresses(address.SupplierId, null);
        
        await supplierAddressRepository.Insert(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> UpdateAddress(
        long id,
        AddressDto addressDto)
    {
        var address = await supplierAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(SupplierAddress), id);
        }

        addressDto.MapTo(address, mapper);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task DeleteAddress(
        long id)
    {
        var address = await supplierAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(SupplierAddress), id);
        }

        if (address.IsMainAddress)
        {
            ResetAddresses(address.SupplierId, address.Id);
            
            var firstAddress = await supplierAddressRepository
                .Query()
                .FirstAsync(x => x.SupplierId == address.SupplierId && x.Id != address.Id);

            firstAddress.IsMainAddress = true;
        }
        
        supplierAddressRepository.Delete(address);

        await dbContext.SaveChanges();
    }

    public async Task<AddressDto> SetMainAddress(
        long id)
    {
        var address = await supplierAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(SupplierAddress), id);
        }

        ResetAddresses(address.SupplierId, address.Id);

        address.IsMainAddress = true;

        supplierAddressRepository.Update(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }
    
    private void ResetAddresses(
        long contactId,
        long? addressId)
    {
        var addresses = supplierAddressRepository
            .Query()
            .Where(x => x.SupplierId == contactId && x.Id != addressId)
            .ToArray();

        foreach (var a in addresses)
        {
            a.IsMainAddress = false;
            supplierAddressRepository.Update(a);
        }
    }

    public async Task<IEnumerable<AddressDto>> GetSupplierAddresses(
        long supplierId)
    {

        var addresses = supplierAddressRepository
            .Query()
            .Where(x => x.SupplierId == supplierId)
            .ToArray();

        if (addresses == null)
        {
            throw new NotFoundException(typeof(SupplierAddress), supplierId);
        }

        return addresses.MapTo<IEnumerable<AddressDto>>(mapper);
    }
}