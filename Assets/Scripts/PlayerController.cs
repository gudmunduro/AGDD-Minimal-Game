using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;
    public float jumpForce = 3.0f;
    public Camera camera;
    
    private Rigidbody2D _rigidbody2D;
    private float _playerMove;
    private Sensor headSensor;
    private Sensor groundSensor;
    private RedTriangleSensor _redTriangleSensor;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        headSensor = transform.Find("HeadSensor").GetComponent<Sensor>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        _redTriangleSensor = transform.Find("RedTriangleSensor").GetComponent<RedTriangleSensor>();
    }

    void Update()
    {
        var cameraPos = camera.gameObject.transform.position;
        var camHeight = 2f * camera.orthographicSize;
        var camWidth = camHeight * camera.aspect;
        var mousePos = Input.mousePosition;
        var playerPos = camera.WorldToScreenPoint(transform.position);
        var distanceToPlayer = mousePos.x - playerPos.x;
        _playerMove = 0;

        if (distanceToPlayer > 50.0f && transform.position.x < cameraPos.x + camWidth / 2)
        {
            _rigidbody2D.velocity = new Vector2(speed, _rigidbody2D.velocity.y);
        }
        else if (distanceToPlayer < -50.0f && transform.position.x > cameraPos.x - camWidth / 2 - 1.5f)
        {
            _rigidbody2D.velocity = new Vector2( -speed, _rigidbody2D.velocity.y); 
        }
        else
        {
            _rigidbody2D.velocity = new Vector2( 0, _rigidbody2D.velocity.y);
        }
        
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && groundSensor.IsSensing)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
        }

        if (headSensor.IsSensing || _redTriangleSensor.IsSensing)
        {
            GameManager.Instance.EndGame();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Triangle"))
        {
            var triangleController = col.gameObject.GetComponent<TriangleController>();
            if (triangleController == null) return;

            // Player touched a green triangle
            if (triangleController.orientation == Orientation.Down)
            {
                GameManager.Instance.TouchedTriangleWithId(triangleController.Id);
            }
        }
    }
}
