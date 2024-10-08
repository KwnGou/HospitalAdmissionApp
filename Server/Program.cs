using HospitalAdmissionApp.Server.Model;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalBedsDB")));
builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
