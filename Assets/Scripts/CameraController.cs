using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float dampTime = 0.4f;
    private Vector3 _cameraPos;
    private Camera _camera;
    private Vector3 _velocity = Vector3.zero;

    private void Update()
    {
        var position = player.position;
        
        _cameraPos = new Vector3(transform.position.x, position.y + 1.0f, -10f);
        transform.position = Vector3.SmoothDamp(gameObject.transform.position, _cameraPos, ref _velocity, dampTime);
    }
}