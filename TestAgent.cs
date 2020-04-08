using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class TestAgent : Agent
{
    Rigidbody2D rbody;
    Vector2 initPos;
    Quaternion initRota;
    float power = 50f;
    Collider2D col;
    VectorSensor sensor = new VectorSensor(4);

    GameObject[] policeteam;
    GameObject target;

    [SerializeField]
    Transform[] waypoints;
    [SerializeField]
    private int[] initInd;
    public int[] InitInd => initInd;
    public int[] directions = new int[] { 1, -4, -1, 4 };//up right down left.

    int nextIndex;
    int preIndex;

    bool seen;
    RayPerceptionSensor ray;


    public override void Initialize()
    {
        this.rbody = GetComponent<Rigidbody2D>();
        this.initPos = this.transform.position;
        this.initRota = this.transform.rotation;
        this.preIndex = InitInd[0];
        this.nextIndex = InitInd[1];
        this.col = GetComponent<Collider2D>();
        this.target = GameObject.FindGameObjectWithTag("Target");
        this.policeteam = GameObject.FindGameObjectsWithTag("Police");
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(rbody.velocity);
    }
    public override void OnEpisodeBegin()
    {
        this.transform.position = this.initPos;
        this.transform.rotation = this.initRota;
        this.preIndex = InitInd[0];
        this.nextIndex = InitInd[1];
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        Move();
        if (Mathf.Approximately(transform.position.x, waypoints[nextIndex].transform.position.x)){
            preIndex = nextIndex;
            nextIndex = preIndex + 1;
        }
    }
    public int Handling()
    {
        if (seen)
        {
            return ChaseTurn();
        }
        else
        {
            return randomTurn();
        }
    }
    public int randomTurn()
    {
        int ans;
        ans = Random.Range(0, 3);
        ans = directions[ans];
        if(preIndex + ans < 0 || preIndex + ans > 15)
        {
            ans = randomTurn();
        }
        return ans;
    }
    public int ChaseTurn()
    {
        int ans=0;
        int targetNext = target.GetComponent<randomMove>().waypointIndex;
        if (targetNext < nextIndex) {
            if(targetNext >= nextIndex - 4)
            {
                //direction up
                ans = 1;
            }
            else if((targetNext - nextIndex) % 4 == 0)
            {
                //direction left
                ans = 4;
            }
        }else if(targetNext <= nextIndex + 4)
        {
            //down
            ans = -1;
        }else if((targetNext - nextIndex) % 4 == 0)
        {
            //right
            ans = -4;
        }
        else
        {
            ans = randomTurn();
        }
        return ans;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            AddReward(100f);
        }
        else
        {
            AddReward(-10f);
        }
        EndEpisode();
    }

    public void Move(int action=0)
    {
        if(action != 0) {
            nextIndex = preIndex + action;
        }
        transform.right = transform.position - waypoints[nextIndex].position;
        transform.position = Vector2.MoveTowards(transform.position,
                                            waypoints[nextIndex].transform.position,
                                            power * Time.deltaTime);

        this.seen = target.GetComponent<isSeen>().Rendered;
    }
    public override float[] Heuristic()
    {
        var Action = new float[2];
        Action[0] = Input.GetAxis("Horizontal");
        Action[1] = Input.GetAxis("Vertical");
        return Action;
    }

}
