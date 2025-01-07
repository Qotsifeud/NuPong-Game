using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Events : NetworkBehaviour
{
    public MultiplayerManager multiplayerManager;

    //UI Items
    //********************************************************
    public Button startBtn, splayerBtn, mplayerBtn, localBtn, networkBtn, hostBtn, clientBtn;
    public TMP_Text titleText, awaitingPlayerText;

    public TMP_Text[] playerTitles = new TMP_Text[2];
    public TMP_Text[] scoreText = new TMP_Text[2];
    int value; // Value is used for increasing the UI game scores.
    //********************************************************

    //Game Objects
    //********************************************************
    public GameObject player;
    public GameObject ballPrefab;
    public GameObject topBoundary;
    //********************************************************

    public void Start()
    {
        multiplayerManager = FindObjectOfType<MultiplayerManager>();
    }

    public void Update()
    {
        startBtn.onClick.AddListener(startBtnClicked);
        splayerBtn.onClick.AddListener(singlePlayerBtnClicked);
        mplayerBtn.onClick.AddListener(multiPlayerBtnClicked);
        localBtn.onClick.AddListener(localBtnClicked);
        networkBtn.onClick.AddListener(networkBtnClicked);
        hostBtn.onClick.AddListener(hostBtnClicked);
        clientBtn.onClick.AddListener(clientBtnClicked);

        

    }

    public void goalScored(int goal)
    {
        if (goal == 0)
        {
            value = int.Parse(scoreText[1].text);
            value += 1;
            scoreText[1].text = value.ToString();
        }
        else if (goal == 1)
        {
            value = int.Parse(scoreText[0].text);
            value += 1;
            scoreText[0].text = value.ToString();
        }
    }

    public void startBtnClicked()
    {
        splayerBtn.gameObject.SetActive(true);
        mplayerBtn.gameObject.SetActive(true);

        startBtn.gameObject.SetActive(false);

    }

    public void singlePlayerBtnClicked()
    {
        titleText.gameObject.SetActive(false);

        gameStart(false);
    }

    public void multiPlayerBtnClicked()
    {
        splayerBtn.gameObject.SetActive(false);
        mplayerBtn.gameObject.SetActive(false);

        localBtn.gameObject.SetActive(true);
        networkBtn.gameObject.SetActive(true);
    }

    public void localBtnClicked()
    {
        localBtn.gameObject.SetActive(false);
        networkBtn.gameObject.SetActive(false);

        gameStart(false);
    }

    public void networkBtnClicked()
    {
        localBtn.gameObject.SetActive(false);
        networkBtn.gameObject.SetActive(false);

        hostBtn.gameObject.SetActive(true);
        clientBtn.gameObject.SetActive(true);
    }

    public void hostBtnClicked()
    {
        hostBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);

        multiplayerManager.HostGame();
        gameStart(true);
    }

    public void clientBtnClicked()
    {
        clientBtn.gameObject.SetActive(false);
        hostBtn.gameObject.SetActive(false);

        multiplayerManager.ClientJoin();
        gameStart(true);
    }  

    public void gameStart(bool isNetworkGame)
    {
        foreach (TMP_Text obj in scoreText)
        {
            obj.gameObject.SetActive(true);
        }

        foreach (TMP_Text obj in playerTitles)
        {
            obj.gameObject.SetActive(true);
        }
        
        titleText.gameObject.SetActive(false);

        topBoundary.SetActive(true);

        //if (isNetworkGame)
        //{
        //    if (connectedPlayers == 1)
        //    {
        //        Debug.Log("not connected yet.");
        //        awaitingPlayerText.gameObject.SetActive(true);
        //    }
        //    else if (connectedPlayers == 2)
        //    {
        //        awaitingPlayerText.gameObject.SetActive(false);
        //    }

        //    switch (connectedPlayers)
        //    {
        //        case 1:
        //            awaitingPlayerText.gameObject.SetActive(true);
        //            break;
        //        case 2:
        //            awaitingPlayerText.gameObject.SetActive(false);
        //            break;

        //    }
        //}
    }

    
}
