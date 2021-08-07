using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiskerScript : MonoBehaviour
{
    public List<GameObject> allWhiskers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float[] Sensor()
    {
        float[] output = new float[allWhiskers.Count];
        for(int i=0;i<allWhiskers.Count;i++)
        {
            output[i] = allWhiskers[i].GetComponent<smallWhisker>().raycast();
        }

        return output;
    }
    
}
