using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject ClockwiseLed;
    public GameObject CounterClockwiseLed;
    public GameObject ButtonLedClock;
    public GameObject ButtonLedCounterClock;
    public GameObject ButtonClock;
    public GameObject ButtonCounterClock;
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
        Clock,
        CounterClock,
        End
    }

    public enum ButtonMode
    {
        Clock,
        CounterClock,
        Random,
        Off
    }
    // Start is called before the first frame update
    void Start()
    {
        AllLeds.Add(ClockwiseLed);
        AllLeds.Add(CounterClockwiseLed);
        AllLeds.Add(ButtonLedClock);
        AllLeds.Add(ButtonLedCounterClock);
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
            case ButtonMode.Clock:
                targetButton = this.ButtonClock;
                break;
            case ButtonMode.CounterClock:
                targetButton = this.ButtonCounterClock;
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

    public void ClockwiseLedSwitch(bool On)
    {
        if (On) TurnOnLed(ClockwiseLed);
        else TurnOffLed(ClockwiseLed);
    }

    public void CounterClockwiseLedSwitch(bool On)
    {
            if(On) TurnOnLed(CounterClockwiseLed);
            else TurnOffLed(CounterClockwiseLed);
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
            case ButtonMode.Clock:
                TurnOnLed(this.ButtonLedClock);
                TurnOffLed(this.ButtonLedCounterClock);
                break;
            case ButtonMode.CounterClock:
                TurnOffLed(this.ButtonLedClock);
                TurnOnLed(this.ButtonLedCounterClock);
                break;
            case ButtonMode.Off:
                TurnOffLed(this.ButtonLedClock);
                TurnOffLed(this.ButtonLedCounterClock);
                break;
            case ButtonMode.Random:
                if(Random.value < 0.5)
                    this.SetButtonMode(ButtonMode.Clock);
                else
                    this.SetButtonMode(ButtonMode.CounterClock);
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
            case DirectionMode.Clock:
                this.ClockwiseLedSwitch(true);
                this.CounterClockwiseLedSwitch(false);
                break;
            case DirectionMode.CounterClock:
                this.ClockwiseLedSwitch(false);
                this.CounterClockwiseLedSwitch(true);
                break;
            case DirectionMode.End:
                this.ClockwiseLedSwitch(false);
                this.CounterClockwiseLedSwitch(false);
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
        this.ButtonUnPushed(this.ButtonClock);
        this.ButtonUnPushed(this.ButtonCounterClock);
        this._roomEntered = false;
    }

}
