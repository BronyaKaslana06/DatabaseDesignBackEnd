using EntityFramework.Context;
using EntityFramework.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ModelContext>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//string format = "yyyy-MM-dd HH:mm:ss";
//if(DateTime.TryParseExact("2023-10-12 10:12:30", format, null, System.Globalization.DateTimeStyles.None, out DateTime result1) &&
//    DateTime.TryParseExact("2023-11-11 23:22:35", format, null, System.Globalization.DateTimeStyles.None, out DateTime result2))
//    Console.WriteLine(result2-result1);
//OrderStatusEnum a = OrderStatusEnum.待评分;
//Console.WriteLine(a.ToString());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

string message = "";
bool re = true;
webapi.OracleHelper.DbConn(ref message, ref re);
Console.WriteLine("----------------\r\n" + (re ? "连接成功" : message) + "\r\n----------------");

app.Run();
