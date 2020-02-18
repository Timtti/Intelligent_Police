using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class randomMove : MonoBehaviour
{
    //initialise variables 
    [SerializeField]
    Transform[] waypoints;

    [SerializeField]
    float moveSpeed = 10f;

    private int[] previous = new int[3];
    private int current;

    //first waypoint index is five
    private int waypointIndex = 5;
    private int i = 0;

    public bool trapped = false;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        previous[0] = waypointIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (previous.Length == 3) i=0;
        //rotate and move towards waypoint
        if (!trapped)
        {
            Move();

            //check if car is approximately at target
            if (Mathf.Approximately(transform.position.x, waypoints[waypointIndex].transform.position.x)
                && Mathf.Approximately(transform.position.y, waypoints[waypointIndex].transform.position.y))
            {
                //find next waypoint
                waypointIndex = randomDirection();
                //set previous as last waypoint
                previous[i] = waypointIndex;
                i++;
            }
        }
    }

    //Move to waypoint
    void Move()
    {
        //rotate
        transform.up = waypoints[waypointIndex].position - transform.position;
        //move
        transform.position = Vector2.MoveTowards(transform.position,
                                        waypoints[waypointIndex].transform.position,
                                        moveSpeed * Time.deltaTime);
    }

    /*Finds random direction to go in
    Output: waypointIndex to go to*/
    int randomDirection()
    {
        //if no waypoints set as no waypoint to follow
        if (waypoints.Length == 0) return -1;
        //current waypoint
        int wpI = waypointIndex;
        //list for choosing direction
        List<int> list = new List<int>();
        //count for number of directions that car cannot go (walls)
        int count = 0;

        //Initialise directions, the waypoint placements are order-specific
        int left = waypointIndex - 4;
        int right = waypointIndex + 4;
        int up = waypointIndex - 1;
        int down = waypointIndex + 1;
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
            previous = new int[3]; //set previous to new list
            //check which direction the car can go
            if (!hitleft)
            {
                wpI = left;
                previous[0] = left;
            } else if (!hitright)
            {
                wpI = right;
                previous[0] = right;
            } else if (!hitup)
            {
                wpI = up;
                previous[0] = up;
            } else if (!hitdown)
            {
                wpI = down;
                previous[0] = down;
            } else
            {
                trapped = true; //no direction car can go
            }
        } else {
            //check if car can go in that direction
            if (left >= 0 && !previous.Contains(left) && !hitleft)
            {
                list.Add(left);
            }
            if (right <= 15 && !previous.Contains(left) && !hitright)
            {
                list.Add(right);
            }
            if (remainUp != 3 && !previous.Contains(left) && !hitup)
            {
                list.Add(up);
            }
            if (remainDown != 0 && !previous.Contains(left) && !hitdown)
            {
                list.Add(down);
            }
            //randomly choose direction from list
            System.Random random = new System.Random();
            wpI = list[random.Next(list.Count)];
        }
        return wpI;
    }

    /* Find Closest waypoint to transform*/
    /*int FindClosestWaypoint()
      {
          if (waypoints.Length == 0) return -1;
          //closest is what is to be returned
          int closest = 0;
          //list for random choosing if multiple are closest
          List<int> list = new List<int>();
          //calculate distance between transform and first element in waypoints
          float lastDist = ManhattanDistance(transform.position, waypoints[0].transform.position);
          //check all distances
          for(int i = 1; i < waypoints.Length; i++)
          {
              //calculate distance between tranform and next element in waypoints
              float thisDist = ManhattanDistance(transform.position, waypoints[i].transform.position);
              //if (((lastDist > thisDist) || (Mathf.Approximately(lastDist,thisDist))) && i != waypointIndex)
              if (lastDist > thisDist && i != waypointIndex)
              {
                  //this is to check if there are multiple closest points
                  if (Mathf.Approximately(lastDist, thisDist))
                  {
                      //if there are then add them to the list
                      list.Add(i);
                  } else {
                      //otherwise choose distance as closest
                      closest = i;
                  }

              }
          }
         bool isNotEmpty = list.Any();
          if(isNotEmpty)
          {
              //to make sure that all elements are not bigger than closest
              //before choosing from the list
              if (list.All(element => element < closest))
              {
                  //choose random element from list and set as closest
                  System.Random random = new System.Random();
                  closest = list[random.Next(list.Count)];
              }
          }
          //closest is returned and is to be next target
          return closest;
      }

      public static float ManhattanDistance(Vector2 a, Vector2 b)
      {
          checked
          {
              return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
          }
      }*/
}