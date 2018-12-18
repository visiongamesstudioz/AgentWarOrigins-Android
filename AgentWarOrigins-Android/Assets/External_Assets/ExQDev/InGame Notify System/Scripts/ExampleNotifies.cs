using UnityEngine;
using System.Collections;

public class ExampleNotifies : MonoBehaviour {

    public Texture2D imgOff;
    public string yourMSG = "";

    // Use this for initialization
    void Start () {
	
	}
    public void MSGToDebug()
    {
        Debug.Log("Notify event activated!");
    }
    public void CreateNewNotifyOnHide()
    {
        NotifyData.AddNew("Previous was hidden and invoked me!");
    }
    public void CreateNewNotifyOnShow()
    {
        NotifyData.AddNew("Previous invoked me immediately");
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NotifyData.AddNew("Hello!", MSGToDebug, CreateNewNotifyOnHide);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            NotifyData.AddNew("Keycode W!");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
    //        NotifyData.AddNew("Turning off...", imgOff);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            NotifyData.AddNew(yourMSG);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            NotifyData.AddNew("Invoke immediately..", CreateNewNotifyOnShow);
        }
    }
}
