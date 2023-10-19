﻿using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.CheckLists.DTOs;
using Lacos.GestioneCommesse.Application.Sync;
using Lacos.GestioneCommesse.Contracts.Dtos;
using Lacos.GestioneCommesse.Contracts.Dtos.Application;
using Lacos.GestioneCommesse.Contracts.Dtos.Registry;
using Lacos.GestioneCommesse.Contracts.Dtos.Security;
using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers
{
    
    public class SyncController : LacosApiController
    {
        private readonly ISyncService service;

        public SyncController(ISyncService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        [HttpGet("Users")]
        public async Task<List<SyncUserDto>> SyncFromDBToApp_Users()
        {
            var result = await service.SyncFromDBToApp_Users();

            return result;
        }

        [AllowAnonymous]
        [HttpGet("Operators")]
        public async Task<List<SyncOperatorDto>> SyncFromDBToApp_Operators()
        {
            var result = await service.SyncFromDBToApp_Operators();

            return result;
        }

        [AllowAnonymous]
        [HttpGet("Vehicles")]
        public async Task<List<SyncVehicleDto>> SyncFromDBToApp_Vehicles()
        {
            var result = await service.SyncFromDBToApp_Vehicles();

            return result;
        }

        
        [AllowAnonymous]
        [HttpGet("Db/{strDate}")]
        public async Task<SyncFullDbDto> SyncFromDBToApp_FullDb(string strDate)
        {
            string sDate = Uri.UnescapeDataString(strDate);

            DateTimeOffset date = DateTimeOffset.Parse(sDate);

            var result = await service.SyncFromDBToApp_FullDb(date);

            return result;
        }

        [AllowAnonymous]
        [HttpPost("LocalDb")]
        public async Task SyncFromAppToDb_Changes([FromBody] SyncLocalDbChanges syncLocalDbChanges)
        {
            await service.SyncFromAppToDB_LocalChanges(syncLocalDbChanges);
            return;   
        }
    }
    
}
