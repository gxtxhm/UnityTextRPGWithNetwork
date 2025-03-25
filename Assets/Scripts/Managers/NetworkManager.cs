using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
//using System.Diagnostics;
#nullable enable

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    public PhotonView PV;

    public int CountReadyPlayer=0;

    int CountRoomNum = 1;


    public event Action<int>? OnChangedPlayerCount;

    private void Awake()
    {
        if (Instance == null)
        {
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PV = GetComponent<PhotonView>();
            // 모든 플레이어가 씬을 자동으로 동기화하도록 설정
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    private void Update()
    {
        int curCount;

        if (PhotonNetwork.InRoom)
        {
            // 방 안에 있을 때는 방의 인원 수를 가져옴
            curCount = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        else
        {
            curCount = PhotonNetwork.CountOfPlayers;
        }

        {
            Debug.Log($"현재 인원 : {curCount}");
            StartCoroutine(InvokeChangedPlayerCount(curCount));
        }
    }

    IEnumerator InvokeChangedPlayerCount(int curCount)
    {
        while(!UIManager.Instance.IsSceneLoaded)
        {
            yield return null;
        }
        OnChangedPlayerCount?.Invoke(curCount);
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("On Connected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버접속 완료!!");
        PhotonNetwork.NickName = GameManager.Instance.Player.Name;
        JoinLobby();
    }

    void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속완료!");
        GameManager.Instance.MoveLobby();
    }

    public void CreateRoom(string RoomName = "")
    {
        string name = ((string.IsNullOrWhiteSpace(RoomName)) ? $"Room{CountRoomNum}" : RoomName);
        PhotonNetwork.CreateRoom(name, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        CountRoomNum++;
        Debug.Log($"{PhotonNetwork.CurrentRoom.Name}방 개설 완료!");
    }

    // lobby에 있을 때 클라이언트에서 실행됨.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        StartCoroutine(CoUpdateRoomList(roomList));
    }

    IEnumerator CoUpdateRoomList(List<RoomInfo> roomList)
    {
        while(UIManager.Instance.IsSceneLoaded==false)
        {
            yield return null;
        }

        UIManager.Instance.UpdateRoomListUI(roomList);
    }

    public void LeaveRoom()
    {
        OnChatSubmit($"{PhotonNetwork.LocalPlayer.NickName}님 퇴장!");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        PhotonNetwork.JoinLobby();    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        UIManager.Instance.SetJoinRoomUI(PhotonNetwork.CurrentRoom.Name);
        OnChatSubmit($"{PhotonNetwork.LocalPlayer.NickName}님 입장!");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        Debug.Log($"{player.NickName}님 : 방 입장완료!");
        
        UIManager.Instance.OnUpdatePlayerListInRoom(true, player);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //Debug.Log($"{otherPlayer.NickName}님 : 방 입장완료!");
        UIManager.Instance.OnUpdatePlayerListInRoom(false, otherPlayer);
    }



    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogError(message);
    }

    public void DisConnect()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByClientLogic)
        {
            Debug.Log("정상 disconnect");
            GameManager.Instance.MoveTown();
        }
        else
        {
            Debug.LogWarning($"Exception of Disconnected {cause}");
        }
    }


    public void OnReadyPlayer()
    {
        PV.RPC("CallUIUpdate", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        OnChatSubmit($"{PhotonNetwork.LocalPlayer.NickName}님 준비완료!");
    }

    [PunRPC]
    void CallUIUpdate(Photon.Realtime.Player player)
    {
        UIManager.Instance.OnReadyPlayer(player);
    }

    public void OnCancelReadyPlayer()
    {
        PV.RPC("CallCancelUIUpdate", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        OnChatSubmit($"{PhotonNetwork.LocalPlayer.NickName}님 준비취소!");
    }

    [PunRPC]
    void CallCancelUIUpdate(Photon.Realtime.Player player)
    {
        UIManager.Instance.OnCancelReadyPlayer(player);
    }

    public void OnStartButton()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PV.RPC("LoadLastScene", RpcTarget.All);
        }
    }

    [PunRPC]
    void LoadLastScene()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(5);
    }

    public void OnChatSubmit(string m)
    {
        PV.RPC("SendChatMessage", RpcTarget.All,PhotonNetwork.LocalPlayer.NickName + " : " + m);
    }


    [PunRPC]
    void SendChatMessage(string message)
    {
        UIManager.Instance.OnSubmittedMessage(message);
    }
}
