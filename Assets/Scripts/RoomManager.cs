using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] CorridorLedLong;
    public GameObject[] CorridorLedShort;
    public GameObject ButtonLedLong;
    public GameObject ButtonLedShort;
    public GameObject ButtonLong;
    public GameObject ButtonShort;
    public Color EmissionColor = new Color(0.25f,0.25f,0.25f);
    private List<GameObject> AllLeds = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        AllLeds.AddRange(CorridorLedLong);
        AllLeds.AddRange(CorridorLedShort);
        AllLeds.Add(ButtonLedLong);
        AllLeds.Add(ButtonLedShort);
        foreach (GameObject Led in AllLeds)
        {
            Renderer ren = Led.GetComponent<Renderer>();
            ren.material.EnableKeyword("_EMISSION");
            TurnOffLed(Led);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOffAllLeds()
    {
        foreach (GameObject Led in this.AllLeds)
        {
            TurnOffLed(Led);
        }
    }

    private void TurnOnLed(GameObject Led)
    {
        Renderer ren = Led.GetComponent<Renderer>();
        ren.material.SetColor("_EmissionColor", EmissionColor);
        RendererExtensions.UpdateGIMaterials(ren);
        DynamicGI.SetEmissive(ren, EmissionColor);
        DynamicGI.UpdateEnvironment();
    }

    private void TurnOffLed(GameObject Led)
    {
        Renderer ren = Led.GetComponent<Renderer>();
        ren.material.SetColor("_EmissionColor", Color.black);
        RendererExtensions.UpdateGIMaterials(ren);
        DynamicGI.SetEmissive(ren, Color.black);
        DynamicGI.UpdateEnvironment();
    }
}
