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

    bool trapped = false;
    GameObject targetObject;
    Transform target;
    
    float searchAngle = 0.785398163f;
    SphereCollider searchArea;
    bool chasing = false;
    float angleFound;

    int nextIndex = 6;
    int preIndex = 10;
    bool handled = false;

    public void Sensor()
    {
        Vector3 toVector = target.transform.position - transform.position;
        Vector2 toVector2 = target.transform.position - transform.position;
        float angleAbs = Mathf.Atan2(toVector2.y, toVector2.x);
        Debug.Log(angleAbs);
        //angleFound = Vector3.Angle(transform.forward, toVector)-90f;
        //if (this.angleFound <= searchAngle && this.angleFound >= (0-searchAngle))
        if(angleAbs <= searchAngle && angleAbs >= (0-searchAngle))
        {
            Debug.Log("YES");
            chasing = true;
        }
        else
        {
            chasing = false;
            this.angleFound = new float();
        }
    }
        
    //public void OnTriggerStay2D(Collider2D collision)
    //{
    //    if(collision.tag == "Target")
    //    {
    //        var targetDirection = collision.transform.position - transform.position;
    //        angleFound = Vector3.Angle(transform.forward, targetDirection);
    //        if(angleFound <= searchAngle)
    //        {
    //            Debug.Log("Found:" + angleFound);
    //            chasing = true;
    //        }
    //        else
    //        {
    //            chasing = false;
    //            angleFound = new float();
    //        }
    //    }
    //    else
    //    {
    //        chasing = false;
    //        angleFound = new float();
    //    }
    //}
    
    public void ChasingMove()
    {
        if (angleFound < 90)
        {
            RelaHandling("Left");
        }
        else if (angleFound > 90)
        {
            RelaHandling("Right");
        }
    }

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
        this.crush = false;
        this.handled = false;
        this.nextIndex = 6;
        this.preIndex = 10;
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
        Sensor();
        if (chasing)
        {
            ChasingMove();
        }
        else
        {
            Handling(waypoints[nextIndex].position);
        }
        
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
        //move
        transform.position = Vector2.MoveTowards(transform.position,
                                        waypoints[nextIndex].transform.position,
                                        power * Time.deltaTime);
    }

    public void Handling(Vector3 dest)
    {
        if (!handled)
        {
            handled = true;
            if(dest.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if(dest.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            if(dest.y < transform.position.y)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            if(dest.y > transform.position.y)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }
        
    }
    public void Handling(string turn)
    {
        if (turn == "Left"){
            transform.Rotate(Vector3.left);
        }
        else if(turn == "Right")
        {
            transform.Rotate(Vector3.right);
        }
    }
    public string Heading()
    {
        string ans;
        if (preIndex - nextIndex == 4)
        {
            ans = "Left";
        }
        else if(preIndex - nextIndex == -4)
        {
            ans = "Right";
        }
        else if(preIndex - nextIndex == 1)
        {
            ans = "Up";
        }
        else if(preIndex - nextIndex == -1)
        {
            ans = "Down";
        }
        else
        {
            ans = (preIndex - nextIndex).ToString();
        }
        return ans;
    }
    public void RelaHandling(string direction)
    {
        string heading = Heading();
        if ((direction == "Left" && heading == "Up")||(direction == "Right" && heading == "Down")||(direction == "Forward" && heading == "Left")||(direction == "Backward" && heading == "Right"))
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if ((direction == "Right" && heading == "Up") || (direction == "Left" && heading == "Down") || (direction == "Forward" && heading == "Right") || (direction == "Backward" && heading == "Left"))
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if ((direction == "Forward" && heading == "Up") || (direction == "Backward" && heading == "Down") || (direction == "Left" && heading == "Right") || (direction == "Right" && heading == "Left"))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if ((direction == "Backward" && heading == "Up") || (direction == "Forward" && heading == "Down") || (direction == "Right" && heading == "Right") || (direction == "Left" && heading == "Left"))
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
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
            else
            {
                trapped = true; //no direction car can go
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
        handled = false;
        return wpI;
    }

    //public Vector3 Handling(int choice = 0)
    //{
    //    if (choice == 1)
    //    {
    //        return Vector3.forward;
    //    }
    //    if (choice == 2)
    //    {
    //        return Vector3.back;
    //    }
    //    else
    //    {
    //        return new Vector3(0, 0, 0);
    //    }
    //}

    //public Vector3 Pedalwork(int min = 0, int max = 0)
    //{
    //    if (min == 0 & max == 0)
    //    {
    //        return new Vector3(power, 0, 0);
    //    }
    //    else
    //    {
    //        System.Random randf = new System.Random();
    //        float f = (float)randf.Next(min, max);
    //        return new Vector3(f, 0, 0);
    //    }
    //}

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
