using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Sprites;
using MLAgents;

public class PoliceAgent : Agent
{
    Collider2D car_collider;
    bool crush;
    Rigidbody2D rbody;
    Vector2 initPos;
    Quaternion initRota;
    RayPerception2D rayper;

    public new Camera camera;
    private Texture2D targetTexture;


    public override void InitializeAgent()
    {
        this.rbody = GetComponent<Rigidbody2D>();
        this.camera = GetComponent<Camera>();

        this.initPos = this.transform.position;
        this.initRota = this.transform.rotation;
        this.target = GameObject.Find("Target");
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
            AddReward(10.0f);
        }
    }
    public override void AgentAction(float[] vectorAction)
    {
        Vector2 handling = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        //rbody.AddForce(handling);
        float handle_x = Random.Range(-1f, 1f);
        float handle_y = Random.Range(-1f, 1f);
        this.gameObject.transform.Rotate(new Vector3(handle_x, handle_y));

        this.rbody.velocity = this.gameObject.transform.rotation * new Vector2(0, 20);
        
        float distance = Vector2.Distance(target.transform.positoin,this.transform.position);
        if distance < 5f {
            AddReward(3.0f)
        }
        if (this.crush)
        {
            Done();
        }
    }
    public override void CollectObservations()
    {
        CollectVideo();
        var cols = targetTexture.GetPixels();
        foreach (var col in cols)
        {
            AddVectorObs(col.b);
        }

        AddVectorObs(this.transform.position);
    }
    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    void CollectVideo()
    {
        var tex = camera.targetTexture;
        targetTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        RenderTexture.active = camera.targetTexture;
        targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        targetTexture.Apply();
    }
}
