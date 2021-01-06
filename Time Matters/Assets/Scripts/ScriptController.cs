using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptController : MonoBehaviour
{
    int currentScriptIx = 0;
    string[] script = new string[] { "Hello", "My Name is", "MyeongKi", "The end"};
    Text story;

    private void Start()
    {
        // script load
        story = GameObject.Find("Story").GetComponent<Text>();
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
