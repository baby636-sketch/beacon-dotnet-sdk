namespace Beacon.Sdk.Core.Domain.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Matrix.Sdk.Core.Utils;
    using Transport.P2P;

    public interface IP2PCommunicationService
    {
        event EventHandler<P2PMessageEventArgs> OnP2PMessagesReceived;
        
        Task LoginAsync();
        
        void Start();
        
        void Stop();

        Task SendChannelOpeningMessageAsync(string id, HexString receiverHexPublicKey,
            string receiverRelayServer, int version, string appName);
    }
}