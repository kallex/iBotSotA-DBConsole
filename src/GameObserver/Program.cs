using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace GameServices
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Warmup testing");
            monitorPhotonRoom();

        }

        static void monitorPhotonRoom()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyName = args.Name.Split(',').First() + ".dll";
                var assemblyPath =
                    Path.Combine(@"C:\Program Files\Unity\Hub\Editor\2021.1.5f1\Editor\Data\Managed\UnityEngine",
                        assemblyName);
                var assembly = Assembly.LoadFile(assemblyPath);
                return assembly;
            };

            var PhotonAppId = "37028cbe-f697-43ff-804a-7b8409a0dbfe";
            var networkObserver = new GameNetworkObserver(PhotonAppId);

            bool waitUntil = true;

            Action<string> notifier = s =>
            {
                Console.WriteLine(s);
            };

            networkObserver.CallNotifier = eventName =>
            {
                //Debug.WriteLine("Calling: " + eventName);
              notifier($"{DateTime.Now} Calling: {eventName}");
            };

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
                notifier("Joined room");
            };

            networkObserver.OnRoomListUpdate += roomList =>
            {
                notifier("Room list count: " + roomList.Count);
                //waitUntil = roomList.Count < 1;
                var existingRoom = roomList.FirstOrDefault();
                networkObserver.PhotonClient.OpJoinOrCreateRoom(new EnterRoomParams()
                {
                    RoomName = existingRoom?.Name ?? "MyRoom"
                });
            };

            networkObserver.PhotonClient.OpResponseReceived += response =>
            {
                notifier("Resp: " + response.DebugMessage);
            };

            networkObserver.PhotonClient.EventReceived += data =>
            {
                var eventInfo =
                    $"Sender: {data.Sender} - SenderKey: {data.SenderKey} - Code: {data.Code} - CustomDataKey: {data.CustomDataKey} - ParamCount: {data.Parameters.Count}";
                notifier(eventInfo);
            };

            networkObserver.PhotonClient.AppVersion = "1.0";
            networkObserver.PhotonClient.AuthMode = AuthModeOption.Auth;

            networkObserver.PhotonClient.NickName = nameof(monitorPhotonRoom);

            var lbp = networkObserver.PhotonClient.LoadBalancingPeer;
            //lbp.

            networkObserver.Connect();

            while (waitUntil)
            {
                //await Task.Delay(1000);
                bool somethingDone = networkObserver.Update();
                //networkObserver.PhotonClient.OpGetGameList(myLobby, "name=testtest");
                if(!somethingDone)
                    Thread.Sleep(500);
                else 
                    Thread.Sleep(10);
            }

        }
    }

}