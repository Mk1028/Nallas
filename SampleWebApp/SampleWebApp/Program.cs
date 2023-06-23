var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<JiraTaskDb>(opt => opt.UseInMemoryDatabase("JiraTask"));
// Register JiraTaskDb as a singleton
builder.Services.AddSingleton<JiraTaskDb>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors();

// Configure Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<ReadMessageService>();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "JiraTasks API V1");
		c.RoutePrefix = string.Empty;
	});
}

// Enable CORS
app.UseCors(builder => builder
	.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader());

app.UseStaticFiles();
app.Run();
