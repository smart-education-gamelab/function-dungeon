using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestedItems : MonoBehaviour
{
    [SerializeField] internal List<Requests> requests;
   
    public void AddRequest(Requests request)
    {
        requests.Add(request);
    }
}

[System.Serializable]
public class Requests
{
    public Items item;
    internal Vector2 coordinates;
    internal SetRequestUI giver;
    internal float interval;
    internal float[] x;
    internal float[] y;
    internal float a;
    internal float b;

    internal int index;
}

