using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
     
    Debug.Log("Start of start");

     if (Application.isEditor) {
     
     }   
        else {
            //Turns off the stack trace for Deub.Log to cut down on log noise (copied this so I could look back and understand what is happening here in the future)
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Debug.Log("after disabled");
        }
        Debug.Log("end of start");
    }

    private void OnGUI() {
        NetworkHelper.GUILayoutNetworkControls();
    }
}
