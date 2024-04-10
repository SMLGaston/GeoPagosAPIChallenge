using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapGet("/Procesador/{monto}", (string monto) =>
{
    try
    {
        decimal numero = Convert.ToDecimal(monto.Replace(",", "."));
        return (Math.Abs(numero % 1) == 0);
    }
    catch
    {
        return false;
    }

})
.WithName("GetProcesador")
.WithOpenApi();

app.Run();

