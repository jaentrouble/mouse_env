using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public RoomManager StartRoom;
    public RoomManager ClockRoom;
    public RoomManager CounterClockRoom;
    public RoomManager FarRoom;
    private List<RoomManager> allRooms;
    public ResetMode resetMode = ResetMode.Random;
    public float correctRoomReward = 0.0f;
    public float correctButtonReward = 1.0f;
    public float finalReward = 10.0f;
    private List<RoomManager> targetRooms;
    private Dictionary<Path,List<RoomManager>> pathDict;
    private int targetRoomIndex = 1;
    private bool _done = false;
    public bool done
    {
        get {return this._done;}
    }
    public enum ResetMode
    {
        Clock,
        CounterClock,
        Random
    }
    public enum Path
    {
        Clock,
        CounterClock
    }
    // Start is called before the first frame update
    void Start()
    {
        allRooms = new List<RoomManager>(){
            StartRoom, ClockRoom, CounterClockRoom, FarRoom
        };

        pathDict = new Dictionary<Path, List<RoomManager>>();
        pathDict.Add(Path.Clock, new List<RoomManager>(){
            StartRoom, ClockRoom, FarRoom
        });
        pathDict.Add(Path.CounterClock, new List<RoomManager>(){
            StartRoom, CounterClockRoom, FarRoom
        });

        this.ResetMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void SetPath(Path targetPath)
    {
        RoomManager.DirectionMode direction;
        switch(targetPath)
        {
            case Path.Clock:
                direction = RoomManager.DirectionMode.Clock;
                break;
            case Path.CounterClock:
                direction = RoomManager.DirectionMode.CounterClock;
                break;
            default:
                throw new System.ArgumentException(
                    "Invalid Path"
                );
        }
        this.targetRooms = pathDict[targetPath];
        // Start room setting
        this.targetRooms[0].SetMode(direction, RoomManager.ButtonMode.Off);
        // Final room setting
        this.targetRooms[this.targetRooms.Count-1].SetMode(
                                RoomManager.DirectionMode.End,
                                RoomManager.ButtonMode.Random);
        // Intermediate rooms
        for (int idx=1; (this.targetRooms.Count-idx)>1; idx ++)
        {
            this.targetRooms[idx].SetMode(
                direction, RoomManager.ButtonMode.Random
            );
        }
    }
    public void ResetMap()
    {
        foreach(RoomManager room in this.allRooms)
        {
            room.SetMode(RoomManager.DirectionMode.End,
                         RoomManager.ButtonMode.Off);
        }
        Debug.Log("here");
        switch(this.resetMode)
        {
            case ResetMode.Clock:
                this.SetPath(Path.Clock);
                break;
            case ResetMode.CounterClock:
                this.SetPath(Path.CounterClock);
                break;
            case ResetMode.Random:
                if(Random.value<0.5)
                {
                    this.SetPath(Path.Clock);
                }
                else
                {
                    this.SetPath(Path.CounterClock);
                }
                break;
            default:
                throw new System.ArgumentException(
                    "Invalid ResetMode"
                );
        }
        this.targetRoomIndex = 1;
        this._done = false;
    }
    public float checkReward(Vector3 headPos)
    {
        if (this.targetRoomIndex == this.targetRooms.Count)
            return 0.0f;

        float reward = 0.0f;
        RoomManager tRoom = this.targetRooms[this.targetRoomIndex];
        if (!tRoom.roomEntered)
        {
            if(tRoom.RoomCollideCheck(headPos))
            {
                reward += this.correctRoomReward;
            }
        }
        if (tRoom.ButtonCollideCheck(headPos))
        {
            reward += this.correctButtonReward;
            this.targetRoomIndex += 1;
            if(this.targetRoomIndex==this.targetRooms.Count)
            {
                reward += this.finalReward;
                this._done = true;
            }
        }
        return reward;
    }

}
