namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
    using Core.Domain.MatrixRoom;
    using Core.Domain.Room;
    using Sodium;

    /// <summary>
    ///     A Client for interaction with Matrix.
    /// </summary>
    public interface IMatrixClient
    {
        string UserId { get; }

        Uri? BaseAddress { get; }

        MatrixEventNotifier<List<BaseRoomEvent>> MatrixEventNotifier { get; }

        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }

        Task StartAsync(Uri? baseAddress, KeyPair keyPair);

        Task StopAsync();

        Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds);

        Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId);

        Task<string> SendMessageAsync(string roomId, string message);

        Task<List<string>> GetJoinedRoomsIdsAsync();

        Task LeaveRoomAsync(string roomId);
    }
}