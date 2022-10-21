using Beersender.Domain.Commands;
using Beersender.Data;
using Beersender.Domain.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EventContext>(options => options.UseSqlServer("Server=localhost;Database=beersender;User id=sa;Password=sqledge123!", b => b.MigrationsAssembly("Beersender.Api")));
builder.Services.AddTransient<Sql_event_store>();

builder.Services.AddTransient<Beer_package_router<Create_package>>(services =>
{
    Guid aggregateId = Guid.NewGuid();
    var store = services.GetService<Sql_event_store>();
    return new Beer_package_router<Create_package>(id => store?.Get_events(id) ?? Enumerable.Empty<IEvent>(), @event => store?.Publish(aggregateId, @event));
});

builder.Services.AddTransient<Beer_package_router<Add_shipping_label>>(services =>
{
    Guid aggregateId = Guid.NewGuid();
    var store = services.GetService<Sql_event_store>();
    return new Beer_package_router<Add_shipping_label>(id => store?.Get_events(id) ?? Enumerable.Empty<IEvent>(), @event => store?.Publish(aggregateId, @event));
});

builder.Services.AddTransient<Beer_package_router<Send_package>>(services =>
{
    Guid aggregateId = Guid.NewGuid();
    var store = services.GetService<Sql_event_store>();
    return new Beer_package_router<Send_package>(id => store?.Get_events(id) ?? Enumerable.Empty<IEvent>(), @event => store?.Publish(aggregateId, @event));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/command", () =>
{
    // TODO
})
.WithName("PostCommand");

app.Run();