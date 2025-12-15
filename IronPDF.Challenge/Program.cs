//using IronSoftware.License;

IronPdf.License.LicenseKey = "IRONSUITE.OOYEDEPO.WEBMD.NET.14806-A59C6E7191-BMSSZNEETGLRLOFN-IVOBIFX3V35Z-4IDZUHOOXONX-4FPEZ4Q2IEBW-6DI2DJN3WHC4-P5T4YMTES27Q-K7DVMJ-TGRTEFVP77CQEA-DEPLOYMENT.TRIAL-SI3CZC.TRIAL.EXPIRES.12.DEC.2025";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
