using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialog
{
    [TextArea]
    public string line;         // 현재 대사
    public Sprite cg;           // 지금 대사에 그림이 있는지
    public bool isChoose;       // 지금 대사가 선택해야하는 대사인지
}

public class ScriptController : MonoBehaviour
{
    int currentScriptIx = 0;
    Dialog[] scripts;
    [SerializeField] Text story;
    [SerializeField] Image dialogImage;

    string[] script;
    public bool isDialog = false;

    private void Start()
    {
        // script load
        script = File.ReadAllLines("Assets/Story/prologue.txt");
        scripts = new Dialog[script.Length];
        //showDialog();
    }

    void nextDialog()
    {
        if (script.Length > currentScriptIx)
        {
            //story.text = script[currentScriptIx++];
            StartCoroutine(Typing(story, script[currentScriptIx], 0.1f));
        }
    }

    public void showDialog()
    {
        dialogImage.gameObject.SetActive(true);
    }


    private void OnMouseUp()
    {
        if (gameObject.name == "board" && !isDialog)
        {
            nextDialog();
        }
        else if (isDialog)
        {
            StopAllCoroutines();
            story.text = script[currentScriptIx++];
            isDialog = false;
        }
    }

    IEnumerator Typing(Text txt, string message, float speed)
    {
        isDialog = true;
        for (int i = 0; i < message.Length; i++)
        {
            txt.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }
        isDialog = false;
        currentScriptIx++;
    }
}
