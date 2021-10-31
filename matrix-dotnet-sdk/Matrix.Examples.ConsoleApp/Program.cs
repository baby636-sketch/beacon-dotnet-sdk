﻿namespace Matrix.Examples.ConsoleApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Sdk;
    using Sdk.Core.Domain.MatrixRoom;
    using Sdk.Core.Domain.Services;
    using Sdk.Listener;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;
    using Sodium;

    internal class Program
    {
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMatrixClient();
                services.AddConsoleApp();
            }).UseConsoleLifetime();

        private static async Task<int> Main(string[] args)
        {
            IHost host = CreateHostBuilder().Build();

            SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            try
            {
                ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("START");

                await RunAsync(host.Services);
            }
            catch (Exception ex)
            {
                ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred");
            }

            return 0;
        }

        private static async Task RunAsync(IServiceProvider serviceProvider)
        {
            (IMatrixClient firstClient, TextMessageListener firstListener) =
                await SetupClientWithTextListener(serviceProvider);

            (IMatrixClient secondClient, TextMessageListener secondListener) =
                await SetupClientWithTextListener(serviceProvider);

            firstClient.Start();
            secondClient.Start();
            
            MatrixRoom firstClientMatrixRoom = await firstClient.CreateTrustedPrivateRoomAsync(new[]
            {
                secondClient.UserId
            });

            MatrixRoom matrixRoom = await secondClient.JoinTrustedPrivateRoomAsync(firstClientMatrixRoom.Id);

            var spin = new SpinWait();
            while (secondClient.JoinedRooms.Length == 0)
                spin.SpinOnce();

            await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "Hello");
            await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, ", ");

            await firstClient.SendMessageAsync(firstClientMatrixRoom.Id, "World");
            await secondClient.SendMessageAsync(secondClient.JoinedRooms[0].Id, "!");

            Console.ReadLine();

            firstClient.Stop();
            secondClient.Stop();

            firstListener.Unsubscribe();
            secondListener.Unsubscribe();
        }

        private static async Task<(IMatrixClient, TextMessageListener)> SetupClientWithTextListener(
            IServiceProvider serviceProvider)
        {
            IMatrixClient matrixClient = serviceProvider.GetRequiredService<IMatrixClient>();
            ICryptographyService cryptographyService = serviceProvider.GetRequiredService<ICryptographyService>();

            var seed = Guid.NewGuid().ToString();
            KeyPair keyPair = cryptographyService.GenerateEd25519KeyPair(seed);
            var nodeAddress = new Uri(Constants.FallBackNodeAddress);

            await matrixClient.LoginAsync(nodeAddress, keyPair); //Todo: generate once and then store seed?

            var textMessageListener = new TextMessageListener(matrixClient.UserId, (listenerId, textMessageEvent) =>
            {
                (string roomId, string senderUserId, string message) = textMessageEvent;
                if (listenerId != senderUserId)
                    Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
            });

            textMessageListener.ListenTo(matrixClient.MatrixEventNotifier);

            return (matrixClient, textMessageListener);
        }
    }
}