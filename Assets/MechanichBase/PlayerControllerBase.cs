using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBase : MonoBehaviour
{
    [SerializeField] private DistanceJoint2D distanceJoin;
    [SerializeField] private float maxDistance;
    [SerializeField] private Player player;
    private ChainOrigen origin;
    private bool hasCreate;
    private bool hasEvaluate;
    private Rigidbody2D _distanceJoinConnectedBody;
    public ButtonCustomActivation.OnActivation OnActivate;
    public ButtonCustomActivation.OnActivation OnInactivate;

    public void Configure(ChainOrigen origen, float velocityOfLoad)
    {
        origin = origen;
        player.SetVelocity(velocityOfLoad);
    }

    // Update is called once per frame
    void Update()
    {
        if (DistanceInTwoObject() && hasEvaluate)
        {
            hasEvaluate = false;
            if (origin != null)
            {
                if (origin.CanCreateChain())
                {
                    origin.CreatedChain(this);
                }
                else
                {
                    distanceJoin.enabled = false;
                    origin.CutChain();
                    origin = null;
                    OnInactivate?.Invoke();
                }
            }
        }
    }

    private bool DistanceInTwoObject()
    {
        var distanceJoinConnectedBody = distanceJoin.connectedBody;
        if (distanceJoinConnectedBody == null) return false;
        var sqrMagnitude = (distanceJoinConnectedBody.transform.position - transform.position).sqrMagnitude;
        Debug.Log($"Distance is {sqrMagnitude}");
        return sqrMagnitude > maxDistance;
    }

    public void SetLastChain(Rigidbody2D lastChain)
    {
        distanceJoin.connectedBody = lastChain;
        
        distanceJoin.enabled = true;
        hasEvaluate = true;
        _distanceJoinConnectedBody = lastChain;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn"))
        {
            if (other.TryGetComponent(out origin))
            {
                if (origin.CanCreateChain())
                {
                    origin.CreatedChain(this);
                    OnActivate?.Invoke();
                }
            }
        }
    }

    public void Evaluate()
    {
        hasEvaluate = true;
    }

    public ChainOrigen GetOrigin()
    {
        return origin;
    }
}
