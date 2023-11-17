using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRequest : FindPossibleCoordinates
{ 
    internal SetRequestUI giver;
    [SerializeField] internal Requests request;
    internal Vector2 location;

    internal float interval;
    internal float[] x;
    internal float[] y;
    internal float a;
    internal float b;

    [Header("Math properties")]
    [SerializeField] private int minA, maxA, minInterval, maxInterval;

    internal int index;

    private int state;

    private void Awake()
    {
        x = new float[5];
        y = new float[5];
    }

    public Requests Set()
    {
        if (state == 0)
        {
            this.location = GetCoordinate();
            this.request.coordinates = location;
            this.request.giver = this.gameObject.GetComponent<SetRequestUI>();

            while (y[0] == y[1])
            {
                LinearMath();
            }

            this.request.index = this.index;
            this.request.x = this.x;
            this.request.y = this.y;
            this.request.x[index] = this.x[index];
            this.request.y[index] = this.y[index];
            this.request.a = this.a;
            this.request.b = this.b;
            this.request.interval = this.interval;


            SetRequestUI UI = gameObject.GetComponent<SetRequestUI>();
            UI.request = this.request;
            UI.SetUI();
            state++;
            return request;
        }
        return null;
    }

    private void LinearMath()
    {
        this.index = Random.Range(0, 5);
        this.x[index] = this.location.x;
        this.y[index] = this.location.y;
        this.a = Random.Range(minA, maxA);
        this.b = y[index] - a * x[index];
        this.interval = Random.Range(minInterval, maxInterval);

        for (int i = 0; i < 5; i++)
        {           
            x[i] = x[index] + (index * -1 + i) * interval;
            y[i] = a * x[i] + b;
            Debug.Log(x[i] + " , " + y[i]);
        }
    }
}
