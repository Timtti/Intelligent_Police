using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class TestAgent : Agent
{
    int direct;
    Rigidbody2D rbody;
    Vector2 initPos;
    Quaternion initRota;
    float power = 50f;
    Collider2D col;

    GameObject[] policeteam;
    GameObject target;

    [SerializeField]
    Transform[] waypoints;
    [SerializeField]
    private int[] initInd;
    public int[] InitInd => initInd;
    public int[] directions = new int[] { -1, -4, 1, 4 };//up left down right.

    int nextIndex;
    int preIndex;

    public bool seen;
    RayPerceptionOutput rayper;

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
        sensor.AddObservation(transform.position);
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
        if (!target.GetComponent<randomMove>().trapped)
        {
            Move();
            if (Mathf.Approximately(transform.position.x, waypoints[nextIndex].transform.position.x) && Mathf.Approximately(transform.position.y, waypoints[nextIndex].transform.position.y))
            {
                preIndex = nextIndex;
                if (seen)
                {
                    Debug.Log("SEEN");
                    nextIndex = this.target.GetComponent<randomMove>().waypointIndex;
                }
                else
                {
                    nextIndex = preIndex + direct;
                }
            }
        }
        else
        {
            AddReward(200f);
            EndEpisode();
        }
    }

    private void Update()
    {
        
        if (!seen)
        {
            direct = randomTurn();
        }
    }

    /*public int Handling()
    {
        if (seen)
        {

            Debug.Log("SEEN");
            direct = ChaseTurn();
            return direct;
        }
        else
        {
            direct = randomTurn();
            return direct;
        }
    }*/

    public int randomTurn()
    {
        int ans;
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -Vector2.right, 48.5f);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, 48.5f);
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, 48.5f);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -Vector2.up, 48.5f);
        List<int> dir = new List<int>(directions);
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
        ans = Random.Range(0, dir.Count);
        ans = dir[ans];
        if (preIndex + ans < 0 || preIndex + ans > 15)
        {
            ans = randomTurn();
        }
        return ans;
    }
    public int ChaseTurn()
    {
        int ans=0;
        int targetCur = target.GetComponent<randomMove>().current;
        int targetNext = target.GetComponent<randomMove>().waypointIndex;
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -Vector2.right, 48.5f);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, 48.5f);
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, 48.5f);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -Vector2.up, 48.5f);
        if (targetNext < nextIndex) {
            if(targetNext >= nextIndex - 4 && !hitup)
            {
                //direction up
                ans = -1;
            }
            else if((targetNext - nextIndex) % 4 == 0 && !hitleft)
            {
                //direction left
                ans = -4;
            }
        }else if(targetNext <= nextIndex + 4 && !hitdown)
        {
            //down
            ans = 1;
        }else if((targetNext - nextIndex) % 4 == 0 && !hitright)
        {
            //right
            ans = 4;
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
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

}
