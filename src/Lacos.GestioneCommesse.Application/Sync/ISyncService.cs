using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.Contracts.Dtos;
using Lacos.GestioneCommesse.Contracts.Dtos.Application;
using Lacos.GestioneCommesse.Contracts.Dtos.Registry;
using Lacos.GestioneCommesse.Contracts.Dtos.Security;

namespace Lacos.GestioneCommesse.Application.Sync
{
    public interface ISyncService
    {
        Task<List<SyncUserDto>> SyncFromDBToApp_Users();

        Task<List<SyncOperatorDto>> SyncFromDBToApp_Operators();

        Task<List<SyncVehicleDto>> SyncFromDBToApp_Vehicles();

        Task<SyncRemoteFullDbDto> SyncFromDBToApp_FullDb(DateTimeOffset date);

        Task<SyncLocalDbChanges> SyncFromAppToDB_LocalChanges(SyncLocalDbChanges syncLocalDbChanges);

        Task SyncFromAppToDB_LocalDocument(SyncDocumentDto syncLocalImage);

        Task SyncFromAppToDB_SignIntervention(SyncSignDto signDto);

        Task<SyncDocumentDto> SyncFromDBToApp_RemoteDocument(SyncDocumentDto syncLocalImage);
        Task<SyncDocumentListDto> SyncFromDBToApp_RemoteDocumentList(SyncDocumentListDto syncDocumentListDto);
        Task SyncFromDBToApp_SyncronizedDocument(SyncDocumentSyncronizedDto syncDocumentSyncronizedDto);
        Task<SyncDocumentListDto>  SyncFromAppToDB_LocalDocumentList(SyncDocumentListDto syncDocumentListDto);
        Task<SyncCountersSyncDto> SyncFromDBToApp_CountersSyncronizedDocument(SyncCountersSyncDto syncCountersSyncDto);
    }
}
