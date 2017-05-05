using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using TestPhotonLib.Common;
using UnityEngine;
using UnityEngine.Events;

public class PhotonServer : MonoBehaviour, IPhotonPeerListener
{
    public string HostAddress = "localhost";
    public string HostPort = "5057";
    public string HostName = "MyCoolServer";

    //singletone
    private static PhotonServer _instance;
    public static PhotonServer Instance
    {
        get { return _instance; }
    }
    

    private PhotonPeer PhotonPeer { get; set; }


    //Делегаты
    public delegate void OnConnectDelegate();
    public delegate void OnDisconnectDelegate();
    public delegate void OnDamageDelegate(string _name);
    public delegate void OnDeathDelegate(string _name);
    public delegate void OnUpdateHealthDelegate(string _name, float health);
    public delegate void OnRespawnDelegate(string _name);
    public delegate void OnChangeDelegate(string _name, int _bodyState);

    //События
    public event OnConnectDelegate onConnect = delegate { };
    public event OnDisconnectDelegate onDisconnect = delegate { };
    public event OnDamageDelegate onDamage = delegate { };
    public event OnDeathDelegate onDeath = delegate { };
    public event OnUpdateHealthDelegate onUpdateHealth = delegate { };
    public event OnRespawnDelegate onRespawn = delegate { };
    public event OnRespawnDelegate onChangeState = delegate { };


    void Awake()
    {
        if (Instance != null)
        {
            DestroyObject(gameObject);
            return;
        }
            

        DontDestroyOnLoad(gameObject);

        Application.runInBackground = true;

        _instance = this;
    }
	// Use this for initialization
	void Start ()
	{        
	    PhotonPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
	    Connect();
	}
	
	// Update is called once per frame
	void Update () {
	    if(PhotonPeer != null)
            PhotonPeer.Service();
	}

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public void Connect()
    {
        if (PhotonPeer != null)
            PhotonPeer.Connect(HostAddress + ":" + HostPort, HostName);
    }

    public void Disconnect()
    {
        if(PhotonPeer != null)
            PhotonPeer.Disconnect();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("DebugReturn level:" + level.ToString());
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.Hit:
                break;
            default:
                Debug.Log("Unknown OperationResponse:" + operationResponse.OperationCode);
                break;
        }
    }
    

    //событие от сервера
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.Damage:
                    OnDamageUnit(eventData);
                break;
            case (byte)EventCode.Death:
                    OnDeathUnit(eventData);
                break;
            case (byte)EventCode.UpdateHealth:
                  OnUpdateHealth(eventData);
                break;
            case (byte)EventCode.Respawn:
                   OnRespawnUnit(eventData);
                break;
            default:
                Debug.Log("Unknown Event:" + eventData.Code);
                break;
        }
    }

    //действия на события
    public void OnDeathUnit(EventData eventData)
    {
        string characterName = (string)eventData.Parameters[(byte)ParameterCode.CharacterName];
        Debug.Log("Death charName:" + characterName);
        onDeath(characterName);
    }

    public void OnDamageUnit(EventData eventData)
    {
        string characterName = (string)eventData.Parameters[(byte)ParameterCode.CharacterName];
        Debug.Log("Damage charName:" + characterName);
        onDamage(characterName);
    }

    public void OnUpdateHealth(EventData eventData)
    {
        string characterName = (string)eventData.Parameters[(byte)ParameterCode.CharacterName];
        float _health = (float)eventData.Parameters[(byte)ParameterCode.Health];
        Debug.Log("Update health charName:" + characterName);
        onUpdateHealth(characterName, _health);
    }

    public void OnRespawnUnit(EventData eventData)
    {
        string characterName = (string)eventData.Parameters[(byte)ParameterCode.CharacterName];
        Debug.Log("Respwn charName:" + characterName);
        onRespawn(characterName);
    }



    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("Connected to server!");
                onConnect();
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnected from server!");
                onDisconnect();
                break;
            case StatusCode.TimeoutDisconnect:
                Debug.Log("TimeoutDisconnected from server!");
                break;
            case StatusCode.DisconnectByServer:
                Debug.Log("DisconnectedByServer from server!");
                break;
            case StatusCode.DisconnectByServerUserLimit:
                Debug.Log("DisconnectedByLimit from server!");
                break;
            case StatusCode.DisconnectByServerLogic:
                Debug.Log("DisconnectedByLogic from server!");
                break;
            case StatusCode.EncryptionEstablished:
                break;
            case StatusCode.EncryptionFailedToEstablish:
                break;
            default:
                Debug.Log("Unknown status:" + statusCode.ToString());
                break;
        }
    }

    

    //запросить нанесение удара
    public void SetHit(string targetName)
    {
        PhotonPeer.OpCustom((byte) OperationCode.Hit,
                            new Dictionary<byte, object> { { (byte) ParameterCode.CharacterName, targetName } }, true);
    }

   

    
}
