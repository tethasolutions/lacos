# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Lacos is an enterprise work order management system (Gestione Commesse) built with:
- **Backend**: ASP.NET Core 8 Web API with Entity Framework Core 8, SQL Server
- **Frontend**: Angular 17 with Kendo UI components, Bootstrap 5
- **Language**: Italian localization (it-IT) primary

## Solution Structure

**Solution file**: `Lacos - Gestione Commesse.sln`

| Project | Role |
|---------|------|
| `Lacos.GestioneCommesse.Domain` | Entity definitions |
| `Lacos.GestioneCommesse.Framework` | Cross-cutting (session, auth, exceptions) |
| `Lacos.GestioneCommesse.Dal` | EF Core DbContext, repositories, migrations |
| `Lacos.GestioneCommesse.Contracts` | DTOs for API layer |
| `Lacos.GestioneCommesse.Application` | Business services by domain |
| `Lacos.GestioneCommesse.WebApi` | API controllers + Angular frontend in `Lacos/` |

No test projects or CI/CD pipelines exist.

## Build & Development Commands

### Backend (.NET)
```bash
dotnet build "Lacos - Gestione Commesse.sln"   # Build solution
# Or F5/Ctrl+Shift+B in Visual Studio 2022
# API runs on http://*:37998
```

### Frontend (Angular)
```bash
cd src/Lacos.GestioneCommesse.WebApi/Lacos

npm start                       # Dev server on port 4200
npm run release                 # Production build to ../wwwroot with base-href /lacos/
npm run extract-i18n           # Extract i18n strings to src/locale/
npm run kendo-translate        # Translate Kendo components to Italian
```

### Database
- SQL Server with EF Core Code-First migrations in `Lacos.GestioneCommesse.Dal/Migrations/`
- Connection string in `appsettings.json` → `ConnectionStrings:Default` (uses Integrated Security)
- Serilog writes to `EventLogs` table in `Logs` schema

## Architecture

**DI Registration Chain** (in `WebApiConfiguration.AddWebApi`):
`AddFramework` → `AddDal` → `AddApplication` → `AddMappings` (AutoMapper validated at startup)

**Middleware Pipeline**: DefaultFiles → StaticFiles → Routing → CORS → ExceptionHandler → Auth → Endpoints

**Error Handling**: `UnauthorizedException`→401, `NotFoundException`→404, `LacosException`→400, default→500

**JSON**: Newtonsoft.Json with `DateParseHandling.DateTimeOffset` and `DateTimeZoneHandling.RoundtripKind`

## Key Patterns

### Repository & Query Filters
```csharp
IRepository<TEntity>         // Full CRUD: Query(), Get(), Insert, Update, Delete
IViewRepository<TEntity>     // Read-only queries

// Query filters auto-applied by LacosDbContext:
QueryFilter.SoftDelete       // Excludes IsDeleted=true entities
QueryFilter.OperatorEntity   // Scopes by CurrentUser.OperatorId
// Override with ExecuteWithDisabledQueryFilters()
```

### Entity Inheritance
- `BaseEntity` - Id + IsTransient()
- `AuditedEntity` - CreatedBy/CreatedOn, EditedBy/EditedOn (auto-set)
- `FullAuditedEntity` - Adds DeletedBy/DeletedOn for soft delete
- `ISoftDelete` - Interface for soft-deletable entities
- `IOperatorEntity` - Interface for operator-scoped data

### Session Context
```csharp
ILacosSession.CurrentUser    // UserId, OperatorId, UserName, Roles
// Injected into DbContext, services, controllers for audit and filtering
```

### Authorization
- `AuthorizeFilter` checks `[RequireUser]` and `[RequireRole("RoleName")]` attributes
- Returns 401 if not authenticated/authorized
- Token-based auth via `AccessTokenProvider`

### API Conventions
- All controllers: `[ApiController] [Route("api/[controller]")]`
- Kendo grid binding: `[LacosDataSourceRequest]` → `DataSourceResult`
- Date handling: `DateTimeOffsetModelBinder` for timezone-aware dates

## Core Domain Entities

**Job Workflow**: Job → Activity → Intervention → InterventionProduct
- Jobs have status tracking (JobStatus, JobsProgressStatus)
- Activities represent scheduled work with products and attachments
- Interventions are actual work performed with notes, products, pictures, disputes

**Supporting Entities**: Customer, Operator, Vehicle, CheckList, PurchaseOrder, Message, Ticket

## Configuration

`appsettings.json`:
```json
{
  "Lacos": {
    "AllowCors": true,
    "CorsOrigins": "http://localhost:4200",
    "AttachmentsPath": "wwwroot\\attachments"
  },
  "ConnectionStrings": {
    "Default": "Server=localhost; Database=Lacos; ..."
  }
}
```

## Frontend Structure (Angular)

- **Routing**: Hash-based (`useHash: true`), `onSameUrlNavigation: 'reload'`, all routes guarded by `AuthGuard.asInjectableGuard` except login/logout
- **No lazy loading** — all components imported eagerly in `app.module.ts`
- **Interceptors**: `headers.interceptor.ts` (Bearer token), `response.interceptor.ts` (error handling), `loader.interceptor.ts` (loading spinner), `upload.interceptor.ts` (file uploads)
- **Services** in `src/app/services/` organized by domain, each wrapping HttpClient calls to `api/[controller]`
- **UI**: Kendo UI 15.5 (Grid, Scheduler, Dialog, DateInputs, Dropdowns) + Bootstrap 5 + ng-bootstrap
- **i18n**: Italian (it-IT) via XLIFF files in `src/locale/`
- **Code style**: 4-space indentation, single quotes for TypeScript (see `.editorconfig`)
