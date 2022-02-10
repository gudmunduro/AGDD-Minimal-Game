using System;
using UnityEngine;

public class RedTriangleSensor : MonoBehaviour
{
    public bool IsSensing => _groundSenseCounter > 0;
    private int _groundSenseCounter = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Triangle"))
        {
            var triangleController = col.gameObject.GetComponent<TriangleController>();
            if (triangleController != null && triangleController.orientation == Orientation.Up)
            {
                _groundSenseCounter += 1;   
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Triangle"))
        {
            var triangleController = other.gameObject.GetComponent<TriangleController>();
            if (triangleController != null && triangleController.orientation == Orientation.Up)
            {
                _groundSenseCounter -= 1;
            }
        }
    }
}