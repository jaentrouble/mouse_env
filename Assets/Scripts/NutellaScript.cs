using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutellaScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.checkCol();
    }

    private void checkCol()
    {
        Collider[] hitcols = Physics.OverlapSphere(
            this.transform.position,
            this.GetComponent<SphereCollider>().radius
        );
        foreach (Collider col in hitcols)
        {
            if(col.tag == "Head")
            {
                col.GetComponentInParent<PlayerController>().eatNutella();
                Destroy(this);
            }
        }
    }
}
