using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public GameObject player;
    public PlayerController pc;
    public float chanceFrames;
    public float requireFrames;
    public bool isWaiting = false;
    private int wait_start;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform zt in transform)
        {
            zt.GetComponent<ZoneScript>().zoneManager = this;
        }

        pc = player.GetComponent<PlayerController>();
        //DEBUG
        this.StartWaiting();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting & (Time.frameCount-wait_start>this.chanceFrames))
        {
            AllOff("all");
            this.isWaiting = false;
        }
    }
    public void StartWaiting()
    {
        isWaiting = true;
        wait_start = Time.frameCount;
        AllOn("red");
    }

    public void ReqMet()
    {

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
