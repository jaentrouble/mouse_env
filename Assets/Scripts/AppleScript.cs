using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleScript : MonoBehaviour
{
    // Player object needed
    // to make sure the apple is not spawned on the player
    public AppleManager manager;
    public bool isColliding;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hit_mouse()
    {
        if (isColliding) return;
        isColliding = true;
        reset_pos();
        StartCoroutine(reset_collide());
    }
    private IEnumerator reset_collide()
    {
        yield return new WaitForEndOfFrame();
        isColliding = false;
    }

    public void reset_pos()
    {
        transform.position = manager.getNewPos();
    }
}
