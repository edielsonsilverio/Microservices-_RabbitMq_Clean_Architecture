using MediatR;
using MicorRabbit.Infra.IoC;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.Command;
using MicroRabbit.Banking.Domain.CommandHandlers;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Infra.Bus;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<BankingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection"));
});


//Carrega as inforamções do arquivo appSettings com a tag RabbitMQSettings
var appSettings = builder.Configuration.GetSection("RabbitMQSettings");
builder.Services.Configure<RabbitMQSettings>(appSettings);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IAccountRespository, AccountRespository>();
builder.Services.AddTransient<BankingDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
         builder.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()
    );
});

var app = builder.Build();

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