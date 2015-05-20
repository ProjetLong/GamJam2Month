using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Photon;

public class NetworkManager : Photon.MonoBehaviour
{

    //Singleton
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(this);
    }

    public string version = "0.1";

    public GameObject playerPrefab;
    public string playerName;
    public string roomToJoin = "Lobby";

    public void authenticate(string id)
    {
        this.playerName = id;
        PhotonNetwork.ConnectUsingSettings(this.version);
        //authenticated
        Debug.Log("Authenticated");
        successfullAuthentication();
    }

    //temp
    void Start()
    {
    }

    private void successfullAuthentication()
    {
        GameManager.Instance.goToLobbyScene();
    }

    void Update()
    {
    }

    #region "Photon"
    void OnJoinedLobby()
    {
        Debug.Log("onJoinedLobby");
        PhotonNetwork.JoinOrCreateRoom(this.roomToJoin, new RoomOptions() { }, TypedLobby.Default);
    }

    public void joinGame(string mapName)
    {
        ConnectedPlayersManager.Instance.removePlayer(NetworkManager.Instance.playerName);
        GameManager.Instance.goToMainScene();
    }

    void OnJoinedRoom()
    {
        Debug.Log("On joined Room : " + PhotonNetwork.room.name);
        switch (PhotonNetwork.room.name)
        {
            case "Lobby":
                ConnectedPlayersManager.Instance.addPlayer(this.playerName);
                break;
            case "Game":
                // Spawn player
                GameObject player = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, Vector3.up * 1, Quaternion.identity, 0);
                PhotonView pv = player.GetComponent<PhotonView>();
                player.GetComponent<soldierMovement>().enabled = pv.isMine;
                player.GetComponent<crouchController>().enabled = pv.isMine;
                player.GetComponent<weaponController>().enabled = pv.isMine;
                player.transform.FindChild("soldierCamera").gameObject.SetActive(pv.isMine);
                //GameObject entities = GameObject.Find("Players");
                //player.transform.parent = entities.transform;

                //init miniMap
                if (pv.isMine)
                {
                    GameObject miniMapCamera = GameObject.Find("MiniMapCamera");
                    MiniMapFollow follow = miniMapCamera.GetComponent<MiniMapFollow>();
                    follow.target = player;
                }

                //attach main camera
                //CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
                //camFollow.setTarget(player.transform);
                break;
        }
    }

    void OnLeftRoom()
    {

    }
    #endregion

    public void acceptInvitation()
    {
        this.joinGame(null);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 3)
        {
            //mainScene
            this.roomToJoin = "Game";
            PhotonNetwork.LeaveRoom();
        }

    }
}
