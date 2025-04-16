var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();

app.UseWebSockets();
app.UseRouting();
app.MapControllers();

app.Run(url:"http://localhost:8022");
