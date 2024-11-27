using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Events : MonoBehaviour
{
    //UI Items
    //********************************************************
    public Button startButton;
    public TMP_Text titleText;

    public TMP_Text[] scoreText = new TMP_Text[2];
    int value; // Value is used for increasing the UI game scores.
    //*************************************************

    //Game Objects
    //********************************************************
    public GameObject[] blockerObjects = new GameObject[2];
    public GameObject ball;
    public GameObject topBoundary;
    //********************************************************

    public void Update()
    {
        startButton.onClick.AddListener(onStart);
    }

    public void goalScored(int goal)
    {
        if(goal == 0)
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

    public void onStart()
    {
        titleText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);

        foreach(TMP_Text obj in scoreText)
        {
            obj.gameObject.SetActive(true);
        }

        topBoundary.SetActive(true);

        foreach (GameObject blocker in blockerObjects)
        {
            blocker.SetActive(true);
        }

        ball.SetActive(true);
    }
}
