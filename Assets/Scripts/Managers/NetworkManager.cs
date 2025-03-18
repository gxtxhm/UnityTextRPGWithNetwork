using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using System.Diagnostics;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    int CountRoomNum = 1;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
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

    public void CreateRoom(string RoomName="")
    {
        string name = (RoomName == "") ? $"Room{CountRoomNum}" : RoomName;
        PhotonNetwork.CreateRoom(name, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        CountRoomNum++;
        Debug.Log($"{PhotonNetwork.CurrentRoom.Name}방 개설 완료!");
        UIManager.Instance.SetJoinRoomUI(PhotonNetwork.CurrentRoom.Name);
    }

    // lobby에 있을 때 클라이언트에서 실행됨.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        UIManager.Instance.UpdateRoomListUI(roomList);
    }

    public void LeaveRoom()=>PhotonNetwork.LeaveRoom();

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        PhotonNetwork.JoinLobby();
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        Debug.Log("방 입장완료!");

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
        if(cause == DisconnectCause.DisconnectByClientLogic)
        {
            Debug.Log("정상 disconnect");
            GameManager.Instance.MoveTown();
        }
        else
        {
            Debug.LogWarning($"Exception of Disconnected {cause}");
        }
    }
}
