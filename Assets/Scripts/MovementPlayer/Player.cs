using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float forceJump;
    [SerializeField] private PlayerControllerBase chainSystemPlayer;
    private Rigidbody2D rb2d;
    private Vector2 inputValue;
    private bool isJump;
    private bool IsPullPanel;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        isJump = false;
        _isOn = true;
    }

    private bool _isOn;
    public void OnAction(InputValue value)
    {
        if (_isOn)
        {
            IsOnPress();
        }
        else
        {
            IsOffPress();
        }
    }

    private void IsOffPress()
    {
        CreateChainToOrigin();
        IsPullPanel = false;
        _isOn = true;
    }

    private void CreateChainToOrigin()
    {
        var rigi = chainSystemPlayer.GetOrigin().GetComponent<Rigidbody2D>();
        rigi.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void IsOnPress()
    {
        PullPanel();
        IsPullPanel = true;
        _isOn = false;
    }

    public void OnMove(InputValue value)
    {
        var readValue = value.Get<Vector2>();
        if (readValue.y > 0.2f)
        {
            Jumping();
        }
        readValue.y = 0;
        inputValue = readValue;
    }

    private void Jumping()
    {
        if (isJump) return;
        isJump = true;
        rb2d.AddForce(Vector2.up * forceJump,ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            isJump = false;
        }
    }

    private void FixedUpdate()
    {
        var beforeVelocity = inputValue * Time.deltaTime * speed;
        beforeVelocity.y = rb2d.velocity.y;
        rb2d.velocity = beforeVelocity;
    }

    private float detaTimeLocal = 0;
    [SerializeField] private float pullHoldTime;
    private void Update()
    {
        if (IsPullPanel)
        {
            detaTimeLocal += Time.deltaTime;
            if (detaTimeLocal > pullHoldTime)
            {
                detaTimeLocal = 0;
                RemoveOnceChain();
            }
        }
        else
        {
            detaTimeLocal = 0;
        }
    }

    private void RemoveOnceChain()
    {
        chainSystemPlayer.GetOrigin().RemoveChain();
    }

    private void PullPanel()
    {
        var rigi = chainSystemPlayer.GetOrigin().GetComponent<Rigidbody2D>();
        rigi.constraints = RigidbodyConstraints2D.None;
        rigi.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
