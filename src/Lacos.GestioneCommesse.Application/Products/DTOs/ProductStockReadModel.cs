namespace Lacos.GestioneCommesse.Application.Products.DTOs;

public class ProductStockReadModel
{
    public long Id { get; set; }
    public string? ProductType { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Side { get; set; }
    public string? Size { get; set; }
    public string? Material { get; set; }
    public int StockQuantity { get; set; }
}