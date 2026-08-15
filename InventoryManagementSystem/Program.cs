
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Services;
using InventoryManagement.Application.Validators;
using InventoryManagement.Infrastructure;
using InventoryManagementSystem.Middleware;

namespace InventoryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddInfrastructure(builder.Configuration);

            // Application layer services
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IStockService, StockService>();

            // Application layer validators
            builder.Services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryValidator>();
            builder.Services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryValidator>();
            builder.Services.AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();
            builder.Services.AddScoped<IValidator<UpdateProductDto>, UpdateProductValidator>();
            builder.Services.AddScoped<IValidator<CreateStockDto>, CreateStockValidator>();
            builder.Services.AddScoped<IValidator<UpdateStockDto>, UpdateStockValidator>();

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

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
