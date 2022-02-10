using System;
using UnityEngine;

public class GoldTriangleController : MonoBehaviour
{
    public float moveSpeed;
    private Vector3 _initialPosition;
    private GoldTriangleSensor _goldTriangleSensor;
    private const int GoldTrianglePoints = 3;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
    }

    private void Start()
    {
        _goldTriangleSensor = transform.Find("GoldTriangle").GetComponent<GoldTriangleSensor>();
    }

    private void Update()
    {
        var newY = Mathf.PingPong(Time.time * moveSpeed, 2) + _initialPosition.y;
        transform.localPosition = new Vector3(_initialPosition.x, newY, _initialPosition.z);

        if (_goldTriangleSensor.sensedPlayer)
        {
            GameManager.Instance.score += GoldTrianglePoints;
            gameObject.SetActive(false);
        }
        else if (_goldTriangleSensor.sensedGround)
        {
            gameObject.SetActive(false);
        }
    }
}