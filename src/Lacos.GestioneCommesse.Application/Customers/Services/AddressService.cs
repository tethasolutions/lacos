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
    private readonly IRepository<ContactAddress> contactAddressRepository;
    private readonly ILacosDbContext dbContext;

    public AddressService(
        IMapper mapper,
        IRepository<ContactAddress> contactAddressRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.contactAddressRepository = contactAddressRepository;
        this.dbContext = dbContext;
    }

    public async Task<AddressDto> GetAddress(
        long id)
    {
        var address = await contactAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(ContactAddress), id);
        }

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> CreateAddress(
        AddressDto addressDto)
    {
        var address = addressDto.MapTo<ContactAddress>(mapper);

        if (address.IsMainAddress)
            ResetAddresses(address.ContactId, null);
        
        await contactAddressRepository.Insert(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task<AddressDto> UpdateAddress(
        long id,
        AddressDto addressDto)
    {
        var address = await contactAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(ContactAddress), id);
        }

        addressDto.MapTo(address, mapper);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }

    public async Task DeleteAddress(
        long id)
    {
        var address = await contactAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(ContactAddress), id);
        }

        if (address.IsMainAddress)
        {
            ResetAddresses(address.ContactId, address.Id);
            
            var firstAddress = await contactAddressRepository
                .Query()
                .FirstAsync(x => x.ContactId == address.ContactId && x.Id != address.Id);

            firstAddress.IsMainAddress = true;
        }
        
        contactAddressRepository.Delete(address);

        await dbContext.SaveChanges();
    }

    public async Task<AddressDto> SetMainAddress(
        long id)
    {
        var address = await contactAddressRepository.Get(id);

        if (address == null)
        {
            throw new NotFoundException(typeof(ContactAddress), id);
        }

        ResetAddresses(address.ContactId, address.Id);

        address.IsMainAddress = true;

        contactAddressRepository.Update(address);

        await dbContext.SaveChanges();

        return address.MapTo<AddressDto>(mapper);
    }
    
    private void ResetAddresses(
        long contactId,
        long? addressId)
    {
        var addresses = contactAddressRepository
            .Query()
            .Where(x => x.ContactId == contactId && x.Id != addressId)
            .ToArray();

        foreach (var a in addresses)
        {
            a.IsMainAddress = false;
            contactAddressRepository.Update(a);
        }
    }
}