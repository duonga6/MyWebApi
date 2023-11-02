using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Services.CategoryServices;
using MyWebApi.Services.ProductServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<MyDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
});

// Services

builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ICategoryService, CategoryServiceInMemory>();

builder.Services.AddScoped<IProductService, ProductSerivce>();

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
