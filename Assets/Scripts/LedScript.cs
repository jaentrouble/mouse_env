using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Zone
{
    public GameObject red;
    public GameObject green;
    public GameObject blue;
    public GameObject collider;
}

public class LedScript : MonoBehaviour
{
    public GameObject NutellaPrefab;
    public Zone zone1;
    public Zone zone2;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
