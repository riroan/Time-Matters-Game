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
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray touchray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(touchray, out hit);

            if (hit.collider != null)
            {
                Debug.Log(hit);
                Debug.Log(hit.collider.gameObject.name);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (script.Length > currentScriptIx)
            {
                story.text = script[currentScriptIx++];
                Debug.Log(currentScriptIx);
            }
        }
    }

    bool isTouched()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray touchray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(touchray, out hit);

            if (hit.collider != null)
            {
                Debug.Log(hit);
                Debug.Log(hit.collider.gameObject.name);
                return true;
            }
        }
        return false;
    }
}
