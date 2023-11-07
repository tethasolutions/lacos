﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
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


        public SyncService(
            IMapper mapper,
           IServiceProvider serviceProvider, ILacosDbContext dbContext)
        {
            this.mapper = mapper;
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
        }
        
        public async Task<List<SyncUserDto>> SyncFromDBToApp_Users()
        {
            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            

            var users = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                userRepository.Query()
                    .Where(x => x.Role == Role.Operator)
                    .Include(x => x.Operator)
                    .ToListAsync()
            , QueryFilter.SoftDelete);;

            var ret = users.MapTo<List<SyncUserDto>>(mapper);
            return ret;
        }

        public async Task<List<SyncOperatorDto>> SyncFromDBToApp_Operators()
        {
            var operatorRepository = serviceProvider.GetRequiredService<IRepository<Operator>>();

            var operators = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                    operatorRepository.Query()
                        .Include(x=>x.User)
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

        public async Task<SyncFullDbDto>  SyncFromDBToApp_FullDb(DateTime date)
        {
            try
            {
                SyncFullDbDto syncFullDb = new SyncFullDbDto();

            syncFullDb.Activities = await GetAllModifiedRecord<Activity, SyncActivityDto>(date);
            syncFullDb.ActivityProducts = await GetAllModifiedRecord<ActivityProduct, SyncActivityProductDto>(date);
            syncFullDb.Interventions = await GetAllModifiedRecord<Intervention, SyncInterventionDto>(date);
            syncFullDb.InterventionDisputes = await GetAllModifiedRecord<InterventionDispute, SyncInterventionDisputeDto>(date);
            syncFullDb.InterventionNotes = await GetAllModifiedRecord<InterventionNote, SyncInterventionNoteDto>(date);
            syncFullDb.InterventionProducts = await GetAllModifiedRecord<InterventionProduct, SyncInterventionProductDto>(date);
            syncFullDb.InterventionProductCheckLists = await GetAllModifiedRecord<InterventionProductCheckList, SyncInterventionProductCheckListDto>(date);
            syncFullDb.InterventionProductCheckListItems = await GetAllModifiedRecord<InterventionProductCheckListItem, SyncInterventionProductCheckListItemDto>(date);
            syncFullDb.InterventionProductPictures = await GetAllModifiedRecord<InterventionProductPicture, SyncInterventionProductPictureDto>(date);
            syncFullDb.Jobs = await GetAllModifiedRecord<Job, SyncJobDto>(date);
            syncFullDb.PurchaseOrders = await GetAllModifiedRecord<PurchaseOrder, SyncPurchaseOrderDto>(date);
            syncFullDb.PurchaseOrderItems = await GetAllModifiedRecord<PurchaseOrderItem, SyncPurchaseOrderItemDto>(date);

            //syncFullDb.Tickets = await GetAllModifiedRecord<Ticket, SyncTicketDto>(date);
            syncFullDb.Tickets = new List<SyncTicketDto>();
                
            syncFullDb.TicketPictures = await GetAllModifiedRecord<TicketPicture, SyncTicketPictureDto>(date);
            syncFullDb.ActivityTypes = await GetAllModifiedRecord<ActivityType, SyncActivityTypeDto>(date);
            syncFullDb.CheckLists = await GetAllModifiedRecord<CheckList, SyncCheckListDto>(date);
            syncFullDb.CheckListItems = await GetAllModifiedRecord<CheckListItem, SyncCheckListItemDto>(date);
            syncFullDb.Customers = await GetAllModifiedRecord<Customer, SyncCustomerDto>(date);
            syncFullDb.Addresses = await GetAllModifiedRecord<Address, SyncAddressDto>(date);
            syncFullDb.OperatorDocuments = await GetAllModifiedRecord<OperatorDocument, SyncOperatorDocumentDto>(date);
            syncFullDb.Products = await GetAllModifiedRecord<Product, SyncProductDto>(date);
            syncFullDb.ProductDocuments = await GetAllModifiedRecord<ProductDocument, SyncProductDocumentDto>(date);
            syncFullDb.ProductTypes= await GetAllModifiedRecord<ProductType, SyncProductTypeDto>(date);
            syncFullDb.Vehicles = await GetAllModifiedRecord<Vehicle, SyncVehicleDto>(date);
            return syncFullDb;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            
        }

        private async Task<List<TDto>> GetAllModifiedRecord<TRepository,TDto>(DateTime date) where  TRepository:FullAuditedEntity where TDto:SyncBaseDto
        {
            var repository = serviceProvider.GetRequiredService<IRepository<TRepository>>();
            DateTimeOffset dto = new DateTimeOffset(date,TimeSpan.Zero);

            var list = await dbContext.ExecuteWithDisabledQueryFilters(() =>
                    repository.Query()
                        .AsNoTracking()
                        .Where(x=>x.CreatedOn > dto || x.EditedOn.Value > dto || x.DeletedOn.Value > dto)
                        .ToListAsync()
                , QueryFilter.SoftDelete);

            return list.MapTo<List<TDto>>(mapper);
        }

    }

}