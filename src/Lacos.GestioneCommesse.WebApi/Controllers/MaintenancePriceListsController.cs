using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Application.MaintenancePriceLists.Services;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class MaintenancePriceListsController : LacosApiController
{

    private readonly IMaintenancePriceListService maintenancePricelistService;
    public MaintenancePriceListsController(IMaintenancePriceListService maintenancePricelistService)
    {
        this.maintenancePricelistService = maintenancePricelistService;
    }

    [HttpGet("maintenancePricelist")]
    public async Task<DataSourceResult> GetMaintenancePriceList([DataSourceRequest] DataSourceRequest request)
    {
        var quotations = (maintenancePricelistService.GetMaintenancePriceList());
        return await quotations.ToDataSourceResultAsync(request);
    }

    [HttpGet("maintenancePricelist-detail/{maintenancePricelistId}")]
    public async Task<MaintenancePriceListDto> GetMaintenancePriceListDetail(long maintenancePricelistId)
    {
        var quotation = await maintenancePricelistService.GetMaintenancePriceListDetail(maintenancePricelistId);
        return quotation;
    }

    [HttpPut("maintenancePricelist/{id}")]
    public async Task<IActionResult> UpdateMaintenancePriceList(long id, [FromBody] MaintenancePriceListDto maintenancePriceListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await maintenancePricelistService.UpdateMaintenancePriceList(id, maintenancePriceListDto);
        return Ok();
    }

    [HttpPost("maintenancePricelist")]
    public async Task<IActionResult> CreateMaintenancePriceList( [FromBody] MaintenancePriceListDto maintenancePriceListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await maintenancePricelistService.CreateMaintenancePriceList(maintenancePriceListDto);
        return Ok(maintenancePriceListDto);
    }

    [HttpDelete("maintenancePricelist/{id}")]
    public async Task<IActionResult> DeleteMaintenancePriceList(long id)
    {
        await maintenancePricelistService.DeleteMaintenancePriceList(id);

        return Ok();
    }

    [HttpGet("maintenancePricelist-items/{maintenancePricelistId}")]
    public async Task<List<MaintenancePriceListItemDto>> GetMaintenancePriceListItems(long maintenancePricelistId)
    {
        var maintenancePriceListItems = (await maintenancePricelistService.GetMaintenancePriceListItems(maintenancePricelistId)).ToList();
        return maintenancePriceListItems;
    }

    [HttpGet("maintenancePricelist-item-detail/{maintenancePricelistItemId}")]
    public async Task<MaintenancePriceListItemDto> GetMaintenancePriceListItemDetail(long maintenancePriceListItemId)
    {
        var maintenancePriceListItemDetail = await maintenancePricelistService.GetMaintenancePriceListItemDetail(maintenancePriceListItemId);
        return maintenancePriceListItemDetail;
    }

    [HttpPut("maintenancePricelist-item/{id}")]
    public async Task<IActionResult> UpdateMaintenancePriceListItem(long id, [FromBody] MaintenancePriceListItemDto maintenancePriceListItemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await maintenancePricelistService.UpdateMaintenancePriceListItem(id, maintenancePriceListItemDto);
        return Ok();
    }

    [HttpPost("maintenancePricelist-item")]
    public async Task<IActionResult> CreateMaintenancePriceListItem([FromBody] MaintenancePriceListItemDto maintenancePriceListItemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await maintenancePricelistService.CreateMaintenancePriceListItem(maintenancePriceListItemDto);
        return Ok(maintenancePriceListItemDto);
    }

    [HttpDelete("maintenancePricelist-item/{id}")]
    public async Task<IActionResult> DeleteMaintenancePriceListItem(long id)
    {
      
        await maintenancePricelistService.DeleteMaintenancePriceListItem(id);

        return Ok();
    }

}
