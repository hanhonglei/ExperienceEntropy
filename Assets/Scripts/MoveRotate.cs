using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public enum MovePath { Tri, Rec, Hor, Ver, Cir };   // several different move path types

public class MoveRotate : MonoBehaviour
{
    public float _moveSpeed = 5.0f;
    public float _rotateSpeed = 90.0f;
    public bool _isMove = false;
    public bool _isRotate = false;
    public MovePath _movePath = MovePath.Tri;   // Move type of this object
    public float _moveRadius = 5.0f;
    public Transform _mainCam;                  // use player camera as the target camera

    private Vector3 _originalPos;
    private List<Vector3> _dests;
    private int _currentDest = 0;

    private float patrolTimer;                  // A timer for the patrolWaitTime.
    [SerializeField]
    private float patrolWaitTime = 0.08f;       // wait time when close enough to the destination   
    [SerializeField]
    private float stopDist = 0.1f;              // the threshold distance to current destination


    // Use this for initialization
    void Start()
    {
        _originalPos = transform.position;      // original position as the start point of moving
        _dests = new List<Vector3>();           // all destinations list
        if (!_mainCam)                          // find player object as 
        {
            _mainCam = Camera.main.transform;
        }
        if (_isMove)
        {
            transform.LookAt(_mainCam);
            SetDests();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    // calculate all destinations based on the moving path type of it
    void SetDests()
    {
        Vector3 v;
        switch (_movePath)
        {
            case MovePath.Tri:
                v = transform.TransformPoint(new Vector3(_moveRadius, 0, 0));
                _dests.Add(v);
                v = _originalPos;
                v.y = _originalPos.y + 2 * _moveRadius;
                _dests.Add(v);
                v = transform.TransformPoint(new Vector3(-_moveRadius, 0, 0));
                _dests.Add(v);
                break;
            case MovePath.Rec:
                v = transform.TransformPoint(new Vector3(_moveRadius, 0, 0));
                _dests.Add(v);
                v.y = _originalPos.y + 2 * _moveRadius;
                _dests.Add(v);
                v = transform.TransformPoint(new Vector3(-_moveRadius, 0, 0));
                v.y = _originalPos.y + 2 * _moveRadius;
                _dests.Add(v);
                v.y = _originalPos.y;
                _dests.Add(v);
                break;
            case MovePath.Cir:
                break;
            case MovePath.Hor:
                v = transform.TransformPoint(new Vector3(_moveRadius, 0, 0));
                _dests.Add(v);
                v = transform.TransformPoint(new Vector3(-_moveRadius, 0, 0));
                _dests.Add(v);
                break;
            case MovePath.Ver:
                v = _originalPos;
                v.y = _originalPos.y + 2 * _moveRadius;
                _dests.Add(v);
                v = _originalPos;
                _dests.Add(v);
                break;
        }
    }

    void FixedUpdate()
    {
        if (_isRotate)
        {
            transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.LookAt(_mainCam);
        }
        if (_isMove)
        {
            if (Vector3.Distance(transform.position, _dests[_currentDest]) <= stopDist)
            {
                // ... increment the timer.
                patrolTimer += Time.deltaTime;

                // If the timer exceeds the wait time...
                if (patrolTimer >= patrolWaitTime)
                {
                    // set the proper position [11/3/2016 Han]
                    transform.position = _dests[_currentDest];
                    _currentDest = (_currentDest + 1) % _dests.Count;

                    // Reset the timer.
                    patrolTimer = 0;
                }
            }
            else
            {
                // If not near a destination, reset the timer.
                patrolTimer = 0;

                // Keep moving [11/3/2016 Han]
                int pre = (_currentDest + _dests.Count - 1) % _dests.Count;
                Vector3 m = _dests[_currentDest] - _dests[pre];
                transform.Translate(_moveSpeed * m.normalized * Time.deltaTime, Space.World);
            }
        }
    }
}