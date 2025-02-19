using System;
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
    public NetworkVariable<int> hostScore = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> clientScore = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> startTimer = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public MultiplayerManager multiplayerManager;
    public DatabaseConnection connection;

    bool usernameFilled;

    //UI Items
    //********************************************************
    public Button startBtn, splayerBtn, mplayerBtn, localBtn, networkBtn, hostBtn, clientBtn, leaderboardBtn, backBtn, submitBtn;
    public TMP_Text titleText, awaitingPlayerText, usernameText, gameOverText, timerText;

    public TMP_InputField usernameField;

    public TMP_Text[] playerTitles = new TMP_Text[2];
    public TMP_Text[] scoreText = new TMP_Text[2];
    int value; // Value is used for increasing the UI game scores.

    public GameObject leaderboardItems;
    //********************************************************

    //Game Objects
    //********************************************************
    public GameObject player;
    public GameObject ballPrefab;
    public GameObject topBoundary;

    public bool gameOver;
    public float timer;
    float timeRemaining = 60.0f;
    //********************************************************

    public void Start()
    {

    }

    public void Update()
    {
        startBtn.onClick.AddListener(startBtnClicked);
        splayerBtn.onClick.AddListener(singlePlayerBtnClicked);
        mplayerBtn.onClick.AddListener(multiPlayerBtnClicked);
        localBtn.onClick.AddListener(localBtnClicked);
        networkBtn.onClick.AddListener(networkOrBackBtnClicked);
        hostBtn.onClick.AddListener(hostBtnClicked);
        clientBtn.onClick.AddListener(clientBtnClicked);
        leaderboardBtn.onClick.AddListener(leaderboardBtnClicked);
        backBtn.onClick.AddListener(networkOrBackBtnClicked);
        submitBtn.onClick.AddListener(submitBtnClicked);

        if(IsServer && multiplayerManager.numberOfPlayers.Value == 2)
        {
            awaitingPlayerText.gameObject.SetActive(false);
        }

        if(startTimer.Value == true)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timer = timeRemaining;
                DisplayTimer(timer);
            }
            else
            {
                GameOverEvent();
            }
        }
    }

    public void DisplayTimer(float timer)
    {
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void goalScored(int goal)
    {
        if (goal == 0)
        {
            GoalScoredServerRpc(0);
        }
        else if (goal == 1)
        {
            GoalScoredServerRpc(1);
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

    public void networkOrBackBtnClicked()
    {
        localBtn.gameObject.SetActive(false);
        networkBtn.gameObject.SetActive(false);
        leaderboardItems.SetActive(false);
        backBtn.gameObject.SetActive(false);

        usernameField.gameObject.SetActive(true);
        submitBtn.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);

        hostBtn.gameObject.SetActive(true);
        clientBtn.gameObject.SetActive(true);
        leaderboardBtn.gameObject.SetActive(true);

    }

    public void leaderboardBtnClicked()
    {
        hostBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);
        leaderboardBtn.gameObject.SetActive(false);
        submitBtn.gameObject.SetActive(false);

        leaderboardItems.SetActive(true);
        backBtn.gameObject.SetActive(true);

        connection.leaderboardRequested();
    }

    public void submitBtnClicked()
    {
        submitBtn.gameObject.SetActive(false);

        if (usernameFilled != true)
        {
            PlayerPrefs.SetString("playerId", usernameField.text);
            usernameFilled = true;
        }

        usernameField.text = "Submitted";

        connection.setUsername(PlayerPrefs.GetString("playerId"));
    }

    public void hostBtnClicked()
    {
        hostBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);
        leaderboardBtn.gameObject.SetActive(false);
        leaderboardItems.SetActive(false);
        usernameField.gameObject.SetActive(false);
        submitBtn.gameObject.SetActive(false);
        usernameText.gameObject.SetActive(false);

        multiplayerManager.HostGame();
        gameStart(true);
    }

    public void clientBtnClicked()
    {
        clientBtn.gameObject.SetActive(false);
        hostBtn.gameObject.SetActive(false);
        leaderboardBtn.gameObject.SetActive(false);
        leaderboardItems.SetActive(false);
        usernameField.gameObject.SetActive(false);
        submitBtn.gameObject.SetActive(false);
        usernameText.gameObject.SetActive(false);

        multiplayerManager.ClientJoin();
        gameStart(true);
    }  

    public void gameStart(bool isNetworkGame)
    {
        timeRemaining = 60.0f;
        gameOver = false;

        foreach (TMP_Text obj in scoreText)
        {
            obj.gameObject.SetActive(true);
        }

        foreach (TMP_Text obj in playerTitles)
        {
            obj.gameObject.SetActive(true);
        }
        
        titleText.gameObject.SetActive(false);

        timerText.gameObject.SetActive(true);

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

    public void GameOverEvent()
    {
        topBoundary.SetActive(false);

        backBtn.gameObject.SetActive(true);

        gameOverText.gameObject.SetActive(true);

        startTimer.Value = false;

        gameOver = true;

        timerText.gameObject.SetActive(false);

        titleText.gameObject.SetActive(true);

        foreach (TMP_Text obj in scoreText)
        {
            obj.gameObject.SetActive(false);
        }

        foreach (TMP_Text obj in playerTitles)
        {
            obj.gameObject.SetActive(false);
        }

        if (hostScore.Value > clientScore.Value)
        {
            gameOverText.text = "Game Over!" + "\n" + "Host Wins!";
        }
        else if (clientScore.Value > hostScore.Value)
        {
            gameOverText.text = "Game Over!" + "\n" + "Client Wins!";
        }
        else
        {
            gameOverText.text = "Game Over! It's a Draw!";
        }

        if(IsServer)
        {
            connection.scoreSubmission(hostScore.Value);
        }
        else
        {
            connection.scoreSubmission(clientScore.Value);
        }


    }

    [ServerRpc]
    public void GoalScoredServerRpc(int goal)
    {
        if (goal == 0)
        {
            clientScore.Value += 1;
        }
        else if (goal == 1)
        {
            hostScore.Value += 1;
        }

        GoalScoredClientRpc();
    }

    [ClientRpc]
    public void GoalScoredClientRpc()
    {
        scoreText[0].text = hostScore.Value.ToString();
        scoreText[1].text = clientScore.Value.ToString();
    }
}
