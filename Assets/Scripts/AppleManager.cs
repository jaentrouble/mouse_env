using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour
{
    public GameObject applePrefab;
    public int apple_num;
    public Vector3 max_pos;
    public Vector3 min_pos;
    public float apple_offset;
    // Player object needed
    // to make sure the apple is not spawned on the player
    public GameObject player;
    // Start is called before the first frame update
    private GameObject[] all_apples;
    void Start()
    {
        all_apples = new GameObject[apple_num];
        for(int i=0;i<apple_num;i++)
        {
            all_apples[i] = 
                Instantiate(applePrefab, getNewPos(), applePrefab.transform.rotation);
            AppleScript a = all_apples[i].GetComponent<AppleScript>();
            a.manager = this;
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
        if (Vector3.Distance(new_pos, player.transform.position)<apple_offset)
        {
            return getNewPos();
        }
        else
        {
            return new_pos;
        }
    }

    public void reset_apples()
    {
        foreach(GameObject apple in all_apples)
        {
            apple.GetComponent<AppleScript>().reset_pos();
        }
    }
}
