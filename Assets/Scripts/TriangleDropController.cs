using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Orientation
{
    Up = 0,
    Down = 1
}

internal enum TrianglePositionState
{
    Free,
    Occupied
}

internal enum TriangleDropStage
{
    DropFirstUp,
    DropSecondUp,
    DropFirstDown,
    Random
}

public class TriangleDropController : MonoBehaviour
{
    public const float StartX = -13.30237f;
    public const float StartY = -7.821842f;
    public Transform camera;
    public Camera cameraComponent;
    public Transform trianglePrefab;
    private float _lastTriangleDroppedTime;
    private int currentLevel = 0;
    private const int NumberOfUpTriangles = 8;
    private const int NumberOfDownTriangles = 8;
    private float TriangleSpawnRate => Math.Max(1.0f - currentLevel * 0.05f, 0.5f);
    private Dictionary<int, List<GameObject>> trianglesInLevels = new Dictionary<int, List<GameObject>>();

    private TrianglePositionState[,] states = {
        {
            TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
            TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
            TrianglePositionState.Free, TrianglePositionState.Free
        },
        {
            TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
            TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
            TrianglePositionState.Free, TrianglePositionState.Free
        }
    };

    private void Start()
    {
        _lastTriangleDroppedTime = Time.time;

        var groundTriangles = GameObject.FindGameObjectsWithTag("Triangle");
        trianglesInLevels[-1] = groundTriangles.ToList();
        trianglesInLevels[0] = new List<GameObject>();
    }

    private void Update()
    {
        if (Time.time < _lastTriangleDroppedTime + TriangleSpawnRate) return;
        
        Orientation orientation;
        // To prevent the game from being impossible, if all but one red triangle have been
        // but no green has been filled, force a green triangle to be dropped
        if (_numberOfFree(Orientation.Up) == 1 && _numberOfOccupied(Orientation.Down) == 0)
        {
            orientation = Orientation.Down;
        }
        else if (_allFree(Orientation.Up) || !_anyPossiblePositionForOrientation(Orientation.Down))
        {
            orientation = Orientation.Up;
        }
        else if (_allOccupied(Orientation.Up))
        {
            orientation = Orientation.Down;
        }
        else
        {
            orientation = (Orientation)Random.Range(0, 2);
        }

        var position = _randomAllowedPosition(orientation);

        _createTriangle(orientation, position);

        if (_allOccupied(Orientation.Down) && _allOccupied(Orientation.Up))
        {
            currentLevel += 1;
            _resetToInitialStates();

            // Delete lower levels that you will never see again to improve performance
            if (currentLevel > 1)
            {
                var deleteLevel = currentLevel - 3;
                if (trianglesInLevels.ContainsKey(deleteLevel))
                {
                    foreach (var triangle in trianglesInLevels[deleteLevel])
                    {
                        Destroy(triangle);
                    }

                    trianglesInLevels.Remove(deleteLevel);
                }
            }

            trianglesInLevels[currentLevel] = new List<GameObject>();
        }
        
        _lastTriangleDroppedTime = Time.time;
    }


    private void _createTriangle(Orientation orientation, int position)
    {
        var triangle = Instantiate(trianglePrefab);
        var triangleRb = triangle.gameObject.GetComponent<Rigidbody2D>();
        var spriteRenderer = triangle.gameObject.GetComponent<SpriteRenderer>();
        var triangleController = triangle.gameObject.GetComponent<TriangleController>();
        var triangleSize = spriteRenderer.bounds.size;
        triangle.position = orientation switch
        {
            Orientation.Up => new Vector2(StartX + position * (triangleSize.x - 0.6f), _triangleSpawnY()),
            Orientation.Down => new Vector2(StartX + (triangleSize.x - 0.6f) / 2 + position * (triangleSize.x - 0.6f), _triangleSpawnY())
        };
        triangle.eulerAngles = orientation switch
        {
            Orientation.Up => new Vector3(0, 0, 0),
            Orientation.Down => new Vector3(0, 0, 180.0f)
        };
        spriteRenderer.color = orientation switch
        {
            Orientation.Up => Color.red,
            Orientation.Down => new Color(0.411f, 0.69f, 0.365f)
        };
        triangleRb.velocity = new Vector2(0, -20.0f);
        triangleController.position = position;
        triangleController.orientation = orientation;
        triangleController.level = currentLevel;

        states[(int)orientation, position] = TrianglePositionState.Occupied;
        trianglesInLevels[currentLevel].Add(triangle.gameObject);
    }

    private List<int> _possiblePositions(Orientation orientation)
    {
        var allowedPositions = new List<int>();

        if (orientation == Orientation.Up)
        {
            foreach (var i in Enumerable.Range(0, NumberOfUpTriangles))
            {
                var plannedPosition = states[0, i];

                if (plannedPosition != TrianglePositionState.Occupied)
                {
                    allowedPositions.Add(i);
                }
            }
        }
        else
        {
            foreach (var i in Enumerable.Range(2, NumberOfDownTriangles-1))
            {
                var first = states[0, i - 2];
                var second = states[0, i - 1];
                var plannedPosition = states[1, i - 2];

                if (first == TrianglePositionState.Occupied && second == TrianglePositionState.Occupied &&
                    plannedPosition != TrianglePositionState.Occupied)
                {
                    allowedPositions.Add(i - 2);
                }
            }

            if (states[0, NumberOfDownTriangles-1] == TrianglePositionState.Occupied && states[1, NumberOfDownTriangles-1] == TrianglePositionState.Free)
            {
                allowedPositions.Add(NumberOfDownTriangles-1);
            }
        }

        return allowedPositions;
    }

    private int _randomAllowedPosition(Orientation orientation)
    {
        var allowedPositions = _possiblePositions(orientation);

        if (allowedPositions.Count == 0)
        {
            return 0;
        }

        return allowedPositions[Random.Range(0, allowedPositions.Count)];
    }

    private bool _allOccupied(Orientation orientation)
    {
        for (int i = 0; i < _trianglesForOrientation(orientation); i++)
        {
            if (states[(int)orientation, i] == TrianglePositionState.Free)
            {
                return false;
            }
        }

        return true;
    }
    
    private bool _allFree(Orientation orientation)
    {
        return _numberOfFree(orientation) == _trianglesForOrientation(orientation);
    }
    
    private int _numberOfOccupied(Orientation orientation)
    {
        var counter = 0;
        for (var i = 0; i < _trianglesForOrientation(orientation); i++)
        {
            if (states[(int)orientation, i] == TrianglePositionState.Occupied)
            {
                counter += 1;
            }
        }

        return counter;
    }

    private int _numberOfFree(Orientation orientation)
    {
        var counter = 0;
        for (var i = 0; i < _trianglesForOrientation(orientation); i++)
        {
            if (states[(int)orientation, i] == TrianglePositionState.Free)
            {
                counter += 1;
            }
        }

        return counter;
    }

    private int _trianglesForOrientation(Orientation orientation)
    {
        return orientation switch
        {
            Orientation.Up => NumberOfUpTriangles,
            Orientation.Down => NumberOfDownTriangles
        };
    }
    
    private bool _anyPossiblePositionForOrientation(Orientation orientation)
    {
        return _possiblePositions(orientation).Count > 0;
    }

    private int _findFirstOccupied(Orientation orientation)
    {
        for (int i = 0; i < _trianglesForOrientation(orientation); i++)
        {
            if (states[(int)orientation, i] == TrianglePositionState.Occupied)
            {
                return i;
            }
        }

        return -1;
    }

    private void _resetToInitialStates()
    {
        states = new[,]{
            {
                TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
                TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
                TrianglePositionState.Free, TrianglePositionState.Free
            },
            {
                TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
                TrianglePositionState.Free, TrianglePositionState.Free, TrianglePositionState.Free,
                TrianglePositionState.Free, TrianglePositionState.Free
            }
        };
    }

    private float _triangleSpawnY()
    {
        return camera.position.y + cameraComponent.orthographicSize + 15.0f;
    }
}