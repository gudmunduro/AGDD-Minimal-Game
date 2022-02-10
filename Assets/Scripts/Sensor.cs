using System;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool IsSensing => _groundSenseCounter > 0;
    private int _groundSenseCounter = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Triangle"))
        {
            _groundSenseCounter += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Triangle"))
        {
            _groundSenseCounter -= 1;
        }
    }
}