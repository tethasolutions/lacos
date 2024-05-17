using Lacos.GestioneCommesse.WebApi;

var builder = WebApplication.CreateBuilder(args);
//#if DEBUG 
//builder.WebHost.UseKestrel();
//builder.WebHost.UseIIS();
//builder.WebHost.UseUrls("http://*:37998");
//#else
//#endif


builder.Services.AddWebApi(builder.Configuration);

var app = builder.Build();

app.UseWebApi(app.Environment);

app.Run();
