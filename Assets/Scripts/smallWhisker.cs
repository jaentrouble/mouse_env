using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smallWhisker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float raycast()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        LayerMask noPlayer = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(transform.position, fwd, out hit, 1, noPlayer, QueryTriggerInteraction.Ignore))
        {
            return hit.distance;
        }
        else
        {
            return 1.0f;
        }
    }

}
