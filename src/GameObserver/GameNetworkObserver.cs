using System;
using Photon.Realtime;

namespace GameServices
{
    public partial class GameNetworkObserver : IDisposable
    {
        public LoadBalancingClient PhotonClient;

        public GameNetworkObserver(string photonAppKey)
        {
            PhotonClient = new LoadBalancingClient();
            PhotonClient.AppId = photonAppKey;
            Initialize();
        }

        public void Connect()
        {
            PhotonClient.ConnectToRegionMaster("eu");
        }

        public void Initialize()
        {
            PhotonClient.AddCallbackTarget(this);
        }

        public void Cleanup()
        {
            PhotonClient.RemoveCallbackTarget(this);
            ClearEvents();
            ClearData();
        }

        internal int intervalDispatch = 10;                 // interval between DispatchIncomingCommands() calls
        internal int lastDispatch = Environment.TickCount;
        internal int intervalSend = 50;                     // interval between SendOutgoingCommands() calls
        internal int lastSend = Environment.TickCount;

        public bool Update()
        {
            bool somethingDone = false;
            if (Environment.TickCount - this.lastDispatch > this.intervalDispatch)
            {
                this.lastDispatch = Environment.TickCount;
                somethingDone = PhotonClient.LoadBalancingPeer.DispatchIncomingCommands();
            }

            if (Environment.TickCount - this.lastSend > this.intervalSend)
            {
                this.lastSend = Environment.TickCount;
                somethingDone = somethingDone || PhotonClient.LoadBalancingPeer.SendOutgoingCommands(); // will send pending, outgoing commands
            }
            /*
            if (this.intervalMove != 0 && Environment.TickCount - this.lastMove > this.intervalMove)
            {
                this.lastMove = Environment.TickCount;
                if (this.State == ClientState.Joined)
                {
                    ((DemoPlayer)LocalPlayer).MoveRandom();
                    this.SendPosition();
                }
            }
            */
            /*
            // Update call for windows phone UI-Thread
            if (Environment.TickCount - this.lastUiUpdate > this.intervalUiUpdate)
            {
                this.lastUiUpdate = Environment.TickCount;
                if (this.OnUpdate != null)
                {
                    this.OnUpdate();
                }
            }
            */
            return somethingDone;
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
