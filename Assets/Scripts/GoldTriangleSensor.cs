using System;
using UnityEngine;

public class GoldTriangleSensor : MonoBehaviour
{
    public bool sensedGround;
    public bool sensedPlayer;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Triangle"))
        {
            sensedGround = true;
        }
        else if (col.CompareTag("Player"))
        {
            sensedPlayer = true;
        }
    }
}