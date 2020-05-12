using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drone_controller : MonoBehaviour
{
    public Vector3 acceleration;
    Vector3 velocity;
    float position;

    public float k1;
    public float k2;

    public float k3;

    public GameObject target;

    Vector3 steady_error;
    Vector3 integrated_error;
    Vector3 derivated_error;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        derivated_error =((target.transform.position - this.gameObject.transform.position)-steady_error)/Time.deltaTime;
        steady_error  = target.transform.position- this.gameObject.transform.position;  
        integrated_error += steady_error * Time.deltaTime;

        acceleration = steady_error*k1 + integrated_error*k2 + derivated_error*k3 - new Vector3(0f,-9.8f,0f);
        velocity += acceleration * Time.deltaTime;
        // Debug.Log(velocity)
        this.gameObject.transform.position += velocity*Time.deltaTime;
        


    }
}
