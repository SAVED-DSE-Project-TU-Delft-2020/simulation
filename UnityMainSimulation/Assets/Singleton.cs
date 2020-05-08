using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton instance;

    public GameObject landing_base;
    public GameObject drone;
    // Start is called before the first frame update

    void Awake(){
        instance = this;
    }
}
