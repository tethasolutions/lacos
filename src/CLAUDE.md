# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Lacos is an enterprise work order management system (Gestione Commesse) built with:
- **Backend**: ASP.NET Core 8 Web API with Entity Framework Core 8, SQL Server
- **Frontend**: Angular 17 with Kendo UI components, Bootstrap 5
- **Language**: Italian localization (it-IT) primary

## Build & Development Commands

### Backend (.NET)
```bash
dotnet build                    # Build solution
# Or F5/Ctrl+Shift+B in Visual Studio 2022
# API runs on http://*:37998
```

### Frontend (Angular)
```bash
cd src/Lacos.GestioneCommesse.WebApi/Lacos

npm start                       # Dev server on port 4200
npm run release                 # Production build to ../wwwroot
npm run extract-i18n           # Extract i18n strings
npm run kendo-translate        # Translate Kendo components to Italian
```

### Database
- EF Core Code-First migrations in `Lacos.GestioneCommesse.Dal/Migrations/`
- Connection string in `appsettings.json` → `ConnectionStrings:Default`

## Architecture

**6-Layer Clean Architecture:**

```
Domain/           → Entity definitions (BaseEntity, AuditedEntity, FullAuditedEntity)
                    Subfolders: Application/ (jobs, interventions), Registry/ (master data), Security/

Framework/        → Cross-cutting: ILacosSession, exceptions, password hashing, token generation

Dal/              → Data access: IRepository<T>, LacosDbContext, migrations
                    Query filters: SoftDelete (auto-excludes deleted), OperatorEntity (tenant scoping)

Contracts/        → DTOs for API layer

Application/      → Business services organized by domain (Customers/, Interventions/, Jobs/, etc.)
                    Each has: Services/, DTOs/, Mappings/ (AutoMapper profiles)

WebApi/           → Controllers inherit LacosApiController, routes at api/[controller]
                    Auth via AuthorizeFilter + [RequireUser]/[RequireRole] attributes
                    Contains Angular app in Lacos/ subfolder
```

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

- Feature modules per domain with lazy loading
- HTTP interceptors for auth tokens, error handling, loading indicators
- Services communicate with API via HttpClient
- Kendo components for grids, forms, dialogs
