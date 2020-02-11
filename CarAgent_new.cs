using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Sprites;
using MLAgents;

public class CarAgent : Agent
{
    Collider2D car_collider;
    bool crush;
    Rigidbody2D rbody;
    Vector2 initPos;
    Quaternion initRota;
    private RayPerception2D rayper;

    public override void InitializeAgent()
    {
        this.rbody = GetComponent<Rigidbody2D>();
        this.initPos = this.transform.position;
        this.initRota = this.transform.rotation;
    }
    public override void AgentReset()
    {
        this.transform.position = this.initPos;
        this.transform.rotation = this.initRota;
        this.crush = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.crush = true;
    }
    public override void AgentAction(float[] vectorAction)
    {
        Vector2 handling = new Vector2(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
        //rbody.AddForce(handling);
        float handle_x = Random.Range(-1f, 1f);
        float handle_y = Random.Range(-1f, 1f);
        this.gameObject.transform.Rotate(new Vector3(handle_x,handle_y));

        this.rbody.velocity = this.gameObject.transform.rotation * new Vector2(0, 20);
        
        AddReward(1.0f);
        if (this.crush)
        {
            Done();
        }
    }
    public override void CollectObservations()
    {
        AddVectorObs(this.transform.position);
    }
    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
