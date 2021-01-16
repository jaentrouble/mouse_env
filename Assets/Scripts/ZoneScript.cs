using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float chanceFrames;
    public float requireFrames;

    private Material red_mat;
    private Material green_mat;
    private Material blue_mat;
    private Renderer red_ren;
    private Renderer green_ren;
    private Renderer blue_ren;
    private bool red_on = true;
    private bool green_on = true;
    private bool blue_on = true;
    private bool isWaiting = false;
    private float wait_start;
    private float in_start;
    private bool is_in = false;

    void Start()
    {
        red_ren = transform.Find("red").gameObject.GetComponent<Renderer>();
        green_ren = transform.Find("green").gameObject.GetComponent<Renderer>();
        blue_ren = transform.Find("blue").gameObject.GetComponent<Renderer>();
        red_mat = red_ren.material;
        green_mat = green_ren.material;
        blue_mat = blue_ren.material;
        red_mat.EnableKeyword("_EMISSION");
        green_mat.EnableKeyword("_EMISSION");
        blue_mat.EnableKeyword("_EMISSION");
        this.LedOff("all");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            this.LedOff("all");
            this.LedOn("red");
        }
        else if (Time.frameCount % 40 == 0)
        {
            this.LedOff("all");
            this.LedOn("green");
        }
        else if (Time.frameCount % 50 == 0)
        {
            this.LedOff("all");
            this.LedOn("blue");
        }

        if (isWaiting & (Time.frameCount-wait_start>chanceFrames))
        {
            this.LedOff("all");
            this.isWaiting = false;
        }
    }

    public void LedOn(string color)
    {
        switch (color)
        {
            case "red":
                if (!red_on)
                {
                    red_mat.SetColor("_EmissionColor", Color.red);
                    RendererExtensions.UpdateGIMaterials(red_ren);
                    DynamicGI.SetEmissive(red_ren, Color.red);
                    DynamicGI.UpdateEnvironment();
                    red_on = true;
                }
                break;
            case "green":
                if (!green_on)
                {
                    green_mat.SetColor("_EmissionColor",Color.green);
                    RendererExtensions.UpdateGIMaterials(green_ren);
                    DynamicGI.SetEmissive(green_ren, Color.green);
                    DynamicGI.UpdateEnvironment();
                    green_on = true;
                }
                break;
            case "blue":
                if (!blue_on)
                {
                    blue_mat.SetColor("_EmissionColor",Color.blue);
                    RendererExtensions.UpdateGIMaterials(blue_ren);
                    DynamicGI.SetEmissive(blue_ren, Color.blue);
                    DynamicGI.UpdateEnvironment();
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
                    RendererExtensions.UpdateGIMaterials(red_ren);
                    DynamicGI.SetEmissive(red_ren, Color.black);
                    DynamicGI.UpdateEnvironment();
                    red_on = false;
                }
                break;
            case "green":
                if (green_on)
                {
                    green_mat.SetColor("_EmissionColor",Color.black);
                    RendererExtensions.UpdateGIMaterials(green_ren);
                    DynamicGI.SetEmissive(green_ren, Color.black);
                    DynamicGI.UpdateEnvironment();
                    green_on = false;
                }
                break;
            case "blue":
                if (blue_on)
                {
                    blue_mat.SetColor("_EmissionColor",Color.black);
                    RendererExtensions.UpdateGIMaterials(blue_ren);
                    DynamicGI.SetEmissive(blue_ren, Color.black);
                    DynamicGI.UpdateEnvironment();
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
        wait_start = Time.frameCount;
        this.LedOn("red");
    }

    public void InZone(PlayerController p)
    {
        if (isWaiting)
        {
            this.LedOn("green");
            if (!is_in)
            {
                is_in = true;
                in_start = Time.frameCount;
            } else if (Time.frameCount - in_start > requireFrames)
            {
                this.LedOn("blue");
                this.LedOff("red");
                this.LedOff("green");
                // Drop food
                p.nutellaManager.newNutella(1);
                this.isWaiting = false;
            }
        }
    }

    public void NotInZone(GameObject player)
    {
        if (isWaiting)
        {
            this.LedOff("green");
            if (is_in)
            {
                is_in = false;
            }
        }
    }
}
