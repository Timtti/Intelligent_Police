using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using MLAgents;
using MLAgents.Sensor;

public class CarAgent : Agent
{
    Collider2D car_collider;
    bool crush;
    Rigidbody2D rbody;
    Vector2 initPos;
    Quaternion initRota;
    VectorSensor sensor = new VectorSensor(4);
    float power = 50f;
    [SerializeField]
    Transform[] waypoints;
    
    GameObject targetObject;
    Transform target;
    bool seen;

    int nextIndex = 6;
    int preIndex = 10;

    public override void InitializeAgent()
    {
        this.rbody = GetComponent<Rigidbody2D>();
        this.initPos = this.transform.position;
        this.initRota = this.transform.rotation;
        this.car_collider = GetComponent<Collider2D>();
        this.targetObject = GameObject.FindGameObjectWithTag("Target");
        this.target = this.targetObject.transform;
    }
    public override void AgentReset()
    {
        this.transform.position = this.initPos;
        this.transform.rotation = this.initRota;
        this.nextIndex = 6;
        this.preIndex = 10;
        this.targetObject = GameObject.FindGameObjectWithTag("Target");
        this.target = targetObject.transform;
        this.seen = targetObject.GetComponent<isSeen>().Rendered;
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
        Done();
    }
    public override void AgentAction(float[] vectorAction)
    {
        Move();
        if (Mathf.Approximately(transform.position.x, waypoints[nextIndex].transform.position.x)
                && Mathf.Approximately(transform.position.y, waypoints[nextIndex].transform.position.y))
        {
            preIndex = nextIndex;
            //find next waypoint
            nextIndex = randomDirection();
            //set previous as last waypoint
        }
    }
    
    public void Move()
    {

        this.seen = targetObject.GetComponent<isSeen>().Rendered;
        if (seen)
        {
            Debug.Log("SEEN");
        }
        //move
        transform.right = transform.position - waypoints[nextIndex].position;
        transform.position = Vector2.MoveTowards(transform.position,
                                        waypoints[nextIndex].transform.position,
                                        power * Time.deltaTime);
    }
    public void Handling()
    {
        if(preIndex - nextIndex == 4)
        {
            //left
            transform.Rotate(0, 0, 0);
        }else if(nextIndex - preIndex == 4)
        {
            //right
            transform.Rotate(180, 0, 0);
        }
        else if(preIndex - nextIndex == 1)
        {
            //up
            transform.Rotate(0, 0, 0);
        }
        else if(preIndex - nextIndex == -1)
        {
            //down
        }
    }
    
    int randomDirection()
    {
        //if no waypoints set as no waypoint to follow
        if (waypoints.Length == 0) return -1;
        //current waypoint
        int wpI = nextIndex;
        //list for choosing direction
        List<int> list = new List<int>();
        //count for number of directions that car cannot go (walls)
        int count = 0;

        //Initialise directions, the waypoint placements are order-specific
        int left = nextIndex - 4;
        int right = nextIndex + 4;
        int up = nextIndex - 1;
        int down = nextIndex + 1;
        //only way to check for modulos in c#
        int remainUp = up % 4;
        int remainDown = down % 4;
        //Check if walls in certain direction
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -Vector2.right, 48.5f);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, 48.5f);
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, 48.5f);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -Vector2.up, 48.5f);
        //add to count for directions   (walls)
        if (hitleft)
        {
            count++;
        }
        if (hitright)
        {
            count++;
        }
        if (hitup)
        {
            count++;
        }
        if (hitdown)
        {
            count++;
        }
        // check if the directions the car cannot go (walls) is greater or equal to 3
        if (count >= 3)
        {
            //check which direction the car can go
            if (!hitleft)
            {
                wpI = left;
            }
            else if (!hitright)
            {
                wpI = right;
            }
            else if (!hitup)
            {
                wpI = up;
            }
            else if (!hitdown)
            {
                wpI = down;
            }
        }
        else
        {
            //check if car can go in that direction
            if (left >= 0 && !hitleft)
            {
                list.Add(left);
            }
            if (right <= 15 && !hitright)
            {
                list.Add(right);
            }
            if (remainUp != 3 && !hitup)
            {
                list.Add(up);
            }
            if (remainDown != 0 && !hitdown)
            {
                list.Add(down);
            }
            //randomly choose direction from list
            System.Random random = new System.Random();
            wpI = list[random.Next(list.Count)];
        }
        return wpI;
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
