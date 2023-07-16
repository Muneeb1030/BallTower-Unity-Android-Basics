using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Rigidbody2D pivot;

    [SerializeField] private float _respawnDelay;
    [SerializeField] private float _invokeDelay;
    
    private int _ballCount = 0;
    private bool _isDragging;

    private Camera camera;
    private Rigidbody2D ballrigidbody;
    private SpringJoint2D ballSpringJoint;

    
    void Start()
    {
        camera = Camera.main;
        Respawn();
    }

    void Update()
    {
        if(ballrigidbody == null) { return; }
        //For Single Touch Support
        //if(!Touchscreen.current.primaryTouch.press.isPressed)

        //For MultiTouch Support
        if(Touch.activeTouches.Count == 0)
        {
            if(_isDragging)
            {
                LaunchBall();
            }
            _isDragging = false;
            return;
        }
        _isDragging=true;
        ballrigidbody.isKinematic = true;
        //For Single Touch Support
        //Vector2 _touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        //For MultiTouch Support
        Vector2 _touchPosition = new Vector2();
        foreach (Touch _touch in Touch.activeTouches)
        {
            _touchPosition += _touch.screenPosition;
        }
        _touchPosition /= Touch.activeTouches.Count;
        Vector3 _worldPosition = camera.ScreenToWorldPoint(_touchPosition);
        ballrigidbody.position = _worldPosition;

    }

    private void Respawn()
    {
        _ballCount++;
        if(_ballCount <= 5)
        {
            GameObject gameObject = Instantiate(ball, pivot.position, Quaternion.identity);

            ballrigidbody = gameObject.GetComponent<Rigidbody2D>();
            ballSpringJoint = gameObject.GetComponent<SpringJoint2D>();

            ballSpringJoint.connectedBody = pivot;
        }
       
    }

    private void LaunchBall()
    {
        ballrigidbody.isKinematic = false;
        ballrigidbody = null;

        Invoke(nameof(DetchBall), _invokeDelay);
    }

    private void DetchBall()
    {
        ballSpringJoint.enabled = false;
        ballSpringJoint = null;

        Invoke(nameof(Respawn), _respawnDelay);
    }

    //For MultiTouch Funcationality
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();   
    }

}
