using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBehaviour : MonoBehaviour
{
    public bool isColliding = false;
    void OnTriggerEnter(Collider other){
        isColliding = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
