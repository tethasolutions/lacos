using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using System.Net.Http;
using System.Web.Http;
using Lacos.GestioneCommesse.Application.CheckLists.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Telerik.SvgIcons;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.CheckLists.Services;
using Lacos.GestioneCommesse.Application.Operators.Services;
using Microsoft.CodeAnalysis.Editing;
using Lacos.GestioneCommesse.Application.Docs.DTOs;

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
    public async Task<IActionResult> CreateCheckList( [FromBody] CheckListDto checkListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await checklistService.CreateCheckList(checkListDto);
        return Ok(checkListDto);
    }

    [HttpDelete("checklist/{id}")]
    public async Task<IActionResult> DeleteCheckList(long id)
    {
        await checklistService.DeleteCheckList(id);

        return Ok();
    }

    [HttpGet("checklist-items/{checklistId}")]
    public async Task<List<CheckListItemDto>> GetCheckListItems(long checklistId)
    {
        var checkListItems = (await checklistService.GetCheckListItems(checklistId)).ToList();
        return checkListItems;
    }

    [HttpGet("checklist-item-detail/{checklistItemId}")]
    public async Task<CheckListItemDto> GetCheckListItemDetail(long checkListItemId)
    {
        var checkListItemDetail = await checklistService.GetCheckListItemDetail(checkListItemId);
        return checkListItemDetail;
    }

    [HttpPut("checklist-item/{id}")]
    public async Task<IActionResult> UpdateCheckListItem(long id, [FromBody] CheckListItemDto checkListItemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await checklistService.UpdateCheckListItem(id, checkListItemDto);
        return Ok();
    }

    [HttpPost("checklist-item")]
    public async Task<IActionResult> CreateCheckListItem([FromBody] CheckListItemDto checkListItemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await checklistService.CreateCheckListItem(checkListItemDto);
        return Ok(checkListItemDto);
    }

    [HttpDelete("checklist-item/{id}")]
    public async Task<IActionResult> DeleteCheckListItem(long id)
    {
      
        await checklistService.DeleteCheckListItem(id);

        return Ok();
    }

    [HttpPost("copy-checklist")]
    public Task<CheckListDto> CopyChecklist(ChecklistCopyDto checklistCopyDto)
    {
        return checklistService.CopyChecklist(checklistCopyDto);
    }
}
