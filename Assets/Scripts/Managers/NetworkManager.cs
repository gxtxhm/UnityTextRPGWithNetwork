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
        Debug.Log("�������� �Ϸ�!!");
        PhotonNetwork.NickName = GameManager.Instance.Player.Name;
        JoinLobby();
    }

    void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���ӿϷ�!");
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
        Debug.Log($"{PhotonNetwork.CurrentRoom.Name}�� ���� �Ϸ�!");
        UIManager.Instance.SetJoinRoomUI(PhotonNetwork.CurrentRoom.Name);
    }

    // lobby�� ���� �� Ŭ���̾�Ʈ���� �����.
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
        Debug.Log("�� ����Ϸ�!");

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
            Debug.Log("���� disconnect");
            GameManager.Instance.MoveTown();
        }
        else
        {
            Debug.LogWarning($"Exception of Disconnected {cause}");
        }
    }
}
