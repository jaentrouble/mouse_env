using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public PlayerController player;
    public NutellaManager nutman;
    public float chanceFrames;
    public float requireFrames;
    public int rewardNutellaNum;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform zt in transform)
        {
            zt.GetComponent<ZoneScript>().zoneManager = this;
        }

        //DEBUG
        this.StartWaiting();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartWaiting()
    {
        // Select zone farther away
        Transform farthest = this.transform.GetChild(0);
        foreach(Transform zt in this.transform)
        {
            // Reset all zones
            // Make sure only one zone is waiting
            zt.GetComponent<ZoneScript>().StopWaiting();
            if (Vector3.Distance(farthest.position, player.transform.position)<
                Vector3.Distance(zt.position, player.transform.position))
            {
                farthest = zt;
            }
        }
        farthest.GetComponent<ZoneScript>().StartWaiting();

    }

    public void ReqMet()
    {
        nutman.newNutella(this.rewardNutellaNum);
    }

    private void AllOn(string color)
    {
        foreach(Transform zt in transform)
        {
            ZoneScript zs = zt.GetComponent<ZoneScript>();
            zs.LedOn(color);
        }
    }

    private void AllOff(string color)
    {
        foreach(Transform zt in transform)
        {
            ZoneScript zs = zt.GetComponent<ZoneScript>();
            zs.LedOff(color);
        }
    }

}
