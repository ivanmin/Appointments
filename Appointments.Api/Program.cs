using System.Runtime.CompilerServices;
using Appointments.Api.Middlewares;
using Appointments.Application.Configurations;
using Appointments.Application.Dtos.Requests.Validations;
using Appointments.Application.ExternalServices.Implementations;
using Appointments.Application.ExternalServices.Interfaces;
using Appointments.Application.Services.Implementations;
using Appointments.Application.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;

[assembly: InternalsVisibleTo("SlotAppointment.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.Configure<AppointmentSettings>(builder.Configuration.GetSection("AppointmentSettings"));
builder.Services.Configure<SlotServiceSettings>(builder.Configuration.GetSection("SlotServiceSttings"));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GetWeeklyFreeSlotsRequestValidator>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ISlotExternalService, SlotExternalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

