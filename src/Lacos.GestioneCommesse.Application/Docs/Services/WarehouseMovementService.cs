using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class WarehouseMovementService : IWarehouseMovementService
{
    private readonly IMapper mapper;
    private readonly IRepository<WarehouseMovement> warehouseMovementRepository;
    private readonly ILacosDbContext dbContext;

    public WarehouseMovementService(
        IMapper mapper,
        IRepository<WarehouseMovement> warehouseMovementRepository,
        ILacosDbContext dbContext)
    {
        this.mapper = mapper;
        this.warehouseMovementRepository = warehouseMovementRepository;
        this.dbContext = dbContext;
    }

    public IQueryable<WarehouseMovementReadModel> GetWarehouseMovements(long productId)
    {
        var movements = warehouseMovementRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .ProjectTo<WarehouseMovementReadModel>(mapper.ConfigurationProvider);

        return movements;
    }

    public async Task<WarehouseMovementDto> GetWarehouseMovement(long id)
    {
        if (id == 0)
            throw new LacosException("Impossibile recuperare un movimento con id 0");

        var movement = await warehouseMovementRepository
            .Query()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (movement == null)
            throw new LacosException($"Impossibile trovare il movimento con id {id}");

        return movement.MapTo<WarehouseMovementDto>(mapper);
    }

    public async Task<WarehouseMovementDto> CreateWarehouseMovement(WarehouseMovementDto dto)
    {
        if (dto.Id > 0)
            throw new LacosException("Impossibile creare un nuovo movimento con un id già esistente");

        var movement = dto.MapTo<WarehouseMovement>(mapper);
        warehouseMovementRepository.Insert(movement);
        await dbContext.SaveChanges();

        return movement.MapTo<WarehouseMovementDto>(mapper);
    }

    public async Task<WarehouseMovementDto> UpdateWarehouseMovement(long id, WarehouseMovementDto dto)
    {
        if (id == 0)
            throw new LacosException("Impossibile aggiornare un movimento con id 0");

        var movement = await warehouseMovementRepository
            .Query()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (movement == null)
            throw new LacosException($"Impossibile trovare il movimento con id {id}");

        dto.MapTo(movement, mapper);
        warehouseMovementRepository.Update(movement);
        await dbContext.SaveChanges();

        return movement.MapTo<WarehouseMovementDto>(mapper);
    }

    public async Task DeleteWarehouseMovement(long id)
    {
        if (id == 0)
            throw new LacosException("Impossibile eliminare un movimento con id 0");

        var movement = await warehouseMovementRepository
            .Query()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (movement == null)
            throw new LacosException($"Impossibile trovare il movimento con id {id}");

        warehouseMovementRepository.Delete(movement);
        await dbContext.SaveChanges();
    }
}
