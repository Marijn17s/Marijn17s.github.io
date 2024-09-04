using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.MySQL(
        connectionString: "server=mysql6008.site4now.net;uid=aac827_rcflow;pwd=SummaIct2024!;database=db_aac827_rcflow;ConvertZeroDateTime=true;",
        tableName: "logs"
    )
    .CreateLogger();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();