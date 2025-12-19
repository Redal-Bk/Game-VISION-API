using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Game_Vision.AppExtention
{
    public static class LogManagerExtension
    {
        public static IHostBuilder UseGameVisionLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, configuration) =>
            {
                var connectionString = context.Configuration.GetConnectionString("GameVision");

                var sinkOptions = new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = false, // در تولید false باشه
                    BatchPostingLimit = 100,
                    EagerlyEmitFirstEvent = true
                };

                var columnOptions = new ColumnOptions
                {
                    AdditionalColumns = new[]
                    {
                        new SqlColumn { ColumnName = "UserId", DataType = System.Data.SqlDbType.Int, AllowNull = true },
                        new SqlColumn { ColumnName = "UserName", DataType = System.Data.SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                        new SqlColumn { ColumnName = "ClientIP", DataType = System.Data.SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
                        new SqlColumn { ColumnName = "RequestMethod", DataType = System.Data.SqlDbType.NVarChar, DataLength = 10, AllowNull = true },
                        new SqlColumn { ColumnName = "RequestPath", DataType = System.Data.SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                        new SqlColumn { ColumnName = "QueryString", DataType = System.Data.SqlDbType.NVarChar, DataLength = 1000, AllowNull = true },
                        new SqlColumn { ColumnName = "StatusCode", DataType = System.Data.SqlDbType.Int, AllowNull = true },                         
                        new SqlColumn { ColumnName = "ElapsedMs", DataType = System.Data.SqlDbType.Float, AllowNull = true },
                        new SqlColumn { ColumnName = "UserAgent", DataType = System.Data.SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                        new SqlColumn { ColumnName = "Referrer", DataType = System.Data.SqlDbType.NVarChar, DataLength = 500, AllowNull = true }
                    }
                };

                // ذخیره JSON کامل پراپرتی‌ها
                columnOptions.Store.Remove(StandardColumn.Properties);
                columnOptions.Store.Add(StandardColumn.LogEvent);

                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentName()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: sinkOptions,
                        columnOptions: columnOptions,
                        restrictedToMinimumLevel: LogEventLevel.Information);
            });
        }

        public static IApplicationBuilder UseGameVisionRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                // سطح لاگ خودکار بر اساس وضعیت و خطا
                options.GetLevel = (httpContext, _, ex) =>
                    ex != null || httpContext.Response.StatusCode >= 500
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode >= 400
                            ? LogEventLevel.Warning
                            : LogEventLevel.Information;

                // قالب پیام — Serilog خودش {Elapsed} رو جایگزین می‌کنه
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath}{QueryString} => {StatusCode} in {Elapsed:0.0000} ms";

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // اطلاعات کاربر از JWT
                    var userIdClaim = httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var userName = httpContext.User?.Identity?.Name ?? "Anonymous";

                    diagnosticContext.Set("UserId", userIdClaim != null ? int.Parse(userIdClaim) : (int?)null);
                    diagnosticContext.Set("UserName", userName);

                    // اطلاعات درخواست
                    diagnosticContext.Set("ClientIP", GetClientIpAddress(httpContext));
                    diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value);
                    diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value ?? string.Empty);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("Referrer", httpContext.Request.Headers["Referer"].ToString());
                    diagnosticContext.Set("Host", httpContext.Request.Host.Value);

                    // اطلاعات پاسخ
                    diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);

                    // Elapsed به صورت خودکار توسط Serilog اضافه می‌شه — نیازی به دستی نیست!
                    // این مقدار دقیقاً در ستون ElapsedMs ذخیره می‌شه
                };
            });

            return app;
        }

        // تابع کمکی برای IP واقعی
        private static string GetClientIpAddress(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
                return ip.Split(',')[0].Trim();

            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
                return ip;

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}