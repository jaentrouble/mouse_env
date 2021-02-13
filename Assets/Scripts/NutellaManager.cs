using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutellaManager : MonoBehaviour
{
    public GameObject nutellaPrefab;
    public Vector3 max_pos;
    public Vector3 min_pos;
    public float nutellaOffset;
    // Player object needed
    // to make sure the apple is not spawned on the player
    public GameObject player;
    public int nutellaNumber = 1;

    private GameObject[] nutellas;

    // Start is called before the first frame update
    void Start()
    {
        nutellas = new GameObject[nutellaNumber];
        for(int i=0;i<nutellaNumber;i++)
        {
            nutellas[i] =
                Instantiate(nutellaPrefab, getNewPos(), nutellaPrefab.transform.rotation);
            NutellaScript n = nutellas[i].GetComponent<NutellaScript>();
            n.manager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Vector3 getNewPos()
    {
        Vector3 new_pos = new Vector3(
            Random.Range(min_pos.x,max_pos.x),
            Random.Range(min_pos.y,max_pos.y),
            Random.Range(min_pos.z,max_pos.z)
        );
        if (Vector3.Distance(new_pos, player.transform.position)<nutellaOffset)
        {
            return getNewPos();
        }
        else
        {
            return new_pos;
        }
    }

    public void resetNutellas()
    {
        foreach(GameObject nutella in nutellas)
        {
            nutella.GetComponent<NutellaScript>().resetPos();
        }
    }

    public float minDistToNutella(Vector3 pos)
    {
        float[] dists = new float[nutellaNumber];
        for(int i=0;i<nutellaNumber;i++)
        {
            dists[i] =
                Vector3.Distance(pos, nutellas[i].transform.position);
        }
        float minDist = dists[0];
        foreach(float d in dists)
        {
            if(d < minDist)
            {
                minDist = d;
            }
        }
        return minDist;
    }
}
