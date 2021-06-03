
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace GameServices
{

    public partial class GameNetworkObserver : IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, IOnEventCallback
    {
        public Action<string> CallNotifier;


        public void ClearData() 
        {
    clearIConnectionCallbacksData();
clearILobbyCallbacksData();
clearIMatchmakingCallbacksData();
clearIInRoomCallbacksData();
clearIOnEventCallbackData();
        }

        public void ClearEvents()
        {
    clearIConnectionCallbacksEvents();
clearILobbyCallbacksEvents();
clearIMatchmakingCallbacksEvents();
clearIInRoomCallbacksEvents();
clearIOnEventCallbackEvents();
        }


        
        public delegate void OnConnectedHandler();

        public event OnConnectedHandler OnConnected;
        void IConnectionCallbacks.OnConnected()
        {
            CallNotifier?.Invoke("IConnectionCallbacks.OnConnected");
            
            OnConnected?.Invoke();
        }


        
        public delegate void OnConnectedToMasterHandler();

        public event OnConnectedToMasterHandler OnConnectedToMaster;
        void IConnectionCallbacks.OnConnectedToMaster()
        {
            CallNotifier?.Invoke("IConnectionCallbacks.OnConnectedToMaster");
            
            OnConnectedToMaster?.Invoke();
        }


        public  DisconnectCause OnDisconnected_cause;
        public delegate void OnDisconnectedHandler(DisconnectCause cause);

        public event OnDisconnectedHandler OnDisconnected;
        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
        {
            CallNotifier?.Invoke("IConnectionCallbacks.OnDisconnected");
            OnDisconnected_cause = cause;
            OnDisconnected?.Invoke(cause);
        }


        public  RegionHandler OnRegionListReceived_regionHandler;
        public delegate void OnRegionListReceivedHandler(RegionHandler regionHandler);

        public event OnRegionListReceivedHandler OnRegionListReceived;
        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
        {
            CallNotifier?.Invoke("IConnectionCallbacks.OnRegionListReceived");
            OnRegionListReceived_regionHandler = regionHandler;
            OnRegionListReceived?.Invoke(regionHandler);
        }


        public  Dictionary<string,object> OnCustomAuthenticationResponse_data;
        public delegate void OnCustomAuthenticationResponseHandler(Dictionary<string,object> data);

        public event OnCustomAuthenticationResponseHandler OnCustomAuthenticationResponse;
        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string,object> data)
        {
            CallNotifier?.Invoke("IConnectionCallbacks.OnCustomAuthenticationResponse");
            OnCustomAuthenticationResponse_data = data;
            OnCustomAuthenticationResponse?.Invoke(data);
        }


        public  string OnCustomAuthenticationFailed_debugMessage;
        public delegate void OnCustomAuthenticationFailedHandler(string debugMessage);

        public event OnCustomAuthenticationFailedHandler OnCustomAuthenticationFailed;
        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
        {
            CallNotifier?.Invoke("IConnectionCallbacks.OnCustomAuthenticationFailed");
            OnCustomAuthenticationFailed_debugMessage = debugMessage;
            OnCustomAuthenticationFailed?.Invoke(debugMessage);
        }


            private void clearIConnectionCallbacksData() 
            {
OnRegionListReceived_regionHandler = null;
OnCustomAuthenticationResponse_data = null;
OnCustomAuthenticationFailed_debugMessage = null;
            }

            private void clearIConnectionCallbacksEvents()
            {
OnConnected = null;
OnConnectedToMaster = null;
OnDisconnected = null;
OnRegionListReceived = null;
OnCustomAuthenticationResponse = null;
OnCustomAuthenticationFailed = null;
            }



        
        public delegate void OnJoinedLobbyHandler();

        public event OnJoinedLobbyHandler OnJoinedLobby;
        void ILobbyCallbacks.OnJoinedLobby()
        {
            CallNotifier?.Invoke("ILobbyCallbacks.OnJoinedLobby");
            
            OnJoinedLobby?.Invoke();
        }


        
        public delegate void OnLeftLobbyHandler();

        public event OnLeftLobbyHandler OnLeftLobby;
        void ILobbyCallbacks.OnLeftLobby()
        {
            CallNotifier?.Invoke("ILobbyCallbacks.OnLeftLobby");
            
            OnLeftLobby?.Invoke();
        }


        public  List<RoomInfo> OnRoomListUpdate_roomList;
        public delegate void OnRoomListUpdateHandler(List<RoomInfo> roomList);

        public event OnRoomListUpdateHandler OnRoomListUpdate;
        void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList)
        {
            CallNotifier?.Invoke("ILobbyCallbacks.OnRoomListUpdate");
            OnRoomListUpdate_roomList = roomList;
            OnRoomListUpdate?.Invoke(roomList);
        }


        public  List<TypedLobbyInfo> OnLobbyStatisticsUpdate_lobbyStatistics;
        public delegate void OnLobbyStatisticsUpdateHandler(List<TypedLobbyInfo> lobbyStatistics);

        public event OnLobbyStatisticsUpdateHandler OnLobbyStatisticsUpdate;
        void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            CallNotifier?.Invoke("ILobbyCallbacks.OnLobbyStatisticsUpdate");
            OnLobbyStatisticsUpdate_lobbyStatistics = lobbyStatistics;
            OnLobbyStatisticsUpdate?.Invoke(lobbyStatistics);
        }


            private void clearILobbyCallbacksData() 
            {
OnRoomListUpdate_roomList = null;
OnLobbyStatisticsUpdate_lobbyStatistics = null;
            }

            private void clearILobbyCallbacksEvents()
            {
OnJoinedLobby = null;
OnLeftLobby = null;
OnRoomListUpdate = null;
OnLobbyStatisticsUpdate = null;
            }



        public  List<FriendInfo> OnFriendListUpdate_friendList;
        public delegate void OnFriendListUpdateHandler(List<FriendInfo> friendList);

        public event OnFriendListUpdateHandler OnFriendListUpdate;
        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnFriendListUpdate");
            OnFriendListUpdate_friendList = friendList;
            OnFriendListUpdate?.Invoke(friendList);
        }


        
        public delegate void OnCreatedRoomHandler();

        public event OnCreatedRoomHandler OnCreatedRoom;
        void IMatchmakingCallbacks.OnCreatedRoom()
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnCreatedRoom");
            
            OnCreatedRoom?.Invoke();
        }


        public  short OnCreateRoomFailed_returnCode;
public  string OnCreateRoomFailed_message;
        public delegate void OnCreateRoomFailedHandler(short returnCode, string message);

        public event OnCreateRoomFailedHandler OnCreateRoomFailed;
        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnCreateRoomFailed");
            OnCreateRoomFailed_returnCode = returnCode;
OnCreateRoomFailed_message = message;
            OnCreateRoomFailed?.Invoke(returnCode, message);
        }


        
        public delegate void OnJoinedRoomHandler();

        public event OnJoinedRoomHandler OnJoinedRoom;
        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnJoinedRoom");
            
            OnJoinedRoom?.Invoke();
        }


        public  short OnJoinRoomFailed_returnCode;
public  string OnJoinRoomFailed_message;
        public delegate void OnJoinRoomFailedHandler(short returnCode, string message);

        public event OnJoinRoomFailedHandler OnJoinRoomFailed;
        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnJoinRoomFailed");
            OnJoinRoomFailed_returnCode = returnCode;
OnJoinRoomFailed_message = message;
            OnJoinRoomFailed?.Invoke(returnCode, message);
        }


        public  short OnJoinRandomFailed_returnCode;
public  string OnJoinRandomFailed_message;
        public delegate void OnJoinRandomFailedHandler(short returnCode, string message);

        public event OnJoinRandomFailedHandler OnJoinRandomFailed;
        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnJoinRandomFailed");
            OnJoinRandomFailed_returnCode = returnCode;
OnJoinRandomFailed_message = message;
            OnJoinRandomFailed?.Invoke(returnCode, message);
        }


        
        public delegate void OnLeftRoomHandler();

        public event OnLeftRoomHandler OnLeftRoom;
        void IMatchmakingCallbacks.OnLeftRoom()
        {
            CallNotifier?.Invoke("IMatchmakingCallbacks.OnLeftRoom");
            
            OnLeftRoom?.Invoke();
        }


            private void clearIMatchmakingCallbacksData() 
            {
OnFriendListUpdate_friendList = null;
OnCreateRoomFailed_message = null;
OnJoinRoomFailed_message = null;
OnJoinRandomFailed_message = null;
            }

            private void clearIMatchmakingCallbacksEvents()
            {
OnFriendListUpdate = null;
OnCreatedRoom = null;
OnCreateRoomFailed = null;
OnJoinedRoom = null;
OnJoinRoomFailed = null;
OnJoinRandomFailed = null;
OnLeftRoom = null;
            }



        public  Player OnPlayerEnteredRoom_newPlayer;
        public delegate void OnPlayerEnteredRoomHandler(Player newPlayer);

        public event OnPlayerEnteredRoomHandler OnPlayerEnteredRoom;
        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
        {
            CallNotifier?.Invoke("IInRoomCallbacks.OnPlayerEnteredRoom");
            OnPlayerEnteredRoom_newPlayer = newPlayer;
            OnPlayerEnteredRoom?.Invoke(newPlayer);
        }


        public  Player OnPlayerLeftRoom_otherPlayer;
        public delegate void OnPlayerLeftRoomHandler(Player otherPlayer);

        public event OnPlayerLeftRoomHandler OnPlayerLeftRoom;
        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
        {
            CallNotifier?.Invoke("IInRoomCallbacks.OnPlayerLeftRoom");
            OnPlayerLeftRoom_otherPlayer = otherPlayer;
            OnPlayerLeftRoom?.Invoke(otherPlayer);
        }


        public  Hashtable OnRoomPropertiesUpdate_propertiesThatChanged;
        public delegate void OnRoomPropertiesUpdateHandler(Hashtable propertiesThatChanged);

        public event OnRoomPropertiesUpdateHandler OnRoomPropertiesUpdate;
        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            CallNotifier?.Invoke("IInRoomCallbacks.OnRoomPropertiesUpdate");
            OnRoomPropertiesUpdate_propertiesThatChanged = propertiesThatChanged;
            OnRoomPropertiesUpdate?.Invoke(propertiesThatChanged);
        }


        public  Player OnPlayerPropertiesUpdate_targetPlayer;
public  Hashtable OnPlayerPropertiesUpdate_changedProps;
        public delegate void OnPlayerPropertiesUpdateHandler(Player targetPlayer, Hashtable changedProps);

        public event OnPlayerPropertiesUpdateHandler OnPlayerPropertiesUpdate;
        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            CallNotifier?.Invoke("IInRoomCallbacks.OnPlayerPropertiesUpdate");
            OnPlayerPropertiesUpdate_targetPlayer = targetPlayer;
OnPlayerPropertiesUpdate_changedProps = changedProps;
            OnPlayerPropertiesUpdate?.Invoke(targetPlayer, changedProps);
        }


        public  Player OnMasterClientSwitched_newMasterClient;
        public delegate void OnMasterClientSwitchedHandler(Player newMasterClient);

        public event OnMasterClientSwitchedHandler OnMasterClientSwitched;
        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
        {
            CallNotifier?.Invoke("IInRoomCallbacks.OnMasterClientSwitched");
            OnMasterClientSwitched_newMasterClient = newMasterClient;
            OnMasterClientSwitched?.Invoke(newMasterClient);
        }


            private void clearIInRoomCallbacksData() 
            {
OnPlayerEnteredRoom_newPlayer = null;
OnPlayerLeftRoom_otherPlayer = null;
OnRoomPropertiesUpdate_propertiesThatChanged = null;
OnPlayerPropertiesUpdate_targetPlayer = null;
OnPlayerPropertiesUpdate_changedProps = null;
OnMasterClientSwitched_newMasterClient = null;
            }

            private void clearIInRoomCallbacksEvents()
            {
OnPlayerEnteredRoom = null;
OnPlayerLeftRoom = null;
OnRoomPropertiesUpdate = null;
OnPlayerPropertiesUpdate = null;
OnMasterClientSwitched = null;
            }



        public  EventData OnEvent_photonEvent;
        public delegate void OnEventHandler(EventData photonEvent);

        public event OnEventHandler OnEvent;
        void IOnEventCallback.OnEvent(EventData photonEvent)
        {
            CallNotifier?.Invoke("IOnEventCallback.OnEvent");
            OnEvent_photonEvent = photonEvent;
            OnEvent?.Invoke(photonEvent);
        }


            private void clearIOnEventCallbackData() 
            {
OnEvent_photonEvent = null;
            }

            private void clearIOnEventCallbackEvents()
            {
OnEvent = null;
            }


    }
}

