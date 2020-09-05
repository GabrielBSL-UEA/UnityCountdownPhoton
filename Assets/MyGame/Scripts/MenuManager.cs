using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public GameObject canvasSearchPanel;
    public GameObject canvasWaitingPanel;
    public GameObject canvasCountdownPanel;
    public GameObject canvasLoadingPanel;

    public GameObject spawnObject;
    public Transform[] spawnPoints;

    int spawnPositionUsed;

    private void Start()
    {
        SetActivePanel(canvasSearchPanel.name);

        PhotonNetwork.SendRate = 25;
        PhotonNetwork.SerializationRate = 25;
    }

    private void SetActivePanel(string activePanel)
    {
        canvasSearchPanel.SetActive(activePanel.Equals(canvasSearchPanel.name));
        canvasWaitingPanel.SetActive(activePanel.Equals(canvasWaitingPanel.name));
        canvasCountdownPanel.SetActive(activePanel.Equals(canvasCountdownPanel.name));
        canvasLoadingPanel.SetActive(activePanel.Equals(canvasLoadingPanel.name));
    }

    public void ButtonSearch()
    {
        Debug.Log("ButtonSearch");
        SetActivePanel(canvasLoadingPanel.name);
        PhotonNetwork.ConnectUsingSettings();
    }
    public void ButtonCancel()
    {
        Debug.Log("ButtonCancel");
        SetActivePanel(canvasLoadingPanel.name);
        PhotonNetwork.Disconnect();
    }

    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");

        RoomOptions options = new RoomOptions();
        options.IsOpen = true;
        options.IsVisible = true;
        options.MaxPlayers = 2;

        string rooomName = "Room" + UnityEngine.Random.Range(1000, 9999);

        PhotonNetwork.CreateRoom(rooomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        SetActivePanel(canvasWaitingPanel.name);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            spawnPositionUsed = 0;

        else
            spawnPositionUsed = 1;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected - " + cause.ToString());
        SetActivePanel(canvasSearchPanel.name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");

        Hashtable props = new Hashtable
        {
            {CountdownTimer.CountdownStartTime, (float) PhotonNetwork.Time } 
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomPropertiesUpdate");
        SetActivePanel(canvasCountdownPanel.name);
    }

    void CountdownAction()
    {
        Debug.Log("CountdownAction");
        SetActivePanel("");

        PhotonNetwork.Instantiate(spawnObject.name, spawnPoints[spawnPositionUsed].position, spawnPoints[spawnPositionUsed].rotation);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        CountdownTimer.OnCountdownTimerHasExpired += CountdownAction;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        CountdownTimer.OnCountdownTimerHasExpired -= CountdownAction;
    }
}
