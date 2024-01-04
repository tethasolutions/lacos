using Lacos.GestioneCommesse.Application.Docs.DTOs;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface IPurchaseOrdersService
{
    IQueryable<PurchaseOrderReadModel> Query();
    Task<PurchaseOrderDto> Get(long id);
    Task<PurchaseOrderDto> Create(PurchaseOrderDto purchaseOrderDto);
    Task<PurchaseOrderDto> Update(PurchaseOrderDto purchaseOrderDto);
    Task Delete(long id);
    Task<int> GetNextNumber(int year);
}