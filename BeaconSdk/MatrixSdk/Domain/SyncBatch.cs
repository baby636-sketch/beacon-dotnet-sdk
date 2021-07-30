namespace MatrixSdk.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Application;
    using Infrastructure.Dto.Sync;
    using Room;

    public class SyncBatch
    {
        private SyncBatch(string nextBatch, List<MatrixRoom> matrixRooms, List<BaseRoomEvent> matrixRoomEvents)
        {
            NextBatch = nextBatch;
            MatrixRooms = matrixRooms;
            MatrixRoomEvents = matrixRoomEvents;
        }

        public string NextBatch { get; }
        public List<MatrixRoom> MatrixRooms { get; }
        public List<BaseRoomEvent> MatrixRoomEvents { get; }


        internal static class Factory
        {
            private static readonly MatrixRoomFactory MatrixRoomFactory = new();
            private static readonly MatrixRoomEventFactory MatrixRoomEventFactory = new();

            public static SyncBatch CreateFromSync(string nextBatch, Rooms rooms)
            {
                var matrixRooms = GetMatrixRoomsFromSync(rooms);
                var matrixRoomEvents = GetMatrixEventsFromSync(rooms);

                return new SyncBatch(nextBatch, matrixRooms, matrixRoomEvents);
            }

            private static List<MatrixRoom> GetMatrixRoomsFromSync(Rooms rooms)
            {
                var joinedMatrixRooms = rooms.Join.Select(pair => MatrixRoomFactory.CreateJoined(pair.Key, pair.Value)).ToList();
                var invitedMatrixRooms = rooms.Invite.Select(pair => MatrixRoomFactory.CreateInvite(pair.Key, pair.Value)).ToList();
                var leftMatrixRooms = rooms.Leave.Select(pair => MatrixRoomFactory.CreateLeft(pair.Key, pair.Value)).ToList();

                return joinedMatrixRooms.Concat(invitedMatrixRooms).Concat(leftMatrixRooms).ToList();
            }

            private static List<BaseRoomEvent> GetMatrixEventsFromSync(Rooms rooms)
            {
                var joinedMatrixRoomEvents = rooms.Join.SelectMany(pair => MatrixRoomEventFactory.CreateFromJoined(pair.Key, pair.Value)).ToList();
                var invitedMatrixRoomEvents = rooms.Invite.SelectMany(pair => MatrixRoomEventFactory.CreateFromInvited(pair.Key, pair.Value)).ToList();
                var leftMatrixRoomEvents = rooms.Leave.SelectMany(pair => MatrixRoomEventFactory.CreateFromLeft(pair.Key, pair.Value)).ToList();

                return joinedMatrixRoomEvents.Concat(invitedMatrixRoomEvents).Concat(leftMatrixRoomEvents).ToList();
            }
        }
    }
}