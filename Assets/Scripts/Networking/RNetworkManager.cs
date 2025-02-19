using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public enum ClientToServerId : ushort
{
    name = 1
}
public class RNetworkManager : MonoBehaviour
{
    private static RNetworkManager _singleton;

    public static RNetworkManager Singleton
    {
        get => _singleton;

        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton == value)
            {
                Debug.Log($"{nameof(RNetworkManager)} instance already exists!");
            }
        }
    }

    public Server server { get; private set; }
    public Client client { get; private set; }

    [SerializeField]
    private ushort port;

    [SerializeField]
    private ushort maxClientNum;

    [SerializeField]
    private string ip;

    private void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
    }

    public void StartServer()
    {
        server = new Server();
        server.Start(port, maxClientNum);
    }

    public void StartClient()
    {
        client = new Client();
        client.Connected += DidConnect;
        client.ConnectionFailed += FailedConnection;
        client.Disconnected += Disconnected;
    }

    public void Connect()
    {
        client.Connect($"{ip}:{port}"); 
    }

    public void DidConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.SendName();
    }

    public void FailedConnection(object sender, EventArgs e)
    {
        UIManager.Singleton.MenuReturn();
    }

    public void Disconnected(object sender, EventArgs e)
    {
        UIManager.Singleton.MenuReturn();
    }

    void FixedUpdate()
    {
        if (server != null)
        {
            server.Update();
        }
        else if (client != null)
        {
            client.Update();
        }
    }

    private void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();
        }
        else if (client != null)
        {
            client.Disconnect();
        }
    }


}
