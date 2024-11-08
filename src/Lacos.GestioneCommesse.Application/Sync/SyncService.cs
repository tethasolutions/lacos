using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
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
using Lacos.GestioneCommesse.Domain.Application;
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

    public class DocumentItem
    {
        public string DocumentName { get; set; }
        public int Order { get; set; }
    }


    public class SyncService : ISyncService
    {
        private readonly IMapper mapper;
        private readonly IServiceProvider serviceProvider;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosConfiguration configuration;
        private readonly IInterventionsService interventionsService;
        


        public SyncService(
            IMapper mapper,
           IServiceProvider serviceProvider, ILacosDbContext dbContext, ILacosConfiguration configuration, IInterventionsService interventionsService)
        {
            this.mapper = mapper;
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.interventionsService = interventionsService;
        }

        public async Task<SyncCountersSyncDto> SyncFromDBToApp_CountersSyncronizedDocument(SyncCountersSyncDto syncCountersSyncDto)
        {
            var documentToSyncQueueRepository = serviceProvider.GetRequiredService<IRepository<DocumentToSyncQueue>>();

            var TotalDocuments = documentToSyncQueueRepository
                                           .Query()
                                           .AsNoTracking()
                                           .Where(x => x.DeviceGuid == syncCountersSyncDto.DeviceGuid);

            var SyncronizedDocuments = TotalDocuments
                .Where(x => x.IsSyncronized);

            var DocumentsInError = TotalDocuments
                .Where(x => x.IsInError);

            syncCountersSyncDto.ErrorDocuments = DocumentsInError.Count();
            syncCountersSyncDto.TotalDocuments = TotalDocuments.Count();
            syncCountersSyncDto.SyncrnizedDocuments = SyncronizedDocuments.Count();

            return syncCountersSyncDto;
        }


        public async Task SyncFromDBToApp_SyncronizedDocument(SyncDocumentSyncronizedDto syncDocumentSyncronizedDto)
        {
            var documentToSyncQueueRepository = serviceProvider.GetRequiredService<IRepository<DocumentToSyncQueue>>();

            var entity = documentToSyncQueueRepository
                .Query()
                .AsNoTracking()
                .SingleOrDefault(x=>x.DocumentName == syncDocumentSyncronizedDto.DocumentName && x.DeviceGuid == syncDocumentSyncronizedDto.DeviceGuid);

            if (entity != null)
            {
                entity.IsSyncronized = true;
                entity.IsInError = false;
                documentToSyncQueueRepository.Update(entity);
                await dbContext.SaveChanges();
            }
            else
            {
                throw new NotFoundException("Documento non presente nel database");
            }
        }

        public async Task<SyncDocumentListDto> SyncFromDBToApp_RemoteDocumentList(SyncDocumentListDto syncDocumentListDto)
        {
            var documentToSyncQueueRepository = serviceProvider.GetRequiredService<IRepository<DocumentToSyncQueue>>();

            await CreateUpdateDocumentsToSync(syncDocumentListDto.DeviceGuid,syncDocumentListDto.OperatorId);
            
            List<string> documentsNames = await documentToSyncQueueRepository
                .Query()
                .Where(x => !x.IsSyncronized && x.DeviceGuid == syncDocumentListDto.DeviceGuid)
                .OrderBy(x=>x.Order)
                .Select(x => x.DocumentName)
                
                .ToListAsync();

            syncDocumentListDto.DocumentNames = documentsNames;
            return syncDocumentListDto;
        }

        public async Task<SyncDocumentListDto> SyncFromAppToDB_LocalDocumentList(SyncDocumentListDto syncDocumentListDto)
        {
            var folder = configuration.AttachmentsPath;
            if (Directory.Exists(folder) && !(syncDocumentListDto.DocumentNames is null) && syncDocumentListDto.DocumentNames.Count > 0)
            {
                List<string> fileList = Directory.EnumerateFiles(folder, "*.*").ToList();
                List<string> fileNotSyncList = syncDocumentListDto.DocumentNames.Except(fileList.Select(Path.GetFileName)).ToList();
                syncDocumentListDto.DocumentNames = fileNotSyncList;
            }
            return syncDocumentListDto;
        }

        private async Task CreateUpdateDocumentsToSync(string deviceGuid, long operatorId)
        {
            //var result = dbContext.ExecuteStoredProcedure<string>("usp_removeDuplicateRecord_SyncDocument");

            var documentToSyncQueueRepository = serviceProvider.GetRequiredService<IRepository<DocumentToSyncQueue>>();
            List<string> oldDocumentsToSyncList = await documentToSyncQueueRepository
                                                  .Query()
                                                  .Where(x => x.DeviceGuid == deviceGuid)
                                                  .Select(x => x.DocumentName)
                                                  .ToListAsync();
            

            List<DocumentItem> newDocumentsToSyncList = new List<DocumentItem>();

            //---------------------------------------------------------------------------------
            var operatorDocumentRepository = serviceProvider.GetRequiredService<IRepository<OperatorDocument>>();

            var operatorDocumentList = await operatorDocumentRepository
                .Query()
                .Where(x=>x.OperatorId == operatorId)
                .Where(x=>!string.IsNullOrEmpty(x.FileName))
                .Select(x => new DocumentItem {DocumentName = x.FileName ?? "", Order = 1} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(operatorDocumentList);
            //--------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------


            var dateTimeNow = DateTimeOffset.Now.AddDays(-14);
            var dateTimeStart = new DateTimeOffset(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 0, 0, 0, dateTimeNow.Offset);
            
            var interventionListRepository = serviceProvider.GetRequiredService<IRepository<Intervention>>();
            var activityIdList = await interventionListRepository
                    .Query()
                    .Where(x=>x.Start >= dateTimeStart || x.Status == InterventionStatus.Scheduled)
                    .Select(x=>x.ActivityId)
                    .ToListAsync();
            
            var activityAttachmentRepository = serviceProvider.GetRequiredService<IRepository<ActivityAttachment>>();

            var activityAttachmentList = await activityAttachmentRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.FileName))
                .Where(x=>activityIdList.Contains(x.ActivityId))
                .OrderByDescending(x=>x.Activity.StartDate)
                .Select(x => new DocumentItem {DocumentName = x.FileName ?? "", Order = 2} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(activityAttachmentList);
            //--------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------
            var interventionNoteRepository = serviceProvider.GetRequiredService<IRepository<InterventionNote>>();

            var interventionNoteList = await interventionNoteRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.PictureFileName))
                .Select(x => new DocumentItem {DocumentName = x.PictureFileName ?? "", Order = 3} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(interventionNoteList);
            //--------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------
            var productRepository = serviceProvider.GetRequiredService<IRepository<Product>>();

            var productList = await productRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.PictureFileName))
                .Select(x => new DocumentItem {DocumentName = x.PictureFileName ?? "", Order = 4} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(productList);
            //--------------------------------------------------------------------------------
            
            //---------------------------------------------------------------------------------
            var productDocumentRepository = serviceProvider.GetRequiredService<IRepository<ProductDocument>>();

            var productDocumentList = await productDocumentRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.OriginalFilename))
                .Select(x => new DocumentItem {DocumentName = x.OriginalFilename ?? "", Order = 5} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(productDocumentList);
            //--------------------------------------------------------------------------------
            
            //---------------------------------------------------------------------------------
            var ticketPictureRepository = serviceProvider.GetRequiredService<IRepository<TicketPicture>>();

            var ticketPictureList = await ticketPictureRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.FileName))
                .Select(x => new DocumentItem {DocumentName = x.FileName ?? "", Order = 6} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(ticketPictureList);
            //--------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------
            var interventionRepository = serviceProvider.GetRequiredService<IRepository<Intervention>>();

            var interventionList = await interventionRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.CustomerSignatureFileName))
                .Select(x => new DocumentItem {DocumentName = x.CustomerSignatureFileName ?? "", Order = 7} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(interventionList);
            //--------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------
            var checkListRepository = serviceProvider.GetRequiredService<IRepository<CheckList>>();

            var checkListList = await checkListRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.PictureFileName))
                .Select(x => new DocumentItem {DocumentName = x.PictureFileName ?? "", Order = 8} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(checkListList);
            //--------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------
            var interventionProductCheckListItemRepository = serviceProvider.GetRequiredService<IRepository<InterventionProductCheckListItem>>();

            var interventionProductCheckListItemList = await interventionProductCheckListItemRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.AttachmentFileName))
                .Select(x => new DocumentItem {DocumentName = x.AttachmentFileName ?? "", Order = 9} )
                .ToListAsync();

            newDocumentsToSyncList.AddRange(interventionProductCheckListItemList);
            //--------------------------------------------------------------------------------
            
            //---------------------------------------------------------------------------------
            var ticketRepository = serviceProvider.GetRequiredService<IRepository<Ticket>>();

            var ticketSignList = await ticketRepository
                .Query()
                .Where(x=>!string.IsNullOrEmpty(x.CustomerSignatureFileName))
                .Select(x => new DocumentItem {DocumentName = x.CustomerSignatureFileName ?? "", Order = 10} )
                .Distinct()
                .ToListAsync();

            newDocumentsToSyncList.AddRange(ticketSignList);
            //--------------------------------------------------------------------------------
            
            
            var list = newDocumentsToSyncList.Where(x => !oldDocumentsToSyncList.Contains(x.DocumentName));

            foreach (var documentItem in list)
            {
                var entity = new DocumentToSyncQueue(){DeviceGuid = deviceGuid, DocumentName = documentItem.DocumentName, Order = documentItem.Order, IsSyncronized = false};
                await documentToSyncQueueRepository.Insert(entity);
            }

            await dbContext.SaveChanges();
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
                    syncLocalDbChangesRemote.Interventions = await InsertUpdateAllModifiedInterventions(syncLocalDbChanges.Interventions);
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
         public async Task SyncFromAppToDB_LocalDocument(SyncDocumentDto syncLocalDocument)
        {
            try
            {
                    await using Stream streamOriginalDocument = new MemoryStream(syncLocalDocument.Content);
                    var folder = configuration.AttachmentsPath;
                    Directory.CreateDirectory(folder);
                    var path = Path.Combine(folder, syncLocalDocument.DocumentName);
                    if (!File.Exists(path))
                    {
                        await using Stream stremNewDocument = File.Create(path);
                        await streamOriginalDocument.CopyToAsync(stremNewDocument);
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
                 entity.ToBeReschedule = signDto.ToBeReschedule;
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


         public async Task<SyncDocumentDto> SyncFromDBToApp_RemoteDocument(SyncDocumentDto syncLocalDocument)
         {
             try
             {
                 var folder = configuration.AttachmentsPath;
                 var path = Path.Combine(folder, syncLocalDocument.DocumentName);
                 
                 if (File.Exists(path))
                 {
                     syncLocalDocument.Content = await File.ReadAllBytesAsync(path);
                    return syncLocalDocument;
                 }


                 var documentToSyncQueueRepository = serviceProvider.GetRequiredService<IRepository<DocumentToSyncQueue>>();

                 var entity = documentToSyncQueueRepository
                     .Query()
                     .AsNoTracking()
                     .SingleOrDefault(x=>x.DocumentName == syncLocalDocument.DocumentName && x.DeviceGuid == syncLocalDocument.DeviceGuid);

                 if (entity != null)
                 {
                     entity.IsInError = true;
                     documentToSyncQueueRepository.Update(entity);
                     await dbContext.SaveChanges();
                 }
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

        private async Task<List<SyncInterventionDto>> InsertUpdateAllModifiedInterventions(List<SyncInterventionDto> tdoModels)
        {
            var repository = serviceProvider.GetRequiredService<IRepository<Intervention>>();
            List<SyncInterventionDto>  list = new List<SyncInterventionDto>();
            
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
                    var newEntity = model.MapTo<Intervention>(mapper);
                    await repository.Insert(newEntity);
                    await dbContext.SaveChanges();
                    list.Add(newEntity.MapTo<SyncInterventionDto>(mapper));
                }
                await interventionsService.UpdateActivityStatus(model.ActivityId.Value);
            }
            return list;
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

                    //aggiunta commento su inserimento note intervento
                    if (typeof(TEntity) == typeof(InterventionNote) && newEntity != null) {
                        
                        
                        var messageRepository = serviceProvider.GetRequiredService<IRepository<Message>>();
                        var interventionNoteRepository = serviceProvider.GetRequiredService<IRepository<InterventionNote>>();
                        Message message = new Message();
                        
                        var dbEntity = await interventionNoteRepository
                                .Query()
                                .AsNoTracking()
                                .Include(x=>x.Intervention)
                                .SingleOrDefaultAsync(x=>x.Id == newEntity.Id);
                        
                        InterventionNote interventionNote = dbEntity as InterventionNote;
                        message.OperatorId = interventionNote.OperatorId;
                        message.ActivityId = interventionNote.Intervention.ActivityId;
                        message.Note = interventionNote.Notes;
                        message.Date = interventionNote.CreatedOn;
                        await messageRepository.Insert(message);
                        await dbContext.SaveChanges();

                        try
                        {
                            var notificationOperatorRepository =
                                serviceProvider.GetRequiredService<IRepository<NotificationOperator>>();
                            var messageNotificationRepository =
                                serviceProvider.GetRequiredService<IRepository<MessageNotification>>();
                            var notificationOperators = await notificationOperatorRepository.Query()
                                .AsNoTracking()
                                .ToListAsync();
                            foreach (var notificationOperator in notificationOperators)
                            {
                                MessageNotification notification = new MessageNotification();
                                notification.MessageId = message.Id;
                                notification.OperatorId = (long) notificationOperator.OperatorId;
                                notification.IsRead = false;
                                await messageNotificationRepository.Insert(notification);

                            }

                            await dbContext.SaveChanges();
                        }
                        catch (Exception)
                        {
                        }
                    }
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
                    var number = await GetNextNumber(model.Year.Value);
                    var newEntity = model.MapTo<PurchaseOrder>(mapper);
                    newEntity.Number = number;
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