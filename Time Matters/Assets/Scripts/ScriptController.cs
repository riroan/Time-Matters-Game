using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialog
{
    public string line;         // 현재 대사
    public string imagePath;    // 그림 경로
    public string[] choice;     // 선택 리스트
}

public class ScriptController : MonoBehaviour
{
    int currentScriptIx = 0;                    // 지금 몇번째 글 읽고 있음?
    bool isDialog = false;                      // 지금 글 써지는중임?

    const int maxChoice = 3;                    // 선택지 최대 몇개임?

    Dialog[] prologScript;                      // 스크립트 클래스
    [SerializeField] Text story;
    [SerializeField] Image dialogImage;
    [SerializeField] GameObject[] chooseBttn;

    Dialog[] currentScript;                     // 현재 스크립트 대화내용
    [SerializeField] float dialogSpeed = 0.1f;  // 글씨 써지는 속도

    private void Start()
    {
        prologScript = loadDialog<Dialog>("Assets/Story/prolog.json");
        currentScript = prologScript;
    }

    void saveDialog<T>(T item, string path) // test
    {
        string str = JsonUtility.ToJson(item);
        StreamWriter sw = new StreamWriter(path);
        sw.Write(str);
        sw.Close();
    }

    T[] loadDialog<T>(string path)
    {
        string[] str = File.ReadAllLines(path);
        T[] item = new T[str.Length];
        for (int i = 0; i < str.Length; i++)
            item[i] = JsonUtility.FromJson<T>(str[i]);
        return item;
    }

    void nextDialog()
    {
        if (currentScript.Length > currentScriptIx)
        {
            // 해당 라인에 이미지가 있으면 이미지 불러옴
            if (currentScript[currentScriptIx].imagePath.Length > 0)
            {
                dialogImage.gameObject.SetActive(true);
                dialogImage.sprite = Resources.Load<Sprite>(currentScript[currentScriptIx].imagePath);
            }
            else
                dialogImage.gameObject.SetActive(false);

            for (int i = 0; i < maxChoice; i++)
            {
                if (i < currentScript[currentScriptIx].choice.Length)
                {      // 선택지 활성화
                    chooseBttn[i].SetActive(true);
                    chooseBttn[i].transform.Find("Text").GetComponent<Text>().text = currentScript[currentScriptIx].choice[i];
                }
                else   // 선택지 비활성화
                    chooseBttn[i].SetActive(false);
            }
            StartCoroutine(Typing(story, currentScript[currentScriptIx].line, dialogSpeed));
        }
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
            story.text = currentScript[currentScriptIx++].line;
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
