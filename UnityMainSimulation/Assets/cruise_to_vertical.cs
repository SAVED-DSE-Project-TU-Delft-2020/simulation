using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cruise_to_vertical : MonoBehaviour

{

    public GameObject body;
    public GameObject flap;

    public GameObject cg;

    public GameObject thrust;
    public GameObject hinge;
    public GameObject down;
    public Vector3 lift;
    public Vector3 drag;

    public float k1;
    public float k2;
    public float k3;

    public Vector3 velocity = new Vector3(2f,0f,0f);
    Vector3 angular_velocity = new Vector3(0f,0f,0f);
    // Start is called before the first frame update

    Vector3 acceleration;

    float steady_error = 0;
    float derivated_error;
    float integrated_error;

    float thrust_gain = 15f;

    float angle = 0f;
    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        // Update cg position
        cg.transform.position = (body.transform.position*1f+ flap.transform.position*0.5f)/1.5f;

        // Get PID gains
        derivated_error =((angle-90)-steady_error)/Time.deltaTime;
        steady_error  = (angle-90);  
        integrated_error += steady_error * Time.deltaTime;  

        // Total gain
        float gain = steady_error *k1 + integrated_error * k2 + derivated_error*k3;

        // Debug.Log(gain);
        thrust_gain =  Mathf.Cos(this.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad)*44f - Mathf.Sin(this.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad)*2f;
        Debug.Log(thrust_gain);

    
        // Rotate hinge: + gain -> + rotation
        hinge.transform.localRotation = Quaternion.Euler(0f,0f, gain);

        // Calculate moment (cross product of thrust force by cg to thrust line of action)
        Vector3 moment = Vector3.Cross(body.transform.position-cg.transform.position, thrust.transform.position- body.transform.position)*thrust_gain;
        
        // Update angular velocity (Moment of Inertia is missing)
        angular_velocity += moment*Time.deltaTime*4;

        // Update rotation
        this.gameObject.transform.RotateAround(cg.transform.position, angular_velocity, angular_velocity.magnitude*Time.deltaTime);     

        lift = Vector3.Cross((thrust.transform.position-body.transform.position) * Mathf.Max(0, Vector3.Dot(velocity,Vector3.Normalize(thrust.transform.position-body.transform.position))), new Vector3(0f,0f, -1f))*0.192f;
        drag = -velocity*Mathf.Abs(Vector3.Dot(down.transform.position-body.transform.position,Vector3.Normalize(velocity)))*0.2f;
        
        // lift = Vector3.Dot(Vector3.Normalize(thrust.transform.position-body.transform.position), Vector3.Normalize(velocity)) *new Vector3(-9* Mathf.Sin(this.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad +Mathf.PI/2f), 9* Mathf.Cos(this.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad +Mathf.PI/2), 0f);
        // Get total acceleration (thrust force + gravitiy force)
        acceleration = (thrust.transform.position - body.transform.position)*thrust_gain/48f + new Vector3(0f, -9.8f, 0f) + lift + drag;
       
        // Update velocity
        velocity += acceleration *Time.deltaTime;

        // Update position
        this.gameObject.transform.position += velocity*Time.deltaTime;



        angle += angular_velocity.z*Time.deltaTime;
        cg.transform.position = (body.transform.position*1f+ flap.transform.position*0.5f)/1.5f;


        // Debug.Log(moment.z);
        

    }
}
