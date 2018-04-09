using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameNetwork : NetworkBehaviour
{
    public static GameNetwork Instance;

    void Awake()
    {
        Instance = this;
    }

    public void InitNetwork()
    {
        NetworkServer.Listen(47777);
        //isAtStartup = false;
    }

    public void ConnectNetwork()
    {
        
    }

    public virtual void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("Vao day");
    }
}
