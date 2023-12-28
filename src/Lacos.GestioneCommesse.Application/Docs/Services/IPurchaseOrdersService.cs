using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IPurchaseOrdersService
{
    IQueryable<PurchaseOrderReadModel> Query();
    Task<PurchaseOrderDto> Get(long id);
    Task<PurchaseOrderItemDto> GetItem(long id);
    Task<PurchaseOrderDto> Create(PurchaseOrderDto purchaseOrderDto);
    Task<PurchaseOrderDto> Update(PurchaseOrderDto purchaseOrderDto);
    Task<PurchaseOrderItemDto> UpdateItem(PurchaseOrderItemDto purchaseOrderItemDto);
    Task Delete(long id);
    Task<int> GetNextNumber(int year);
}