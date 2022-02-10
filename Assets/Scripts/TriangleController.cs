using System;
using UnityEngine;
using Random = System.Random;

public class TriangleController : MonoBehaviour
{
    public int position;
    public Orientation orientation;
    public int level;
    public string Id { get; private set; }
    private Rigidbody2D _rigidbody2D;
    private bool _fixedPosition;
    private GameObject _goldTriangleContainer;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        // Generate a unique id for this triangle
        var uuid = Guid.NewGuid();
        Id = uuid.ToString();
    }

    private void Start()
    {
        _goldTriangleContainer = transform.Find("GoldTriangleContainer").gameObject;

        // 1/3rd chance of spawning gold triangle on any green triangle
        if (orientation == Orientation.Down && UnityEngine.Random.Range(0, 4) == 3)
        {
           _goldTriangleContainer.SetActive(true); 
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Triangle") && !_fixedPosition)
        {
            _fixTrianglePosition();
            _fixedPosition = true;
        }
    }

    private void _fixTrianglePosition()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        transform.position = _calculateOptimalTrianglePosition(position, orientation, level);
    }
    
    private Vector2 _calculateOptimalTrianglePosition(int position, Orientation orientation, int level)
    {
        var triangleSize = transform.gameObject.GetComponent<SpriteRenderer>().bounds.size;
        var x = TriangleDropController.StartX + position * (triangleSize.x - 0.6f) + orientation switch
        {
            Orientation.Up => 0,
            Orientation.Down => (triangleSize.x - 0.6f) / 2
        };
        var y = TriangleDropController.StartY + level * (triangleSize.y - 1.35f) + orientation switch
        {
            Orientation.Up => -0.03f,
            Orientation.Down => 1.25f
        };

        return new Vector2(x, y);
    }
}