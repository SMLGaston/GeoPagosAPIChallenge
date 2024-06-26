using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
        decimal numero = Convert.ToDecimal(monto);
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