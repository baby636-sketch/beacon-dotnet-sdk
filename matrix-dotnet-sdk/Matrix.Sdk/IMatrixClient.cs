namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Domain.MatrixRoom;
    using Core.Domain.Network;

    /// <summary>
    ///     A Client for interaction with Matrix.
    /// </summary>
    public interface IMatrixClient
    {
        string UserId { get; }

        Uri? BaseAddress { get; }

        bool LoggedIn { get; }

        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }
        event EventHandler<MatrixRoomEventsEventArgs> OnMatrixRoomEventsReceived;

        Task LoginAsync(LoginRequest request);

        void Start();

        void Stop();

        Task<MatrixRoom> CreateTrustedPrivateRoomAsync(string[] invitedUserIds);

        Task<MatrixRoom> JoinTrustedPrivateRoomAsync(string roomId);

        Task<string> SendMessageAsync(string roomId, string message);

        Task<List<string>> GetJoinedRoomsIdsAsync();

        Task LeaveRoomAsync(string roomId);
    }
}