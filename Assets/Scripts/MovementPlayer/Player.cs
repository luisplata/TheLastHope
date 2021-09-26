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
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject spriteRobot;
    public delegate void OnActionPlayer(float speed);

    public OnActionPlayer OnActionPlayerEvent;
    private Rigidbody2D rb2d;
    private Vector2 inputValue;
    private bool isJump;
    private bool IsPullPanel;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        isJump = false;
        _isOn = true;
        chainSystemPlayer.OnActivate += OnActivate;
    }

    private void OnActivate()
    {
        animator.SetTrigger("cable");
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
        animator.SetTrigger("accion");
    }
    public void OnEnvironment(InputValue value){
        Debug.Log("Press");
        OnActionPlayerEvent?.Invoke(5);
        animator.SetTrigger("boton");
    }

    private void IsOffPress()
    {
        CreateChainToOrigin();
        IsPullPanel = false;
        _isOn = true;
    }

    private void CreateChainToOrigin()
    {
        chainSystemPlayer.GetOrigin()?.GetComponent<SolarPanel>().BlockAll();
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
        if (inputValue.x < 0 && spriteRobot.transform.localScale.x < 0)
        {
            var localScaleRobot =spriteRobot.transform.localScale; 
            localScaleRobot.x = spriteRobot.transform.localScale.x * -1;
            spriteRobot.transform.localScale = localScaleRobot;
        }
        if (inputValue.x > 0 && spriteRobot.transform.localScale.x > 0)
        {
            var localScaleRobot =spriteRobot.transform.localScale; 
            localScaleRobot.x = spriteRobot.transform.localScale.x * -1;
            spriteRobot.transform.localScale = localScaleRobot;
        }
    }

    private void Jumping()
    {
        if (isJump) return;
        isJump = true;
        rb2d.AddForce(Vector2.up * forceJump,ForceMode2D.Impulse);
        chainSystemPlayer.GetOrigin()?.GetComponent<SolarPanel>().Jump();
        animator.SetBool("jump", true);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Respawn"))
        {
            isJump = false;
            animator.SetBool("jump", false);
        }
    }

    private void FixedUpdate()
    {
        speedTotal = inputValue * Time.deltaTime * speed;
        
        animator.SetBool("walk", speedTotal.sqrMagnitude > 1);
        
        var beforeVelocity = speedTotal;
        beforeVelocity.y = rb2d.velocity.y;
        rb2d.velocity = beforeVelocity;
    }

    private float detaTimeLocal = 0;
    [SerializeField] private float pullHoldTime;
    private Vector2 speedTotal;
    private CheckPoint _checkPoint;

    private void Update()
    {
        if (IsPullPanel)
        {
            detaTimeLocal += Time.deltaTime;
            if (detaTimeLocal > pullHoldTime)
            {
                detaTimeLocal = 0;
                //RemoveOnceChain();
            }
        }
        else
        {
            detaTimeLocal = 0;
        }
    }

    private void RemoveOnceChain()
    {
        chainSystemPlayer.GetOrigin()?.RemoveChain();
    }

    private void PullPanel()
    {
        chainSystemPlayer.GetOrigin()?.GetComponent<SolarPanel>().EnableAll(speedTotal);
    }

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        _checkPoint = checkPoint;
    }

    public CheckPoint GetLastCheckPoint()
    {
        return _checkPoint;
    }
}
