using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Lacos.GestioneCommesse.Application.CheckList.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Telerik.SvgIcons;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.CheckList;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class CheckListController : LacosApiController
{

    private readonly ICheckListService checklistService;
    public CheckListController(ICheckListService checklistService)
    {
        this.checklistService = checklistService;
    }

    [HttpGet("checklist")]
    public async Task<DataSourceResult> GetCheckList([DataSourceRequest] DataSourceRequest request)
    {
        var quotations = (checklistService.GetCheckList());
        return await quotations.ToDataSourceResultAsync(request);
    }

    [HttpGet("checklist-detail/{checklistId}")]
    public async Task<CheckListDto> GetCheckListDetail(long checklistId)
    {
        var quotation = await checklistService.GetCheckListDetail(checklistId);
        return quotation;
    }

    [HttpPut("checklist/{id}")]
    public async Task<IActionResult> UpdateCheckList(long id, [FromBody] CheckListDto checkListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await checklistService.UpdateCheckList(id, checkListDto);
        return Ok();
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
