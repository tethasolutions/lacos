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
using Lacos.GestioneCommesse.Application.Products.DTOs;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class ProductsController : LacosApiController
{
    [HttpGet("products")]
    public async Task<DataSourceResult> GetProducts([DataSourceRequest] DataSourceRequest request)
    {
        List<ProductReadModel> products = new List<ProductReadModel>
        {
            new ProductReadModel
            {
                Id = 1,
                Code = "P001",
                Name = "Porta finestra 1 anta",
                Description = "Portoncino 1 anta con fiancoluce",
                PictureFileName = "https://okna-everest.ru/assets/products/vhodnye-dveri/plastikovaya-vhodnaya-odnostvorchataya-dver-so-steklom-800x2200.jpg",
                QrCode = "https://cdn.ttgtmedia.com/rms/misc/qr_code_barcode.jpg",
                ProductType = new ProductTypeDto
                {
                    Name = "Porta"
                }
            },
            new ProductReadModel
            {
                Id = 2,
                Code = "F001",
                Name = "Finestra 2 ante",
                Description = "Finestra 2 ante Green Evolution",
                PictureFileName = "https://oknap.ru/upload/medialibrary/e9f/size-1.jpg",
                QrCode = "https://cdn.ttgtmedia.com/rms/misc/qr_code_barcode.jpg",
                ProductType = new ProductTypeDto
                {
                    Name = "Finestra"
                }
            },
            new ProductReadModel
            {
                Id = 3,
                Code = "PF001",
                Name = "Porta finestra 3 ante",
                Description = "Portafinestra 3 ante montante Dx Green Evolution",
                PictureFileName = "https://www.okno-moskva.ru/files/647/1-1.jpg",
                QrCode = "https://cdn.ttgtmedia.com/rms/misc/qr_code_barcode.jpg",
                ProductType = new ProductTypeDto
                {
                    Name = "Porta Finestra 3 ante"
                }
            }
        };

        DataSourceResult result = new DataSourceResult
        {
            AggregateResults = null,
            Errors = null,
            Total = 3,
            Data = products
        };

        return result;
    }
}
