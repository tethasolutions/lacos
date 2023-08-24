using AutoMapper;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Framework.Exceptions;

namespace Lacos.GestioneCommesse.Application.Vehicles.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetVehicles();

        Task<VehicleDto> CreateVehicle(VehicleDto vehicleDto);

        Task UpdateVehicle(long id, VehicleDto vehicleDto);

        Task<VehicleDto> GetVehicle(long id);

    }

    public class VehicleService : IVehicleService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Vehicle> vehicleRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public VehicleService(
            IMapper mapper,
            IRepository<Vehicle> vehicleRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.vehicleRepository = vehicleRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<VehicleDto> CreateVehicle(VehicleDto vehicleDto)
        {
            if (vehicleDto.Id > 0)
                throw new LacosException("Impossibile creare un nuovo tipo con un id già esistente");

            var vehicle = vehicleDto.MapTo<Vehicle>(mapper);

            await vehicleRepository.Insert(vehicle);

            await dbContext.SaveChanges();

            return vehicle.MapTo<VehicleDto>(mapper);
        }

        public async Task<VehicleDto> GetVehicle(long id)
        {
            var vehicle = await vehicleRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return vehicle.MapTo<VehicleDto>(mapper);
        }

        public async Task<IEnumerable<VehicleDto>> GetVehicles()
        {
            var vehicles = await vehicleRepository
                .Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return vehicles.MapTo<IEnumerable<VehicleDto>>(mapper);
        }

        public async Task UpdateVehicle(long id, VehicleDto vehicleDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un tipo con id 0");

            var vehicle = await vehicleRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (vehicle == null)
                throw new LacosException($"Impossibile trovare un tipo con id {id}");

            vehicleDto.MapTo(vehicle, mapper);
            vehicleRepository.Update(vehicle);
            await dbContext.SaveChanges();
        }
    }
}
