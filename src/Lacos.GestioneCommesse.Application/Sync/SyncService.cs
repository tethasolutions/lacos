using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Application.Docs.Services;
using Lacos.GestioneCommesse.Contracts.Dtos;
using Lacos.GestioneCommesse.Contracts.Dtos.Application;
using Lacos.GestioneCommesse.Contracts.Dtos.Docs;
using Lacos.GestioneCommesse.Contracts.Dtos.Registry;
using Lacos.GestioneCommesse.Contracts.Dtos.Security;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Configuration;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lacos.GestioneCommesse.Application.Sync
{
    public class SyncService : ISyncService
    {
        private readonly IMapper mapper;
        private readonly IServiceProvider serviceProvider;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosConfiguration configuration;


        public SyncService(
            IMapper mapper,
           IServiceProvider serviceProvider, ILacosDbContext dbContext, ILacosConfiguration configuration)
        {
            this.mapper = mapper;
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        public async Task<List<SyncUserDto>> SyncFromDBToApp_Users()
        {
            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();


            var users = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                userRepository.Query()
                    .Include(x => x.Operator)
                    .ToListAsync()
            , QueryFilter.SoftDelete); ;

            var ret = users.MapTo<List<SyncUserDto>>(mapper);
            return ret;
        }

        public async Task<List<SyncOperatorDto>> SyncFromDBToApp_Operators()
        {
            var operatorRepository = serviceProvider.GetRequiredService<IRepository<Operator>>();

            var operators = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                    operatorRepository.Query()
                        .Include(x => x.User)
                        .ToListAsync()
                , QueryFilter.SoftDelete);

            var ret = operators.MapTo<List<SyncOperatorDto>>(mapper);
            return ret;
        }

        public async Task<List<SyncVehicleDto>> SyncFromDBToApp_Vehicles()
        {
            var vehicleRepository = serviceProvider.GetRequiredService<IRepository<Vehicle>>();

            var vehicles = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                    vehicleRepository.Query()
                        .ToListAsync()
                , QueryFilter.SoftDelete);

            var ret = vehicles.MapTo<List<SyncVehicleDto>>(mapper);
            return ret;
        }

        public async Task<SyncLocalDbChanges> SyncFromAppToDB_LocalChanges(SyncLocalDbChanges syncLocalDbChanges)
        {
            SyncLocalDbChanges syncLocalDbChangesRemote = new SyncLocalDbChanges();
            try
            {
                await using (var transaction = await dbContext.BeginTransaction())
                {
                    syncLocalDbChangesRemote.Interventions = await InsertUpdateAllModified<SyncInterventionDto, Intervention>(syncLocalDbChanges.Interventions);
                    syncLocalDbChangesRemote.InterventionNotes = await InsertUpdateAllModified<SyncInterventionNoteDto, InterventionNote>(syncLocalDbChanges.InterventionNotes);
                    syncLocalDbChangesRemote.InterventionDisputes = await InsertUpdateAllModified<SyncInterventionDisputeDto, InterventionDispute>(syncLocalDbChanges.InterventionDisputes);
                    syncLocalDbChangesRemote.InterventionProducts = await InsertUpdateAllModified<SyncInterventionProductDto, InterventionProduct>(syncLocalDbChanges.InterventionProducts);
                    syncLocalDbChangesRemote.InterventionProductPictures = await InsertUpdateAllModified<SyncInterventionProductPictureDto, InterventionProductPicture>(syncLocalDbChanges.InterventionProductPictures);

                    syncLocalDbChangesRemote.InterventionProductCheckLists = await InsertUpdateAllModifiedInterventionProductCheckList(syncLocalDbChanges.InterventionProductCheckLists, syncLocalDbChanges.InterventionProductCheckListItems);
                    syncLocalDbChangesRemote.InterventionProductCheckListItems = await InsertUpdateAllModified<SyncInterventionProductCheckListItemDto, InterventionProductCheckListItem>(syncLocalDbChanges.InterventionProductCheckListItems);

                    syncLocalDbChangesRemote.Tickets = await InsertUpdateAllModifiedTicket(syncLocalDbChanges.Tickets, syncLocalDbChanges.TicketPictures);
                    syncLocalDbChangesRemote.TicketPictures = await InsertUpdateAllModified<SyncTicketPictureDto, TicketPicture>(syncLocalDbChanges.TicketPictures);

                    syncLocalDbChangesRemote.PurchaseOrders = await InsertUpdateAllModifiedPurchaseOrder(syncLocalDbChanges.PurchaseOrders, syncLocalDbChanges.PurchaseOrderItems);
                    syncLocalDbChangesRemote.PurchaseOrderItems = await InsertUpdateAllModified<SyncPurchaseOrderItemDto, PurchaseOrderItem>(syncLocalDbChanges.PurchaseOrderItems);

                    await transaction.CommitAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return syncLocalDbChangesRemote;
        }
         public async Task SyncFromAppToDB_LocalImage(SyncImageDto syncLocalImage)
        {
            try
            {
                    await using Stream streamOriginalImage = new MemoryStream(syncLocalImage.Content);
                    var folder = configuration.AttachmentsPath;
                    Directory.CreateDirectory(folder);
                    var path = Path.Combine(folder, syncLocalImage.Filename);
                    if (!File.Exists(path))
                    {
                        await using Stream stremNewImage = File.Create(path);
                        await streamOriginalImage.CopyToAsync(stremNewImage);
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

         public async Task SyncFromAppToDB_SignIntervention(SyncSignDto signDto)
         {
             try
             {
                 var repository = serviceProvider.GetRequiredService<IRepository<Intervention>>();
                 var entity = await
                     repository
                         .Query()
                         .Where(x=>x.Id == signDto.InterventionId)
                         .SingleOrDefaultAsync();
                 if (entity == null)
                     throw new NotFoundException("Intervention not found");

                 entity.CustomerSignatureName = signDto.NameSurname;
                 entity.FinalNotes = signDto.FinalNotes;
                 entity.CustomerSignatureFileName = signDto.Filename;
                 //entity. = signDto.NameSurname;
                 repository.Update(entity);
                 await dbContext.SaveChanges();

                 await using Stream streamOriginalImage = new MemoryStream(signDto.Content);
                 var folder = configuration.AttachmentsPath;
                 Directory.CreateDirectory(folder);
                 var path = Path.Combine(folder, signDto.Filename);
                
                 await using Stream stremNewImage = File.Create(path);
                 await streamOriginalImage.CopyToAsync(stremNewImage);
                 


             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 throw;
             }
         }


         public async Task<SyncImageDto> SyncFromDBToApp_RemoteImage(SyncImageDto syncRemoteImage)
         {
             try
             {
                 var folder = configuration.AttachmentsPath;
                 var path = Path.Combine(folder, syncRemoteImage.Filename);
                 if (File.Exists(path))
                 {
                     syncRemoteImage.Content = await File.ReadAllBytesAsync(path);
                    return syncRemoteImage;
                 }
                 //else
                 //{
                 //    throw new Exception("Immagine inesistente");
                 //}
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 
                 throw;
             }
             return null;
         }

        public async Task<SyncRemoteFullDbDto> SyncFromDBToApp_FullDb(DateTimeOffset date)
        {
            try
            {
                SyncRemoteFullDbDto syncFullDb = new SyncRemoteFullDbDto();

                syncFullDb.Activities = await GetAllModifiedRecord<Activity, SyncActivityDto>(date);
                syncFullDb.ActivityProducts = await GetAllModifiedRecord<ActivityProduct, SyncActivityProductDto>(date);
                syncFullDb.ActivityAttachments = await GetAllModifiedRecord<ActivityAttachment, SyncActivityAttachmentsDto>(date);
                syncFullDb.InterventionDisputes = await GetAllModifiedRecord<InterventionDispute, SyncInterventionDisputeDto>(date);
                syncFullDb.InterventionNotes = await GetAllModifiedRecord<InterventionNote, SyncInterventionNoteDto>(date);
                syncFullDb.InterventionProducts = await GetAllModifiedRecord<InterventionProduct, SyncInterventionProductDto>(date);
                syncFullDb.InterventionProductCheckLists = await GetAllModifiedRecord<InterventionProductCheckList, SyncInterventionProductCheckListDto>(date);
                syncFullDb.InterventionProductCheckListItems = await GetAllModifiedRecord<InterventionProductCheckListItem, SyncInterventionProductCheckListItemDto>(date);
                syncFullDb.InterventionProductPictures = await GetAllModifiedRecord<InterventionProductPicture, SyncInterventionProductPictureDto>(date);
                syncFullDb.Jobs = await GetAllModifiedRecord<Job, SyncJobDto>(date);
                syncFullDb.PurchaseOrders = await GetAllModifiedRecord<PurchaseOrder, SyncPurchaseOrderDto>(date);
                syncFullDb.PurchaseOrderItems = await GetAllModifiedRecord<PurchaseOrderItem, SyncPurchaseOrderItemDto>(date);
                syncFullDb.Tickets = await GetAllModifiedRecord<Ticket, SyncTicketDto>(date);
                syncFullDb.TicketPictures = await GetAllModifiedRecord<TicketPicture, SyncTicketPictureDto>(date);
                syncFullDb.CheckLists = await GetAllModifiedRecord<CheckList, SyncCheckListDto>(date);
                syncFullDb.CheckListItems = await GetAllModifiedRecord<CheckListItem, SyncCheckListItemDto>(date);
                syncFullDb.Customers = await GetAllModifiedRecord<Customer, SyncCustomerDto>(date);
                syncFullDb.Addresses = await GetAllModifiedRecord<Address, SyncAddressDto>(date);
                syncFullDb.OperatorDocuments = await GetAllModifiedRecord<OperatorDocument, SyncOperatorDocumentDto>(date);
                syncFullDb.Products = await GetAllModifiedRecord<Product, SyncProductDto>(date);
                syncFullDb.ProductDocuments = await GetAllModifiedRecord<ProductDocument, SyncProductDocumentDto>(date);
                syncFullDb.ProductTypes = await GetAllModifiedRecord<ProductType, SyncProductTypeDto>(date);
                syncFullDb.Suppliers = await GetAllModifiedRecord<Supplier, SyncSupplierDto>(date);
                syncFullDb.ActivityTypes = await GetAllActivityTypeModifiedRecord(date);
                syncFullDb.Interventions = await GetAllInterventionModifiedRecord(date);


                return syncFullDb;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private async Task<List<TDto>> InsertUpdateAllModified<TDto, TEntity>(List<TDto> tdoModels) where TEntity : FullAuditedEntity where TDto : SyncBaseDto
        {
            var repository = serviceProvider.GetRequiredService<IRepository<TEntity>>();
            List<TDto>  list = new List<TDto>();
            
            foreach (var model in tdoModels)
            {
                var entity = await
                    repository
                        .Query()
                        .Where(x=>x.Id == model.Id)
                        .SingleOrDefaultAsync();

                if (entity != null)
                {
                    model.MapTo(entity, mapper);
                    repository.Update(entity);
                    await dbContext.SaveChanges();
                }
                else
                { 
                    var newEntity = model.MapTo<TEntity>(mapper);
                    await repository.Insert(newEntity);
                    await dbContext.SaveChanges();
                    list.Add(newEntity.MapTo<TDto>(mapper));
                }
            }
            
            return list;
        }

        private async Task<List<SyncInterventionProductCheckListDto>> InsertUpdateAllModifiedInterventionProductCheckList(List<SyncInterventionProductCheckListDto> tdoModels,List<SyncInterventionProductCheckListItemDto> tdoChild) 
        {
            var repository = serviceProvider.GetRequiredService<IRepository<InterventionProductCheckList>>();
            List<SyncInterventionProductCheckListDto>  list = new List<SyncInterventionProductCheckListDto>();

            foreach (var model in tdoModels)
            {
                var entity = await
                    repository
                        .Query()
                        .Where(x=>x.Id == model.Id)
                        .SingleOrDefaultAsync();

                if (entity != null)
                {
                    model.MapTo(entity, mapper);
                    repository.Update(entity);
                    await dbContext.SaveChanges();
                }
                else
                { 
                    var oldId = model.Id;

                    var newEntity = model.MapTo<InterventionProductCheckList>(mapper);
                    await repository.Insert(newEntity);
                    await dbContext.SaveChanges();
                    list.Add(newEntity.MapTo<SyncInterventionProductCheckListDto>(mapper));

                    if (tdoChild != null)
                    {
                        tdoChild.ForEach(x=>x.CheckListId = (x.CheckListId == oldId?  newEntity.Id : x.CheckListId));
                    }
                }
            }
            return list;
        }

        private async Task<List<SyncTicketDto>> InsertUpdateAllModifiedTicket(List<SyncTicketDto> tdoModels,List<SyncTicketPictureDto> tdoChild) 
        {
            var repository = serviceProvider.GetRequiredService<IRepository<Ticket>>();
            var ticketsService = serviceProvider.GetRequiredService<ITicketsService>();
            List<SyncTicketDto>  list = new List<SyncTicketDto>();
            foreach (var model in tdoModels)
            {
                var entity = await
                    repository
                        .Query()
                        .Where(x=>x.Id == model.Id)
                        .SingleOrDefaultAsync();

                if (entity != null)
                {
                    
                    model.MapTo(entity, mapper);
                    repository.Update(entity);
                    await dbContext.SaveChanges();
                }
                else
                { 
                    var oldId = model.Id;
                    model.Number =  await ticketsService.GetNextNumber(model.Year.Value);
                    var newEntity = model.MapTo<Ticket>(mapper);
                    await repository.Insert(newEntity);
                    await dbContext.SaveChanges();
                    list.Add(newEntity.MapTo<SyncTicketDto>(mapper));
                    if (tdoChild != null)
                    {
                        tdoChild.ForEach(x=>x.TicketId = (x.TicketId == oldId?  newEntity.Id : x.TicketId));
                    }
                }
            }
            return list;
        }

        private async Task<List<SyncPurchaseOrderDto>> InsertUpdateAllModifiedPurchaseOrder(List<SyncPurchaseOrderDto> tdoModels,List<SyncPurchaseOrderItemDto> tdoChild) 
        {
            var repository = serviceProvider.GetRequiredService<IRepository<PurchaseOrder>>();
            List<SyncPurchaseOrderDto>  list = new List<SyncPurchaseOrderDto>();
            foreach (var model in tdoModels)
            {
                var entity = await
                    repository
                        .Query()
                        .Where(x=>x.Id == model.Id)
                        .SingleOrDefaultAsync();

                if (entity != null)
                {
                    model.MapTo(entity, mapper);
                    repository.Update(entity);
                    await dbContext.SaveChanges();
                }
                else
                { 
                    var oldId = model.Id;
                    var number = await GetNextNumber(entity.Year);
                    entity.Number = number;
                    var newEntity = model.MapTo<PurchaseOrder>(mapper);
                    await repository.Insert(newEntity);
                    await dbContext.SaveChanges();
                    list.Add(newEntity.MapTo<SyncPurchaseOrderDto>(mapper));
                    if (tdoChild != null)
                    {
                        tdoChild.ForEach(x=>x.PurchaseOrderId = (x.PurchaseOrderId == oldId?  newEntity.Id : x.PurchaseOrderId));
                    }
                }
            }
            return list;
        }

        private async Task<int> GetNextNumber(int year)
        {
            var repository = serviceProvider.GetRequiredService<IRepository<PurchaseOrder>>();
            var maxNumber = await repository.Query()
                .Where(e => e.Year == year)
                .Select(e => (int?)e.Number)
                .MaxAsync();

            return (maxNumber ?? 0) + 1;
        }
        private async Task<List<TDto>> GetAllModifiedRecord<TEntity, TDto>(DateTimeOffset date) where TEntity : FullAuditedEntity where TDto : SyncBaseDto
        {
            try
            {
                var repository = serviceProvider.GetRequiredService<IRepository<TEntity>>();

                var list = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                        repository.Query()
                            .AsNoTracking()
                            .Where(x => x.CreatedOn >= date || (x.EditedOn ?? DateTimeOffset.MinValue) >= date || (x.DeletedOn ?? DateTimeOffset.MinValue) >= date)
                            .ToListAsync()
                    , QueryFilter.SoftDelete);

                return list.MapTo<List<TDto>>(mapper);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<List<SyncInterventionDto>> GetAllInterventionModifiedRecord(DateTimeOffset date)
        {
            try
            {
                var repository = serviceProvider.GetRequiredService<IRepository<Intervention>>();

                var list = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                        repository.Query()
                            .AsNoTracking()
                            .Include(x => x.Operators)
                            .Where(x => x.CreatedOn >= date || (x.EditedOn ?? DateTimeOffset.MinValue) >= date || (x.DeletedOn ?? DateTimeOffset.MinValue) >= date)
                            .ToListAsync()
                    , QueryFilter.SoftDelete);
                var listDto = list.MapTo<List<SyncInterventionDto>>(mapper);

                return listDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private async Task<List<SyncActivityTypeDto>> GetAllActivityTypeModifiedRecord(DateTimeOffset date)
        {
            try
            {
                var repository = serviceProvider.GetRequiredService<IRepository<ActivityType>>();

                var list = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                        repository.Query()
                            .AsNoTracking()
                            .Include(x => x.Operators)
                            .Where(x => x.CreatedOn >= date || (x.EditedOn ?? DateTimeOffset.MinValue) >= date || (x.DeletedOn ?? DateTimeOffset.MinValue) >= date)
                            .ToListAsync()
                    , QueryFilter.SoftDelete);
                var listDto = list.MapTo<List<SyncActivityTypeDto>>(mapper);

                return listDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

    }

}