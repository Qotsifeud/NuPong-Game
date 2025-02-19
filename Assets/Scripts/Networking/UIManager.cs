using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Riptide;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;

    public static UIManager Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton == value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists, getting rid of duplicates.");
            }
        }
    }

    [Header("Connect")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private GameObject gameUI;

    
    public void HostClicked()
    {
        HideMenuUI();

        RNetworkManager.Singleton.StartServer();
    }

    public void ConnectClicked()
    {
        HideMenuUI();

        RNetworkManager.Singleton.StartClient();
        RNetworkManager.Singleton.Connect();
    }

    public void MenuReturn()
    {
        connectUI.SetActive(true);
        usernameField.interactable = true;
    }

    public void SendName()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ClientToServerId.name);

        RNetworkManager.Singleton.client.Send(message);
    }

    public void HideMenuUI()
    {
        connectUI.SetActive(false);
        usernameField.interactable = false;

        gameUI.SetActive(true);
    }
}
