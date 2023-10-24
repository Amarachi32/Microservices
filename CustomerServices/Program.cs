using CustomerServices.Entity;
using MassTransit;
using static MassTransit.Logging.OperationName;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


/*builder.Services.AddMassTransit(mass =>
{

    mass.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("ticketQueue", e =>
        {
            e.ConfigureConsumer<TicketConsumer>(context);
        });
       // cfg.ConfigureEndpoints(context);
    });
});*/
/*var busControl = app.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync(new CancellationToken());*/

//create bus control
//var busControl = Bus.Factory.CreateUsingRabbitMq();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("ticketQueue", e =>
    {
        e.Consumer<TicketConsumer>();
    });

});
//start bus control
await busControl.StartAsync(new CancellationToken());

try
{
    Console.WriteLine("Press enter to exit");

    await Task.Run(() => Console.ReadLine());
}
finally
{
    //stop bus control
    await busControl.StopAsync();
}
var app = builder.Build();
app.Services.GetRequiredService<IBusControl>().Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
