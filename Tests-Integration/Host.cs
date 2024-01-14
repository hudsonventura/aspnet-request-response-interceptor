using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;


namespace Tests_Integration;

public class Host : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {


        builder.ConfigureServices(services =>
        {

        });

        return base.CreateHost(builder);
    }
}