using Swashbuckle.AspNetCore.SwaggerUI;

using RequestResponseInterceptor;
using RequestResponseInterceptor.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




InterceptorOptions options = new InterceptorOptions(){
    //If you are using docker container logs, leave it enabled. It is going to agrupate whole request and reponse line and will write to logs in the end
    WriteRequestAndResponseTogetherInTheEnd = false, 

    //If you are using docker container logs, leave it enabled. It will be easier to search by 'traceId'
    WriteTraceIDBeforEachLine = true, 
};
IInterceptor interceptor = new InterceptorToTXTFile(options);
app.UseInterceptor(interceptor);







app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { } // For integration testing