using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float chance;
    public float require;

    private Material red_mat;
    private Material green_mat;
    private Material blue_mat;
    private bool red_on = false;
    private bool green_on = false;
    private bool blue_on = false;
    private bool isWaiting = false;
    private float wait_start;

    void Start()
    {
        red_mat = transform.Find("red").gameObject.GetComponent<Renderer>().material;
        green_mat = transform.Find("green").gameObject.GetComponent<Renderer>().material;
        blue_mat = transform.Find("blue").gameObject.GetComponent<Renderer>().material;
        red_mat.EnableKeyword("_EMISSION");
        green_mat.EnableKeyword("_EMISSION");
        blue_mat.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LedOn(string color)
    {
        switch (color)
        {
            case "red":
                if (!red_on)
                {
                    red_mat.SetColor("_EmissionColor", Color.red);
                    red_on = true;
                }
                break;
            case "green":
                if (!green_on)
                {
                    green_mat.SetColor("_EmissionColor",Color.green);
                    green_on = true;
                }
                break;
            case "blue":
                if (!blue_on)
                {
                    blue_mat.SetColor("_EmissionColor",Color.blue);
                    blue_on = true;
                }
                break;
            case "all":
                this.LedOn("red");
                this.LedOn("green");
                this.LedOn("blue");
                break;
        }
    }

    public void LedOff(string color)
    {
        switch (color)
        {
            case "red":
                if (red_on)
                {
                    red_mat.SetColor("_EmissionColor", Color.black);
                    red_on = false;
                }
                break;
            case "green":
                if (green_on)
                {
                    green_mat.SetColor("_EmissionColor",Color.black);
                    green_on = false;
                }
                break;
            case "blue":
                if (blue_on)
                {
                    blue_mat.SetColor("_EmissionColor",Color.black);
                    blue_on = false;
                }
                break;
            case "all":
                this.LedOff("red");
                this.LedOff("green");
                this.LedOff("blue");
                break;
        }
    }

    public void StartWaiting()
    {
        isWaiting = true;
        wait_start = Time.time;
        this.LedOn("red");
    }

    public void InZone(GameObject player)
    {
        if (isWaiting)
        {
            this.LedOn("green");
        }
    }
}
