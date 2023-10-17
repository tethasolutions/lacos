﻿using System;
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

        Task<SyncFullDbDto> SyncFromDBToApp_FullDb(DateTime date);

    }
}