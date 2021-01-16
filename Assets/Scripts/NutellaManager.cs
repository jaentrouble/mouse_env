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

    private List<GameObject> nutellas;

    // Start is called before the first frame update
    void Start()
    {
        nutellas = new List<GameObject>();
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

    public void newNutella(int numNutella)
    {
        for(int i=0;i<numNutella;i++)
        {
            nutellas.Add(
            Instantiate(nutellaPrefab, getNewPos(), nutellaPrefab.transform.rotation));
        }
        
    }

    public void clearNutella()
    {
        foreach(GameObject nut in nutellas)
        {
            Destroy(nut);
        }
        nutellas = new List<GameObject>();
    }

}
