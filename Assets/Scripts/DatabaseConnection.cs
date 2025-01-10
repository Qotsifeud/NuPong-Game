using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEditor;
using TMPro;

public class DatabaseConnection : MonoBehaviour
{
    string leaderboardId = "nupongHighscore";

    IEnumerator requestCoroutine = null;
    IEnumerator submitCoroutine = null;
    bool usernameSet = false;

    public TMP_Text leaderboardNames;
    public TMP_Text leaderboardScores;

    private void Awake()
    {
        StartCoroutine(LoginRoutine());
    }

    public void leaderboardRequested()
    {
        if (requestCoroutine == null)
        {
            requestCoroutine = GetLeaderboard();
            StartCoroutine(GetLeaderboard());
        }
    }

    public void scoreSubmission(int score)
    {
        if (submitCoroutine == null)
        {
            StartCoroutine(SubmitToLeaderboard(score));
            submitCoroutine = SubmitToLeaderboard(score);
        }
    }

    public void setUsername(string username)
    {
        if(usernameSet == false)
        {
            LootLockerSDKManager.SetPlayerName(username, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("The player name was set to: " + username);
                }
                else
                {
                    Debug.Log("The player name could not be set due to: " + response.errorData.message);
                }
            });

            usernameSet = true;
        }
    }

    IEnumerator LoginRoutine()
    {
        bool loginDone = false;

        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("The player was logged in to the LootLocker server.");
                loginDone = true;
            }
            else
            {
                Debug.Log("The login has failed.");
            }
        });

        yield return new WaitWhile(() => loginDone == false);
    }

    IEnumerator SubmitToLeaderboard(int score)
    {
        bool submitDone = false;

        LootLockerSDKManager.SubmitScore(PlayerPrefs.GetString("playerId"), score, leaderboardId, (response) =>
        {
            if (response.success)
            {
                Debug.Log("The score was submitted to the leaderboard.");
                submitDone = true;
            }
            else
            {
                Debug.Log("The score submission has failed due to: " + response.errorData.message);
            }
        });

        yield return new WaitWhile(() => submitDone == false);

    }

    IEnumerator GetLeaderboard()
    {
        bool getLeaderboardComplete = false;

        LootLockerSDKManager.GetScoreList(leaderboardId, 10, (response) =>
        {
            if (response.success)
            {
                string playerNames = "Names\n";
                string playerScores = "Scores\n";

                LootLockerLeaderboardMember[] members = response.items;

                foreach(var member in members)
                {
                    playerNames += member.rank + ". ";

                    if(member.player.name != "")
                    {
                        playerNames += member.player.name + "\n";
                    }
                    else
                    {
                        playerNames += member.player.id + "\n";
                    }

                    playerScores += member.score + "\n";
                }

                getLeaderboardComplete = true;

                leaderboardNames.text = playerNames;
                leaderboardScores.text = playerScores;
            }
            else
            {
                Debug.Log("Failed due to: " + response.errorData.message);
                getLeaderboardComplete = true;
            }

        });


        yield return new WaitWhile(() => getLeaderboardComplete == false);
    }
}
