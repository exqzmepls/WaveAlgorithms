using Core.Clients.Echo;
using Core.Configuration;
using Core.Services.Echo;

var builder = WebApplication.CreateBuilder(args);

var switchMappings = new Dictionary<string, string>()
{
    { "-p", "Endpoint" }
};
builder.Configuration.AddCommandLine(args, switchMappings);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "[HH:mm:ss.fffffff] ";
});
builder.Services.AddSingleton<INetConfigurationProvider, NetConfigurationProvider>();
builder.Services.AddHttpClient<IEchoClient, HttpEchoClient>();
builder.Services.AddSingleton<IEchoClient, HttpEchoClient>();
builder.Services.AddSingleton<IEchoService, EchoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();