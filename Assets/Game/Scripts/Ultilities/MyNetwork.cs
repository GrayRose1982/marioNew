using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetwork : NetworkBehaviour {

    public virtual void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("Vao day");
    }
}
