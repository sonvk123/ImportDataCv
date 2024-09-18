using apiZaloOa.Data;
using apiZaloOa.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ImportDataCv.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext
            builder.Services.AddDbContext<apiZaloOaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("apiZaloOaContext")
                ?? throw new InvalidOperationException("Connection string 'apiZaloOaContext' not found.")));

            // Cấu hình CORS cho phép tất cả các nguồn gốc, phương thức và headers
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // Thêm tùy chọn JSON để giữ nguyên tên thuộc tính khi serializing
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // Để giữ nguyên tên thuộc tính
            });

            // Đăng ký ApiService
            builder.Services.AddScoped<ApiService>();

            builder.Services.AddHttpClient();


            // Cấu hình Swagger để giúp tạo tài liệu API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Cấu hình Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Xây dựng ứng dụng
            var app = builder.Build();

            // Cấu hình CORS với chính sách đã tạo
            app.UseCors("AllowAll");

            // Cấu hình để phục vụ các file tĩnh và mặc định
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Định tuyến các yêu cầu HTTP
            app.UseRouting();

            // Cấu hình các endpoints cho controller
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger chỉ được kích hoạt khi đang trong môi trường phát triển
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Áp dụng HTTPS và Authorization cho các yêu cầu
            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Định nghĩa fallback để trả về `index.html` cho các yêu cầu không khớp
            app.MapFallbackToFile("/index.html");

            // Chạy ứng dụng
            app.Run();
        }
    }
}
