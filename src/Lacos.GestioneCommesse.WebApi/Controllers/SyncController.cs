using Kendo.Mvc.UI;
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
        [HttpPost("Db")]
        public async Task<SyncRemoteFullDbDto> SyncFromDBToApp_FullDb([FromBody] SyncDbDate dbDate)
        {
            var result = await service.SyncFromDBToApp_FullDb(dbDate.Date);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("LocalDb")]
        public async Task<SyncLocalDbChanges> SyncFromAppToDb_Changes([FromBody] SyncLocalDbChanges syncLocalDbChanges)
        {
            return await service.SyncFromAppToDB_LocalChanges(syncLocalDbChanges);
        }

        [AllowAnonymous]
        [HttpPost("Documents/Remote/List")]
        public async Task<SyncDocumentListDto> SyncFromDBToApp_RemoteDocumentList([FromBody] SyncDocumentListDto syncDocumentListDto)
        {
              return  await service.SyncFromDBToApp_RemoteDocumentList(syncDocumentListDto);
        }

        [AllowAnonymous]
        [HttpPost("Documents/IsSyncronized")]
        public async Task<IActionResult> SyncFromAppToDb_IsSyncronized([FromBody] SyncDocumentSyncronizedDto syncDocumentSyncronizedDto)
        {
            try
            {
                await service.SyncFromDBToApp_SyncronizedDocument(syncDocumentSyncronizedDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("Documents/Local/List")]
        public async Task<SyncDocumentListDto> SyncFromAppToDb_LocalDocumentList([FromBody] SyncDocumentListDto syncDocumentListDto)
        {
                 return  await service.SyncFromAppToDB_LocalDocumentList(syncDocumentListDto);
        }

        [AllowAnonymous]
        [HttpPost("Documents/Local")]
        public async Task<IActionResult> SyncFromAppToDb_LocalDocument([FromBody] SyncDocumentDto syncLocalDocument)
        {
            try
            {
                await service.SyncFromAppToDB_LocalDocument(syncLocalDocument);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("Documents/Remote")]
        public async Task<SyncDocumentDto> SyncFromAppToDb_RemoteImage([FromBody] SyncDocumentDto syncLocalDocument)
        {
               return await service.SyncFromDBToApp_RemoteDocument(syncLocalDocument);
        }


        [AllowAnonymous]
        [HttpPost("SignIntervention")]
        public async Task<IActionResult> SyncFromAppToDB_SignIntervention([FromBody] SyncSignDto signDto)
        {
            try
            {
                await service.SyncFromAppToDB_SignIntervention(signDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
           
        }
    }
    
}
