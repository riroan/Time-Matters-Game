using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScriptController : MonoBehaviour
{
    int currentScriptIx = 0;
    string[] script;
    Text story;
    
    private void Start()
    {
        // script load
        story = GameObject.Find("Story").GetComponent<Text>();
        script = File.ReadAllLines("Assets/Story/prologue.txt");
        Debug.Log(script.Length);
    }
    private void Update()
    {
    }


    private void OnMouseUp()
    {
        if (gameObject.name == "board")
        {
            if (script.Length > currentScriptIx)
            {
                story.text = script[currentScriptIx++];
            }
        }
    }
}
