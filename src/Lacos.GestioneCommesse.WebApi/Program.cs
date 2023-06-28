using Lacos.GestioneCommesse.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi(builder.Configuration);

var app = builder.Build();

app.UseWebApi(app.Environment);

app.Run();
