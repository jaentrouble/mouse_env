using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsCamera : MonoBehaviour
{
    public List<Light> HiddenLights;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPreCull() {
        foreach (Light light in this.HiddenLights){
            light.enabled = false;
        }
    }

    private void OnPostRender() {
        foreach (Light light in this.HiddenLights){
            light.enabled = true;
        }
    }

}
