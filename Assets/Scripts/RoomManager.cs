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
    private DirectionMode CurrentDirection = DirectionMode.End;
    private ButtonMode CurrentButtonMode = ButtonMode.Off;
    private bool _roomEntered = false;
    public bool roomEntered
    {
        get {return this._roomEntered;}
    }

    public enum DirectionMode
    {
        Long,
        Short,
        End
    }

    public enum ButtonMode
    {
        Long,
        Short,
        Random,
        Off
    }
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

    public bool ButtonCollideCheck(Vector3 point)
    {
        GameObject targetButton;
        switch(this.CurrentButtonMode)
        {
            case ButtonMode.Long:
                targetButton = this.ButtonLong;
                break;
            case ButtonMode.Short:
                targetButton = this.ButtonShort;
                break;
            default:
                return false;
        }
        Bounds buttonBound = targetButton.GetComponent<Collider>().bounds;
        if(buttonBound.Contains(point))
        {
            this.ButtonPushed(targetButton);
            this.SetButtonMode(ButtonMode.Off);
            return true;
        }
        else return false;
    }

    public bool RoomCollideCheck(Vector3 point)
    {
        Bounds roomBound = this.GetComponent<Collider>().bounds;
        if(roomBound.Contains(point))
        {
            this._roomEntered = true;
            return true;
        }
        else return false;
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

    public void CorridorLongSwitch(bool On)
    {
        foreach(GameObject Led in this.CorridorLedLong)
        {
            if (On) TurnOnLed(Led);
            else TurnOffLed(Led);
        }
    }

    public void CorridorShortSwitch(bool On)
    {
        foreach(GameObject Led in this.CorridorLedShort)
        {
            if(On) TurnOnLed(Led);
            else TurnOffLed(Led);
        }
    }
    private void ButtonPushed(GameObject targetButton)
    {
        targetButton.transform.localScale
            = new Vector3(
                0.5f,
                targetButton.transform.localScale.y,
                targetButton.transform.localScale.z
            );
    }
    private void ButtonUnPushed(GameObject targetButton)
    {
        targetButton.transform.localScale
            = new Vector3(
                1.5f,
                targetButton.transform.localScale.y,
                targetButton.transform.localScale.z
            );
    }


    private void SetButtonMode(ButtonMode mode)
    {
        switch (mode)
        {
            case ButtonMode.Long:
                TurnOnLed(this.ButtonLedLong);
                TurnOffLed(this.ButtonLedShort);
                break;
            case ButtonMode.Short:
                TurnOffLed(this.ButtonLedLong);
                TurnOnLed(this.ButtonLedShort);
                break;
            case ButtonMode.Off:
                TurnOffLed(this.ButtonLedLong);
                TurnOffLed(this.ButtonLedShort);
                break;
            case ButtonMode.Random:
                if(Random.value < 0.5)
                    this.SetButtonMode(ButtonMode.Long);
                else
                    this.SetButtonMode(ButtonMode.Short);
                // Becareful to not set the current mode to Random
                // Return here
                return;
        }
        this.CurrentButtonMode = mode;
    }

    private void SetDirectionMode(DirectionMode mode)
    {
        switch(mode)
        {
            case DirectionMode.Long:
                this.CorridorLongSwitch(true);
                this.CorridorShortSwitch(false);
                break;
            case DirectionMode.Short:
                this.CorridorLongSwitch(false);
                this.CorridorShortSwitch(true);
                break;
            case DirectionMode.End:
                this.CorridorLongSwitch(false);
                this.CorridorShortSwitch(false);
                break;
        }
        this.CurrentDirection = mode;
    }

    public void SetMode(DirectionMode DM, ButtonMode BM)
    {
        this.SetButtonMode(BM);
        this.SetDirectionMode(DM);
    }
    public void ResetRoom()
    {
        this.SetMode(DirectionMode.End, ButtonMode.Off);
        this.ButtonUnPushed(this.ButtonLong);
        this.ButtonUnPushed(this.ButtonShort);
        this._roomEntered = false;
    }

}
