using FluentValidation;
using TinyMediator.DependencyInjection;
using TinyMediator.Sample.Application.Ports;
using TinyMediator.Sample.Infrastructure.Items;
using TinyMediator.Sample.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator(opts =>
{
    opts.UsePipelineMediator = true;
    opts.HandlerLifetime = ServiceLifetime.Scoped;
    opts.BehaviorLifetime = ServiceLifetime.Scoped;
}, typeof(Program).Assembly);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddSingleton<InMemoryItemsDb>();
builder.Services.AddSingleton<IItemReadStore, InMemoryItemReadStore>();
builder.Services.AddSingleton<IItemRepository, InMemoryItemRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 에러 매핑 미들웨어
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapItems();

app.Run();
