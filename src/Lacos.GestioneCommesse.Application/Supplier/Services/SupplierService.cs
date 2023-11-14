using AutoMapper;
using Lacos.GestioneCommesse.Application.Suppliers.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Suppliers.Services;

public interface ISupplierService
{
    Task<SupplierDto> CreateSupplier(SupplierDto dto);

    Task DeleteSupplier(long id);

    Task<SupplierDto> UpdateSupplier(long id,SupplierDto dto);

    Task<SupplierDto> GetSupplier(long id);

    Task<IEnumerable<SupplierDto>> GetSuppliers();
}

public class SupplierService : ISupplierService
{
    private readonly IMapper mapper;
    private readonly IRepository<Supplier> supplierRepository;
    private readonly ILacosDbContext dbContext;

    public SupplierService(
        IMapper mapper,
        IRepository<Supplier> supplierRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.supplierRepository = supplierRepository;
        this.dbContext = dbContext;
    }

    public async Task<SupplierDto> CreateSupplier(SupplierDto dto)
    {
        if (dto.Id > 0)
            throw new LacosException("Impossibile creare un nuovo contatto con un id già esistente");

        var supplier = dto.MapTo<Supplier>(mapper);
        supplierRepository.Insert(supplier);
        await dbContext.SaveChanges();

        return supplier.MapTo<SupplierDto>(mapper);
    }

    public async Task DeleteSupplier(long id)
    {
        if (id == 0)
            throw new LacosException("Impossible eliminare un contatto con id 0");

        var supplier = await supplierRepository
            .Query()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (supplier == null)
            throw new LacosException($"Impossibile trovare il contatto con id {id}");

        supplierRepository.Delete(supplier);
        await dbContext.SaveChanges();
    }

    public async Task<SupplierDto> UpdateSupplier(long id,SupplierDto dto)
    {
        if (id == 0)
            throw new LacosException("Impossibile aggiornare un contatto con id 0");

        var supplier = await supplierRepository
            .Query()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (supplier == null)
            throw new LacosException($"Impossibile trovare il contatto con id {id}");

        dto.MapTo(supplier, mapper);
        supplierRepository.Update(supplier);
        await dbContext.SaveChanges();

        return supplier.MapTo<SupplierDto>(mapper);
    }

    public async Task<SupplierDto> GetSupplier(long id)
    {
        if (id == 0)
            throw new LacosException("Impossibile recuperare un contatto con id 0");

        var supplier = await supplierRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Addresses)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (supplier == null)
            throw new LacosException($"Impossibile trovare il contatto con id {id}");

        return supplier.MapTo<SupplierDto>(mapper);
    }

    public async Task<IEnumerable<SupplierDto>> GetSuppliers()
    {
        var suppliers = await supplierRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.Addresses)
            .OrderBy(x => x.Name)
            .ToArrayAsync();

        return suppliers.MapTo<IEnumerable<SupplierDto>>(mapper);
    }
}