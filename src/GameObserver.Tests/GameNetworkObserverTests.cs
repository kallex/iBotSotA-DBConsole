using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using GameServices;
using Photon.Realtime;
using Xunit;

namespace GameObserver.Tests
{
    public class GameNetworkObserverTests : IAsyncLifetime
    {
        public string PhotonAppId { get; set; }

        [Fact]
        public void ListRoomsTest()
        {
            var networkObserver = new GameNetworkObserver(PhotonAppId);

            bool waitUntil = true;

            networkObserver.CallNotifier = eventName => Debug.WriteLine("Calling: " + eventName);

            var myLobby = new TypedLobby("testtest", LobbyType.SqlLobby);

            networkObserver.OnDisconnected += cause =>
            {
                waitUntil = false;
            };
            networkObserver.OnConnectedToMaster += () =>
            {
                networkObserver.PhotonClient.OpJoinLobby(null);
                return;
                networkObserver.PhotonClient.OpJoinLobby(myLobby);
            };
            networkObserver.OnConnected += () =>
            {
                //networkObserver.PhotonClient.OpJoinLobby(TypedLobby.Default);
            };

            networkObserver.OnJoinedLobby += () =>
            {
                return;
                networkObserver.PhotonClient.OpJoinOrCreateRoom(new EnterRoomParams()
                {
                    RoomName = "TestingTesting"
                });
            };
            networkObserver.OnJoinedRoom += () =>
            {
                //waitUntil = false;
                Debug.WriteLine("Joined room");
            };

            networkObserver.OnRoomListUpdate += roomList =>
            {
                Debug.WriteLine("Room list count: " + roomList.Count);
                waitUntil = roomList.Count < 1;
            };

            networkObserver.PhotonClient.NickName = nameof(ListRoomsTest);
            networkObserver.Connect();

            while (waitUntil)
            {
                //await Task.Delay(1000);
                networkObserver.Update();
                //networkObserver.PhotonClient.OpGetGameList(myLobby, "name=testtest");
                Thread.Sleep(500);
            }
        }

        public async Task InitializeAsync()
        {
            var parameterClient = new AwsParameterStoreClient(RegionEndpoint.EUWest1);
            var photonAppId = await parameterClient.GetValueAsync("ibotsota-photonappid");
            PhotonAppId = photonAppId;
        }

        public async Task DisposeAsync()
        {
        }
    }

    public class AwsParameterStoreClient
    {
        private readonly RegionEndpoint _region;

        public AwsParameterStoreClient(
            RegionEndpoint region)
        {
            _region = region;
        }

        public async Task<string> GetValueAsync(string parameter)
        {
            var ssmClient = new AmazonSimpleSystemsManagementClient(_region);

            var response = await ssmClient.GetParameterAsync(new GetParameterRequest
            {
                Name = parameter,
                WithDecryption = true
            });

            return response.Parameter.Value;
        }
    }


}
