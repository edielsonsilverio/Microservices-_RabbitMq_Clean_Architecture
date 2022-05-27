using MicorRabbit.Infra.IoC;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHanlders;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<TransferDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TransferDbConnection"));
});


//Carrega as inforamções do arquivo appSettings com a tag RabbitMQSettings
var appSettings = builder.Configuration.GetSection("RabbitMQSettings");
builder.Services.Configure<RabbitMQSettings>(appSettings);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddTransient<ITransferService, TransferService>();
builder.Services.AddTransient<ITransferRepository, TransferRepository>();
builder.Services.AddTransient<TransferDbContext>();

builder.Services.AddTransient<IEventHandler<TransferCreateEvent>, TransferEventHandler>();


//Subscritpions
builder.Services.AddTransient<TransferEventHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
         builder.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()
    );
});

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<TransferCreateEvent, TransferEventHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();


// docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MeuDb@123" -p 11433:1433 --name "sqlserver-microservice" -d mcr.microsoft.com/mssql/server:latest

// Add-Migration "Initial Migration" -Context BankingDbContext
// Add-Migration "Initial Migration" -Context TransferDbContext

// Update-Database -Context BankingDbContext
// Update-Database -Context TransferDbContext