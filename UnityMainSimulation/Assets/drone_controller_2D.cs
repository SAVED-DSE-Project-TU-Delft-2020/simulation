using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drone_controller_2D : MonoBehaviour

{

    public GameObject body;
    public GameObject flap;

    public GameObject cg;

    public GameObject thrust;
    public GameObject hinge;

    public float k1;
    public float k2;
    public float k3;

    Vector3 velocity = new Vector3(0f,0f,0f);
    Vector3 angular_velocity = new Vector3(0f,0f,0f);
    // Start is called before the first frame update

    Vector3 acceleration;

    float steady_error = 90f;
    float derivated_error;
    float integrated_error;

    float thrust_gain = 60f;

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


        // Rotate hinge: + gain -> + rotation
        hinge.transform.localRotation = Quaternion.Euler(0f,0f, gain);

        // Calculate moment (cross product of thrust force by cg to thrust line of action)
        Vector3 moment = Vector3.Cross(body.transform.position-cg.transform.position, thrust.transform.position- body.transform.position)*thrust_gain;
        
        // Update angular velocity (Moment of Inertia is missing)
        angular_velocity += moment*Time.deltaTime;

        // Update rotation
        this.gameObject.transform.RotateAround(cg.transform.position, angular_velocity, angular_velocity.magnitude*Time.deltaTime);     


        // Get total acceleration (thrust force + gravitiy force)
        acceleration = (thrust.transform.position - body.transform.position)*thrust_gain/48f + new Vector3(0f, -9.8f, 0f);
       
        // Update velocity
        velocity += acceleration *Time.deltaTime;

        // Update position
        this.gameObject.transform.position += velocity*Time.deltaTime;



        angle += angular_velocity.z*Time.deltaTime;
        cg.transform.position = (body.transform.position*1f+ flap.transform.position*0.5f)/1.5f;


        // Debug.Log(moment.z);
        

    }
}
