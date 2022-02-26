using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DirectLineTokenFuncProj
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                {
                    s.AddHttpClient<IDlTokenFunction, DlTokenFunction>();
                })
                .Build();

            host.Run();
        }
    }
}