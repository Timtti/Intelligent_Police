using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class TestAgent : Agent
{
    //set variables
    int direct;//direction
    Rigidbody2D rbody;
    Vector2 initPos;//initial position
    Quaternion initRota;
    float power = 50f;//speed
    Collider2D col;

    GameObject[] policeteam;//list of all police cars
    GameObject target;//target car

    [SerializeField]
    Transform[] waypoints;//way points, act as map
    [SerializeField]
    private int[] initInd;//initial waypoint
    public int[] InitInd => initInd;
    public int[] directions = new int[] { -1, -4, 1, 4 };//up left down right, posible movements

    int nextIndex;//next index of waypoint
    int preIndex;//previous waypoint

    public bool seen;//is the target seen
    RayPerceptionOutput rayper;

    public override void Initialize()
    {
        //initialise agent
        this.rbody = GetComponent<Rigidbody2D>();
        this.initPos = this.transform.position;
        this.initRota = this.transform.rotation;
        this.preIndex = InitInd[0];
        this.nextIndex = InitInd[1];
        this.col = GetComponent<Collider2D>();
        this.target = GameObject.FindGameObjectWithTag("Target");//one gameobject with tag target
        this.policeteam = GameObject.FindGameObjectsWithTag("Police");//all gameobjects with tag police
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rbody.velocity);
        if (seen)
        {
            sensor.AddObservation(this.target.GetComponent<randomMove>().transform.position);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
    }
    public override void OnEpisodeBegin()
    {
        //called at the beginning of every episode
        this.transform.position = this.initPos;
        this.transform.rotation = this.initRota;
        this.preIndex = InitInd[0];
        this.nextIndex = InitInd[1];
        Move();//make sure the police car is moving at the beginning
    }
    public override void OnActionReceived(float[] vectorAction)
    {
           Move();
           AddReward(-0.01f);//the sooner the police capture the target the less penalty it gets
           if (Mathf.Approximately(transform.position.x, waypoints[nextIndex].transform.position.x) && Mathf.Approximately(transform.position.y, waypoints[nextIndex].transform.position.y))
           {
            //if it is at the waypoint
               preIndex = nextIndex;//update preIndex, nextIndex is now reached
               if (seen)
               {
                //if target car is seen
                   Debug.Log("SEEN");//for debug use
                   nextIndex = this.target.GetComponent<randomMove>().waypointIndex;//chase the target car
               }
               else
               {
                   nextIndex = preIndex + direct;//move according to direct
               }
           }
    }

    private void Update()
    {
        //called every frame
        if (!seen)
        {
            direct = randomTurn();//if not seen, randomly decide direction
        }
    }

    public int randomTurn()
    {
        //output random direction
        int ans;
        //check if the way is blocked
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -Vector2.right, 48.5f);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, 48.5f);
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, 48.5f);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -Vector2.up, 48.5f);
        List<int> dir = new List<int>(directions);
        //if blocked, don't go
        if (hitdown)
        {
            dir.Remove(1);
        }
        if (hitup)
        {
            dir.Remove(-1);
        }
        if (hitleft)
        {
            dir.Remove(-4);
        }
        if (hitright)
        {
            dir.Remove(4);
        }
        //randomly choose an answer
        ans = Random.Range(0, dir.Count);
        ans = dir[ans];
        if (preIndex + ans < 0 || preIndex + ans > 15)
        {
            //if answer is not in the map index range, run the function again
            ans = randomTurn();
        }
        return ans;
    }
    //public int ChaseTurn()
    //{
    //    int ans=0;
    //    int targetCur = target.GetComponent<randomMove>().current;
    //    int targetNext = target.GetComponent<randomMove>().waypointIndex;
    //    RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -Vector2.right, 48.5f);
    //    RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, 48.5f);
    //    RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, 48.5f);
    //    RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -Vector2.up, 48.5f);
    //    if (targetNext < nextIndex) {
    //        if(targetNext >= nextIndex - 4 && !hitup)
    //        {
    //            //direction up
    //            ans = -1;
    //        }
    //        else if((targetNext - nextIndex) % 4 == 0 && !hitleft)
    //        {
    //            //direction left
    //            ans = -4;
    //        }
    //    }else if(targetNext <= nextIndex + 4 && !hitdown)
    //    {
    //        //down
    //        ans = 1;
    //    }else if((targetNext - nextIndex) % 4 == 0 && !hitright)
    //    {
    //        //right
    //        ans = 4;
    //    }
    //    else
    //    {
    //        ans = randomTurn();
    //    }
    //    return ans;
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collides
        if (collision.gameObject.tag == "Target")
        {
            //if hits the target, get reward
            AddReward(1f);
        }
        else
        {
            //if hit obstactle, get punishment
            AddReward(-2f);
        }
        EndEpisode();//end episode and restart
    }

    public void Move(int action=0)
    {
        if(action != 0) {
            nextIndex = preIndex + action;//move to the next index
        }

        transform.right = transform.position - waypoints[nextIndex].position;//update position, using transorm.right becuase the car image is horizontal
        transform.position = Vector2.MoveTowards(transform.position,
                                            waypoints[nextIndex].transform.position,
                                            power * Time.deltaTime);//move according to poer

        this.seen = target.GetComponent<isSeen>().Rendered;//isSeen will decide if the target is seen. here get the boolean from isSeen/

    }
    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
