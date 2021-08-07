using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Text;

[Serializable]
public class Control_JSON
{
    public float move;
    public float turn;
    public bool reset = false;
    public bool render = false; //View from top
    public Control_JSON(float init_move, float init_turn)
    {
        move = init_move;
        turn = init_turn;
    }
}

[Serializable]
public class GameInfo_JSON
{
    public float reward=0.0f;
    public bool done=false;
}

[Serializable]
public class Window_size
{
    public int width;
    public int height;
}

[Serializable]
public class MessageInfo_JSON
{
    public int length;
}
public class PlayerController : MonoBehaviour
{
    public float FixedYPos;
    public GameObject StartPoint;
    public Camera cam_obs;
    public Camera cam_render;
    public MapManager mapManager;


    public Window_size render_info;
    public Window_size obs_info;

    private GameObject Head;

    private Control_JSON control_json;
    private GameInfo_JSON gameinfo_json;
    private MessageInfo_JSON msginfo_json;

    //Snapshot - Render
    private Texture2D image_ren;
    private RenderTexture rt_ren;
    private IEnumerator RenCapture;
    private Color32[] ren_array;
    private byte[] ren_bytes;


    //Snapshot - Observe
    private Texture2D image_obs;
    private RenderTexture rt_obs;
    private IEnumerator ObsCapture;
    private Color32[] obs_array;
    private byte[] obs_bytes;

    //Snapshot - Whisker
    public WhiskerScript whisker;
    private byte[] whi_bytes;

    private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    private int single_length = Marshal.SizeOf(typeof(Color32));
    private int total_length;
    private string gameinfo_str;
    private byte[] gameinfo_byte;
    private UTF8Encoding encoder = new UTF8Encoding();

    // Server
    public int port = 7777;
    private string ip = "127.0.0.1";
    private TcpClient client;
    private NetworkStream net_stream;
    
    // Start is called before the first frame update
    void Start()
    {

        this.Head = transform.Find("Head").gameObject;

        rt_obs = new RenderTexture(obs_info.width,obs_info.height,24,RenderTextureFormat.ARGB32);
        cam_obs.targetTexture = rt_obs;

        image_obs = new Texture2D(obs_info.width, obs_info.height);
        ObsCapture = CaptureObserve();
        ObsCapture.MoveNext();
        

        rt_ren = new RenderTexture(
            render_info.width,render_info.height,24,RenderTextureFormat.ARGB32);
        cam_render.targetTexture = rt_ren;

        image_ren = new Texture2D(render_info.width,render_info.height);
        RenCapture = CaptureRender();
        RenCapture.MoveNext();

        setupSocket();

        control_json = new Control_JSON(0.0f, 0.0f);
        gameinfo_json = new GameInfo_JSON();
        msginfo_json = new MessageInfo_JSON();

    }

    // Update is called once per frame
    void Update()
    {
        // To make sure physics is fast
        if (Time.deltaTime < Time.fixedDeltaTime)
        {
            Time.fixedDeltaTime = Time.deltaTime;
        }
        while (true)
        {
            receive();
            if (control_json.render)
            {
                control_json.render = false;
                send_render();
            }
            else
            {
                if (control_json.reset)
                {
                    control_json.reset=false;
                    this.resetGame();
                }
                break;
            }
        }
        

        transform.Rotate(0,control_json.turn,0);
        transform.Translate(0,0,control_json.move);
        limitPos();

        Step();


    }
    private void LateUpdate() 
    {
        send();
        
        // Reset reward after sending
        gameinfo_json.reward = 0.0f;
    }
    void OnApplicationQuit() 
    {
        //DEBUG
        client.Close();
    }


    private IEnumerator CaptureObserve()
    {   
        while (true) {
            yield return frameEnd;
            RenderTexture.active = cam_obs.targetTexture;
            cam_obs.Render();
            
            image_obs.ReadPixels(new Rect(0,0,obs_info.width,obs_info.height),0,0);
            image_obs.Apply();
            obs_array = image_obs.GetPixels32();
            obs_bytes = Color32ArrayToByteArray(obs_array);
            
        }
    }

    private IEnumerator CaptureRender()
    {   
        while (true) {
            yield return frameEnd;
            RenderTexture.active = cam_render.targetTexture;
            cam_render.Render();
            
            image_ren.ReadPixels(new Rect(0,0,render_info.width,render_info.height),0,0);
            image_ren.Apply();
            ren_array = image_ren.GetPixels32();
            ren_bytes = Color32ArrayToByteArray(ren_array);
            
        }
    }

    private byte[] Color32ArrayToByteArray(Color32[] colors)
    {
        total_length = single_length * colors.Length;
        byte[] bytes = new byte[total_length];

        GCHandle handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        Marshal.Copy(ptr, bytes, 0, total_length);
        handle.Free();
        return bytes;
    }

    private void setupSocket()
    {
        try
        {
            client = new TcpClient(ip, port);
            net_stream = client.GetStream();
        }
        catch (Exception e)
        {
            Debug.Log("socket error: " + e);
            Application.Quit();
        }

    }

    private void receive()
    {
        // Get request from server
        byte[] server_answer = new byte [65536];
        int nbytes = net_stream.Read(server_answer, 0, server_answer.Length);
        
        string output = encoder.GetString(server_answer, 0, nbytes);
        JsonUtility.FromJsonOverwrite(output, control_json);
    }

    private void send()
    {
        

        gameinfo_str = JsonUtility.ToJson(gameinfo_json);
        gameinfo_byte = new byte[encoder.GetByteCount(gameinfo_str)];
        int gameinfo_bytecount = encoder.GetBytes(gameinfo_str,0,gameinfo_str.Length,gameinfo_byte,0);

        ObsCapture.MoveNext();

        //Whisker
        this.getWhisker();
        
        byte[] to_send = new byte[obs_bytes.Length+whi_bytes.Length+gameinfo_bytecount];
        Buffer.BlockCopy(obs_bytes,0,to_send,0,obs_bytes.Length);
        Buffer.BlockCopy(whi_bytes,0,to_send,obs_bytes.Length,whi_bytes.Length);
        Buffer.BlockCopy(gameinfo_byte,0,to_send,obs_bytes.Length+whi_bytes.Length,gameinfo_bytecount);

        // msginfo_json.length = to_send.Length;
        // string msginfo_str = JsonUtility.ToJson(msginfo_json);
        string msginfo_str = to_send.Length.ToString();
        byte[] msginfo_byte = new byte[encoder.GetByteCount(msginfo_str)];
        encoder.GetBytes(msginfo_str,0,msginfo_str.Length,msginfo_byte,0);
        try
        {        
            net_stream.Write(msginfo_byte,0,msginfo_byte.Length);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Application.Quit();
        }

        byte[] server_answer = new byte [65536];
        int nbytes = net_stream.Read(server_answer, 0, server_answer.Length);
        
        try
        {
            net_stream.Write(to_send, 0, to_send.Length);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Application.Quit();
        }
    }

    private void send_render()
    {
        RenCapture.MoveNext();
        byte[] to_send = new byte[ren_bytes.Length];
        Buffer.BlockCopy(ren_bytes,0,to_send,0,ren_bytes.Length);
        
        // msginfo_json.length = to_send.Length;
        // string msginfo_str = JsonUtility.ToJson(msginfo_json);
        string msginfo_str = to_send.Length.ToString();
        byte[] msginfo_byte = new byte[encoder.GetByteCount(msginfo_str)];
        encoder.GetBytes(msginfo_str,0,msginfo_str.Length,msginfo_byte,0);
        try
        {        
            net_stream.Write(msginfo_byte,0,msginfo_byte.Length);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Application.Quit();
        }

        byte[] server_answer = new byte [65536];
        int nbytes = net_stream.Read(server_answer, 0, server_answer.Length);

        try
        {
            net_stream.Write(to_send, 0, to_send.Length);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Application.Quit();
        }
    }
    public void setNewPos()
    {
        Vector3 new_pos = new Vector3(
            StartPoint.transform.position.x,
            FixedYPos,
            StartPoint.transform.position.z
        );
        transform.position = new_pos;
        transform.eulerAngles = 
            new Vector3(0, StartPoint.transform.eulerAngles.y,0);
    }

    private void limitPos()
    {
        this.transform.localEulerAngles = 
            new Vector3(0, this.transform.localEulerAngles.y, 0);
        Vector3 newpos = new Vector3(this.transform.localPosition.x,
                                    FixedYPos,
                                    this.transform.localPosition.z);
        this.transform.localPosition = newpos;
    }
    
    private void doneGame()
    {
        gameinfo_json.done = true;
    }

    private void resetGame()
    {
        this.setNewPos();
        this.gameinfo_json = new GameInfo_JSON();
        this.mapManager.ResetMap();
    }

    private void getWhisker()
    {
        float[] whi_fl = whisker.Sensor();
        whi_bytes = new byte[whi_fl.Length * sizeof(float)];
        Buffer.BlockCopy(whi_fl, 0, whi_bytes, 0, whi_bytes.Length);
    }

    private void Step()
    {
        this.gameinfo_json.reward += this.mapManager.checkReward(
            this.Head.transform.position
        );
        if (this.mapManager.done)
            this.doneGame();
    }

}
