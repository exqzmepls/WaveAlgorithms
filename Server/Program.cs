using Core.Configuration;
using Core.Services.Echo;

var builder = WebApplication.CreateBuilder(args);

var switchMappings = new Dictionary<string, string>()
{
    { "-p", "Endpoint" }
};
builder.Configuration.AddCommandLine(args, switchMappings);

// Add services to the container.
builder.Services.AddSingleton<INetConfigurationProvider, NetConfigurationProvider>();
builder.Services.AddSingleton<IEchoService, EchoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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