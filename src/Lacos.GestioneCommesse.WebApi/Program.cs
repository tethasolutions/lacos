using Lacos.GestioneCommesse.WebApi;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
//#if DEBUG 
//builder.WebHost.UseKestrel();
//builder.WebHost.UseIIS();
//builder.WebHost.UseUrls("http://*:37998");
//#else
//#endif

builder.Host.UseSerilog((context, provider, config) => {
    config.WriteTo.MSSqlServer(context.Configuration.GetConnectionString("Default"),
        sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions() { 
            TableName = "EventLogs", 
            SchemaName = "Logs",
            AutoCreateSqlTable = true,
        },
        restrictedToMinimumLevel: LogEventLevel.Information
       );
    });

builder.Services.AddWebApi(builder.Configuration);

var app = builder.Build();

app.UseWebApi(app.Environment);

app.Run();
