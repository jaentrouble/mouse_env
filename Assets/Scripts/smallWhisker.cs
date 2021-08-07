using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smallWhisker : MonoBehaviour
{
    Renderer rend;

    private float max_distance;
    // Start is called before the first frame update
    void Start()
    {
        rend = transform.Find("Cylinder").GetComponent<Renderer>();
        max_distance = this.transform.localScale.z;
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
        if (Physics.Raycast(transform.position, fwd, out hit, max_distance, noPlayer, QueryTriggerInteraction.Ignore))
        {
            rend.material.color = Color.red;
            return hit.distance;
        }
        else
        {
            rend.material.color = Color.white;
            return max_distance;
        }
    }

}
