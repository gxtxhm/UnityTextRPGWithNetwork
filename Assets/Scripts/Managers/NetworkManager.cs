using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom($"Room{CountRoomNum}", new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        CountRoomNum++;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        Debug.Log("");
    }
}
