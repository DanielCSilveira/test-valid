using Infra;
using Worker;
using Microsoft.EntityFrameworkCore;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WorkerService>();
//// registra DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));


var host = builder.Build();
host.Run();
