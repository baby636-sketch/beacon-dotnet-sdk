namespace Beacon.Sdk.Core
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Repositories;
    using Matrix.Sdk.Core.Domain.Room;
    using Matrix.Sdk.Core.Utils;
    using Matrix.Sdk.Listener;

    public class EncryptedMessageListener : MatrixEventListener<List<BaseRoomEvent>>
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly Action<TextMessageEvent> _onNewTextMessage;

        private readonly HexString _publicKeyToListen;

        public EncryptedMessageListener(ICryptographyService cryptographyService,
            ISessionKeyPairRepository sessionKeyPairRepository,
            HexString publicKeyToListen,
            Action<TextMessageEvent> onNewTextMessage)
        {
            // _keyPair = keyPair;
            _publicKeyToListen = publicKeyToListen;
            _onNewTextMessage = onNewTextMessage;
            _cryptographyService = cryptographyService;
        }

        public override void OnCompleted() => throw new NotImplementedException();

        public override void OnError(Exception error) => throw error;

        public override void OnNext(List<BaseRoomEvent> value)
        {
            foreach (BaseRoomEvent matrixRoomEvent in value)
                if (matrixRoomEvent is TextMessageEvent textMessageEvent)
                    if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, _publicKeyToListen) &&
                        _cryptographyService.Validate(textMessageEvent.Message)) // Todo: implement validate
                        _onNewTextMessage(textMessageEvent);
        }

        private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
        {
            byte[] hash = _cryptographyService.Hash(publicKey.ToByteArray());

            return HexString.TryParse(hash, out HexString hexHash) &&
                   senderUserId.StartsWith($"@{hexHash}");
        }
    }
}

// private string Decrypt(string encryptedMessage, HexString publicKey)
// {
//     var encryptedBytes = HexString.TryParse(encryptedMessage, out var hexString)
//         ? hexString.ToByteArray()
//         : Encoding.UTF8.GetBytes(encryptedMessage);
//
//     var serverSessionKeyPair = SessionKeyPairInMemory.CreateOrReadServer(publicKey, keyPair);
//     
//     var decryptedBytes = EncryptionService.Decrypt(encryptedBytes, serverSessionKeyPair.Rx);
//
//     return Encoding.UTF8.GetString(decryptedBytes);
// }