namespace BeaconSdk
{
    using Infrastructure.Repositories;
    using Infrastructure.Serialization;
    using Infrastructure.Transport.Communication;
    using MatrixSdk;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBeaconSdk(this IServiceCollection services)
        {
            services.AddMatrixSdk();
            services.AddSingleton<ISdkStorage, SdkStorage>();
            services.AddSingleton<RelayServerService>();
            services.AddSingleton<JsonSerializerService>();

            return services;
        }
    }
}