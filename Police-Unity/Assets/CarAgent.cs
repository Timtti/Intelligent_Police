using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CarAgent : Agent
{
    // Start is called before the first frame update
    Rigidbody2D rbody;
    Collider2D col;
    private Vector2 initPosition; 
    private Quaternion initRotation;
    bool collided;

    public void Start()
    {
        col = GetComponent<Collider2D>();
        rbody = GetComponent<Rigidbody2D>();
    }
    public override void InitializeAgent()
    {
        this.initPosition = this.transform.position;
        this.initRotation = this.transform.rotation;
    }

    public override void AgentReset()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;
        collided = false;
    }

    public override void CollectObservations()
    {
        AddVectorObs(rbody.velocity.x);
        AddVectorObs(rbody.velocity.y);
        AddVectorObs(transform.position);
    }

    public void OnCollisionEnter(Collision collision)
    {
        collided = true;
    }

    public override void AgentAction(float[] vectorAction)
    {
        base.AgentAction(vectorAction);
        if (collided)
            Done();
    }
    public override float[] Heuristic()
    {
        var action = new float[1];
        return action;
    }
}

