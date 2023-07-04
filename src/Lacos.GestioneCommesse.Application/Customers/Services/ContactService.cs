using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Customers.Services;

public interface IContactService
{
    Task<ContactDto> CreateContact(
        ContactDto dto);

    Task DeleteContact(
        long id);

    Task<ContactDto> UpdateContact(
        long id,
        ContactDto dto);

    Task<ContactDto> GetContact(
        long id);

    Task<IEnumerable<ContactDto>> GetContacts(
        ContactType type);
}

public class ContactService : IContactService
{
    private readonly IMapper mapper;
    private readonly IRepository<Contact> contactRepository;
    private readonly ILacosDbContext dbContext;

    public ContactService(
        IMapper mapper,
        IRepository<Contact> contactRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.contactRepository = contactRepository;
        this.dbContext = dbContext;
    }

    public async Task<ContactDto> CreateContact(
        ContactDto dto)
    {
        if (dto.Id > 0)
            throw new ApplicationException("Impossibile creare un nuovo contatto con un id già esistente");

        var contact = dto.MapTo<Contact>(mapper);
        contactRepository.Insert(contact);
        await dbContext.SaveChanges();

        return contact.MapTo<ContactDto>(mapper);
    }

    public async Task DeleteContact(
        long id)
    {
        if (id == 0)
            throw new ApplicationException("Impossible eliminare un contatto con id 0");

        var contact = await contactRepository
            .Query()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (contact == null)
            throw new ApplicationException($"Impossibile trovare il contatto con id {id}");

        contactRepository.Delete(contact);
        await dbContext.SaveChanges();
    }

    public async Task<ContactDto> UpdateContact(
        long id,
        ContactDto dto)
    {
        if (id == 0)
            throw new ApplicationException("Impossibile aggiornare un contatto con id 0");

        var contact = await contactRepository
            .Query()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (contact == null)
            throw new ApplicationException($"Impossibile trovare il contatto con id {id}");

        dto.MapTo(contact, mapper);
        contactRepository.Update(contact);
        await dbContext.SaveChanges();

        return contact.MapTo<ContactDto>(mapper);
    }

    public async Task<ContactDto> GetContact(
        long id)
    {
        if (id == 0)
            throw new ApplicationException("Impossibile recuperare un contatto con id 0");

        var contact = await contactRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (contact == null)
            throw new ApplicationException($"Impossibile trovare il contatto con id {id}");

        return contact.MapTo<ContactDto>(mapper);
    }

    public async Task<IEnumerable<ContactDto>> GetContacts(
        ContactType type)
    {
        var contacts = await contactRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Addresses)
            .Where(x => x.Type == type)
            .OrderBy(x => x.CompanyName ?? x.Surname)
            .ToArrayAsync();

        return contacts.MapTo<IEnumerable<ContactDto>>(mapper);
    }
}