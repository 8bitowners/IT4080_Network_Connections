using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ToggleScores : MonoBehaviour
{
    private Canvas canvas;
    void Start(){
        canvas = GetComponent<Canvas>();
    }
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Tab)){
            Debug.Log("You hit tab");
            canvas.enabled = !canvas.enabled;
        }
    }
}