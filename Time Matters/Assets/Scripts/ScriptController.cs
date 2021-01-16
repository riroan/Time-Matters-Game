using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Dialog
{
    // read-only
    public string Line;         // 현재 대사
    public string ImagePath;    // 그림 경로
    public string[] Choice;     // 선택 리스트
    public string[] Answer1;     // 선택에 따른 답
    public string[] Answer2;
    public string[] Answer3;
    public List<string[]> Answer;

    public Dialog(Dialog right)
    {
        Line = right.line;
        ImagePath = right.imagePath;
        Choice = right.choice;
    }

    void addItem(string[] strArr, List<string[]> listAnswer)
    {
        listAnswer.Add(strArr);
    }

    public string line { get { return Line; } }
    public string imagePath { get { return ImagePath; } }
    public string[] choice { get { return Choice; } }
    public List<string[]> answer { get { return Answer; } }
    public int numChoice { get { return Choice.Length; } }
    public bool hasChoice { get { return Choice.Length > 0; } }
    public bool hasImage {  get { return ImagePath.Length > 0; } }
}

public class ScriptController : MonoBehaviour
{
    int currentScriptIx = 0;                    // 지금 몇번째 글 읽고 있음?
    bool isDialog = false;                      // 지금 글 써지는중임?

    const int maxChoice = 3;                    // 선택지 최대 몇개임?

    Dialog[] prologScript;
    Dialog[] Chapter1;

    [SerializeField] Text story;
    [SerializeField] Image dialogImage;
    [SerializeField] GameObject[] chooseBttn;

    Dialog[] currentScript;                     // 현재 스크립트 대화내용
    [SerializeField] float dialogSpeed = 0.1f;  // 글씨 써지는 속도

    private void Start()
    {
        prologScript = loadDialog("Assets/Story/prolog.json");
        //Chapter1 = loadDialog("Assets/Story/Chapter1.json");

        currentScript = prologScript;
    }

    void saveDialog(Dialog item, string path) // test
    {
        string str = JsonUtility.ToJson(item);
        StreamWriter sw = new StreamWriter(path);
        sw.Write(str);
        sw.Close();
    }

    Dialog[] loadDialog(string path)
    {
        string[] str = File.ReadAllLines(path);
        Dialog[] item = new Dialog[str.Length];
        for (int i = 0; i < str.Length; i++)
            item[i] = new Dialog(JsonUtility.FromJson<Dialog>(str[i]));
        return item;
    }

    void nextDialog()
    {
        if (currentScript.Length > currentScriptIx)
        {
            // 해당 라인에 이미지가 있으면 이미지 불러옴
            if (currentScript[currentScriptIx].hasImage)
            {
                dialogImage.gameObject.SetActive(true);
                dialogImage.sprite = Resources.Load<Sprite>(currentScript[currentScriptIx].imagePath);
            }
            else
            {
                dialogImage.gameObject.SetActive(false);
            }

            if (currentScript[currentScriptIx].hasChoice)
            {
                for (int i = 0; i < maxChoice; i++)
                {
                    if (i < currentScript[currentScriptIx].numChoice)
                    {      // 선택지 활성화
                        chooseBttn[i].SetActive(true);
                        chooseBttn[i].transform.Find("Text").GetComponent<Text>().text = currentScript[currentScriptIx].choice[i];
                    }
                    else   // 선택지 비활성화
                        chooseBttn[i].SetActive(false);
                }
            }
            StartCoroutine(Typing(story, currentScript[currentScriptIx].line, dialogSpeed));
        }
    }

    public void goNextDialog()
    {
        if (!isDialog)
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
