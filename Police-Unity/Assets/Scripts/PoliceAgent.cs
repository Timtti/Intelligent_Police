using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using MLAgents;
using MLAgents.Sensor;

public class PoliceAgent : Agent
{
    Collider2D car_collider;
    bool crush;
    Rigidbody2D rbody;
    Vector2 initPos;
    Quaternion initRota;
    VectorSensor sensor = new VectorSensor(4);
    float power = 5f;

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
        if (collision.gameObject.tag == "Target")
        {
            AddReward(100f);
        }
        else
        {
            AddReward(-10f);
        }
    }
    public override void AgentAction(float[] vectorAction)
    {
        System.Random rnd = new System.Random();
        int hand = rnd.Next(0,3);
        transform.Rotate(Handling(hand));
        
        Vector3 velocity = transform.rotation * Pedalwork();
        transform.position -= velocity * Time.deltaTime;

        AddReward(0.1f);
        if (this.crush)
        {
            Done();
        }
    }
    public Vector3 Handling(int choice=0)
    {
        if (choice == 1)
        {
            return Vector3.forward;
        }
        if (choice == 2)
        {
            return Vector3.back;
        }
        else
        {
            return new Vector3(0,0,0);
        }
    }
    
    public Vector3 Pedalwork(int min = 0, int max = 0)
    {
        if(min == 0 & max == 0){
            return new Vector3(power, 0, 0);
        }
        else
        {
            System.Random randf = new System.Random();
            float f = (float)randf.Next(min,max);
            return new Vector3(f,0,0);
        }
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        sensor.AddObservation(this.transform.position);

        // Agent velocity
        sensor.AddObservation(rbody.velocity.x);
        sensor.AddObservation(rbody.velocity.y);
    }
    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
