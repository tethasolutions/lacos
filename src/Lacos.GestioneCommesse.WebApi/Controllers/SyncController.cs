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
        public async Task<SyncFullDbDto> SyncFromDBToApp_FullDb([FromBody] SyncDbDate dbDate)
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
        [HttpPost("LocalImage")]
        public async Task<IActionResult> SyncFromAppToDb_LocalImage([FromBody] SyncImageDto syncLocalImage)
        {
            try
            {
                await service.SyncFromAppToDB_LocalImage(syncLocalImage);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("RemoteImage")]
        public async Task<SyncImageDto> SyncFromAppToDb_RemoteImage([FromBody] SyncImageDto syncLocalImage)
        {
               return await service.SyncFromDBToApp_RemoteImage(syncLocalImage);
        }
    }
    
}
