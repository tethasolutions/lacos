using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Lacos.GestioneCommesse.Application.CheckList.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Telerik.SvgIcons;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class CheckListController : LacosApiController
{
    [HttpGet("checklist")]
    public async Task<DataSourceResult> GetCheckList([DataSourceRequest] DataSourceRequest request)
    {
        List<CheckListDto> checkList = new List<CheckListDto>
        {
            new CheckListDto
            {
                Id = 1,
                PictureFileName = "https://okna-everest.ru/assets/products/vhodnye-dveri/plastikovaya-vhodnaya-odnostvorchataya-dver-so-steklom-800x2200.jpg",
                Description = "Portoncino 1 anta con fiancoluce",
                ProductTypeId = 1,
                ProductType = new ProductTypeDto
                {
                    Id = 1,
                    Code = "prod_code_1",
                    Name = "Porta",
                    Description = "Descrizione porta",
                    IsReiDoor = false,
                    IsSparePart = false
                },
                ActivityTypeId = 1,
                ActivityType = new ActivityTypeDto
                {
                    Id = 1,
                    Name = "Posa",
                    PictureRequired = false
                },
                Items = new List<CheckListItemDto>
                {
                    new CheckListItemDto
                    {
                        Id = 1,
                        CheckListId = 1,
                        Description = "Sigillatura"
                    },
                    new CheckListItemDto
                    {
                        Id = 2,
                        CheckListId = 1,
                        Description = "Posa riempimento"
                    },
                    new CheckListItemDto
                    {
                        Id = 3,
                        CheckListId = 1,
                        Description = "Integrità prodotti"
                    }
                }
            },
            new CheckListDto
            {
                Id = 2,
                PictureFileName = "https://oknap.ru/upload/medialibrary/e9f/size-1.jpg",
                Description = "Portoncino 2 anta con fiancoluce",
                ProductTypeId = 2,
                ProductType = new ProductTypeDto
                {
                    Id = 2,
                    Code = "prod_code_2",
                    Name = "Porta 2",
                    Description = "Descrizione porta 2",
                    IsReiDoor = false,
                    IsSparePart = false
                },
                ActivityTypeId = 2,
                ActivityType = new ActivityTypeDto
                {
                    Id = 2,
                    Name = "Posa 2",
                    PictureRequired = false
                },
                Items = new List<CheckListItemDto>
                {
                    new CheckListItemDto
                    {
                        Id = 4,
                        CheckListId = 2,
                        Description = "Sigillatura"
                    },
                    new CheckListItemDto
                    {
                        Id = 5,
                        CheckListId = 2,
                        Description = "Posa riempimento"
                    },
                    new CheckListItemDto
                    {
                        Id = 6,
                        CheckListId = 2,
                        Description = "Integrità prodotti"
                    }
                }
            },
            new CheckListDto
            {
                Id = 3,
                PictureFileName = "https://www.okno-moskva.ru/files/647/1-1.jpg",
                Description = "Portoncino 3 anta con fiancoluce",
                ProductTypeId = 3,
                ProductType = new ProductTypeDto
                {
                    Id = 3,
                    Code = "prod_code_3",
                    Name = "Porta 3",
                    Description = "Descrizione porta 3",
                    IsReiDoor = false,
                    IsSparePart = false
                },
                ActivityTypeId = 3,
                ActivityType = new ActivityTypeDto
                {
                    Id = 3,
                    Name = "Posa 3",
                    PictureRequired = false
                },
                Items = new List<CheckListItemDto>
                {
                    new CheckListItemDto
                    {
                        Id = 7,
                        CheckListId = 3,
                        Description = "Sigillatura"
                    },
                    new CheckListItemDto
                    {
                        Id = 8,
                        CheckListId = 3,
                        Description = "Posa riempimento"
                    },
                    new CheckListItemDto
                    {
                        Id = 9,
                        CheckListId = 3,
                        Description = "Integrità prodotti"
                    }
                }
            }
        };

        DataSourceResult result = new DataSourceResult
        {
            AggregateResults = null,
            Errors = null,
            Total = 3,
            Data = checkList
        };

        return result;
    }

    [HttpGet("checklist-detail/{checklistId}")]
    public async Task<CheckListDto> GetCheckListDetail(long checklistId)
    {
        CheckListDto checkListDetail = new CheckListDto
        {
            Id = 2,
            PictureFileName = "https://okna-everest.ru/assets/products/vhodnye-dveri/plastikovaya-vhodnaya-odnostvorchataya-dver-so-steklom-800x2200.jpg",
            Description = "Portoncino 2 anta con fiancoluce",
            ProductTypeId = 2,
            ProductType = new ProductTypeDto
            {
                Id = 2,
                Code = "prod_code_2",
                Name = "Porta 2",
                Description = "Descrizione porta 2",
                IsReiDoor = false,
                IsSparePart = false
            },
            ActivityTypeId = 2,
            ActivityType = new ActivityTypeDto
            {
                Id = 2,
                Name = "Posa 2",
                PictureRequired = false
            },
            Items = new List<CheckListItemDto>
            {
                new CheckListItemDto
                {
                    Id = 4,
                    CheckListId = 2,
                    Description = "Sigillatura"
                },
                new CheckListItemDto
                {
                    Id = 5,
                    CheckListId = 2,
                    Description = "Posa riempimento"
                },
                new CheckListItemDto
                {
                    Id = 6,
                    CheckListId = 2,
                    Description = "Integrità prodotti"
                }
            }
        };

        return checkListDetail;
    }

    [HttpPut("checklist/{id}")]
    public async Task<IActionResult> UpdateCheckList(long id)
    {
        // CheckListDto request from form
        var files = HttpContext.Request.Form.Files;
        var provider = new MultipartMemoryStreamProvider();
        foreach (var file in provider.Contents)
        {
            var buffer = await file.ReadAsByteArrayAsync();
        }
        return NoContent();
    }

    [HttpPost("checklist")]
    public async Task<IActionResult> CreateCheckList()
    {
        // CheckListDto request from form
        var files = HttpContext.Request.Form.Files;
        var provider = new MultipartMemoryStreamProvider();
        foreach (var file in provider.Contents)
        {
            var buffer = await file.ReadAsByteArrayAsync();
        }
        return Ok(2);
    }

    [HttpDelete("checklist/{id}")]
    public async Task<IActionResult> DeleteCheckList(long id)
    {
        return Ok();
    }

    [HttpGet("product-types")]
    public async Task<List<ProductTypeDto>> GetProductTypes()
    {
        List<ProductTypeDto> productTypes = new List<ProductTypeDto>
        {
            new ProductTypeDto
            {
                Id = 1,
                Code = "prod_code_1",
                Name = "Porta",
                Description = "Descrizione porta",
                IsReiDoor = false,
                IsSparePart = false
            },
            new ProductTypeDto
            {
                Id = 2,
                Code = "prod_code_2",
                Name = "Porta 2",
                Description = "Descrizione porta 2",
                IsReiDoor = false,
                IsSparePart = false
            },
            new ProductTypeDto
            {
                Id = 3,
                Code = "prod_code_3",
                Name = "Porta 3",
                Description = "Descrizione porta 3",
                IsReiDoor = false,
                IsSparePart = false
            }
        };

        return productTypes;
    }

    [HttpGet("activity-types")]
    public async Task<List<ActivityTypeDto>> GetActivityTypes()
    {
        List<ActivityTypeDto> activityTypes = new List<ActivityTypeDto>
        {
            new ActivityTypeDto
            {
                Id = 1,
                Name = "Posa",
                PictureRequired = false
            },
            new ActivityTypeDto
            {
                Id = 2,
                Name = "Posa 2",
                PictureRequired = false
            },
            new ActivityTypeDto
            {
                Id = 3,
                Name = "Posa 3",
                PictureRequired = false
            }
        };

        return activityTypes;
    }

    [HttpGet("checklist-items/{checklistId}")]
    public async Task<List<CheckListItemDto>> GetCheckListItems(long checklistId)
    {
        List<CheckListItemDto> checkListItems = new List<CheckListItemDto>
        {
            new CheckListItemDto
            {
                Id = 4,
                CheckListId = 2,
                Description = "Sigillatura"
            },
            new CheckListItemDto
            {
                Id = 5,
                CheckListId = 2,
                Description = "Posa riempimento"
            },
            new CheckListItemDto
            {
                Id = 6,
                CheckListId = 2,
                Description = "Integrità prodotti"
            }
        };

        return checkListItems;
    }

    [HttpGet("checklist-item-detail/{checklistItemId}")]
    public async Task<CheckListItemDto> GetCheckListItemDetail(long checkListItemId)
    {
        CheckListItemDto checkListItemDetail = new CheckListItemDto
        {
            Id = 4,
            CheckListId = 2,
            Description = "Sigillatura"
        };

        return checkListItemDetail;
    }

    [HttpPut("checklist-item/{id}")]
    public async Task<IActionResult> UpdateCheckListItem(long id, [FromBody] CheckListItemDto request)
    {
        return NoContent();
    }

    [HttpPost("checklist-item")]
    public async Task<IActionResult> CreateCheckListItem([FromBody] CheckListItemDto request)
    {
        return Ok(2);
    }

    [HttpDelete("checklist-item/{id}")]
    public async Task<IActionResult> DeleteCheckListItem(long id)
    {
        return Ok();
    }
}
