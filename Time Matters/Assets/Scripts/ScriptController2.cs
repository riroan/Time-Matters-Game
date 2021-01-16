using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Node 
{
    [SerializeField] int Ix;
    [SerializeField] string Line;
    [SerializeField] string ImagePath;
    [SerializeField] string[] Choice;
    [SerializeField] int ParentIx;
    [SerializeField] int[] ChildIx;
    public Node[] children;
    public Node parent;

    public bool isEnd { get { return childIx[0] == -1; } }
    public bool isHead { get { return ParentIx == -1; } }
    public int ix { get { return Ix; } }
    public string line { get { return Line; } }
    public string imagePath { get { return ImagePath; } }
    public string[] choice { get { return Choice; } }
    public int parentIx { get { return ParentIx; } }
    public int[] childIx { get { return ChildIx; } }
    public Node this[int ix] { get { return children[ix]; } }
    public int numChoice { get { return choice.Length; } }
    public bool hasChoice { get { return numChoice > 0; } }
    public bool hasImage { get { return imagePath.Length > 0; } }
}

public class Graph
{
    public Node[] nodes;

    public void connect()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].children = new Node[nodes[i].childIx.Length];
            for (int j = 0; j < nodes[i].childIx.Length; j++)
            {
                if (!nodes[i].isEnd)
                    nodes[i].children[j] = nodes[nodes[i].childIx[j]];
                if (!nodes[i].isHead)
                    nodes[i].parent = nodes[nodes[i].parentIx];
            }
        }
    }

    public Node this[int ix] { get { return nodes[ix]; } }
}

public class ScriptController2 : MonoBehaviour
{
    bool isDialog = false;
    int maxChoice = 3;
    int currentScriptIx = -1;
    Graph currentScript;
    Graph graphP;
    Graph chapter1;

    [SerializeField] Image dialogImage;
    [SerializeField] Text story;
    [SerializeField] GameObject[] chooseBttn;
    [SerializeField] float dialogSpeed = 0.1f;

    private void Start()
    {
        graphP = new Graph();   // 프롤로그 그래프
        chapter1 = new Graph(); // 챕터 1
        
        graphP.nodes = loadData("Assets/Story/Prolog/prolog.json");
        chapter1.nodes = loadData("Assets/Story/Chapter1/Chapter1.json");
        graphP.connect();
        chapter1.connect();
        currentScript = chapter1;
    }

    Node[] loadData(string path)
    {
        string[] str = File.ReadAllLines(path);
        Node[] item = new Node[str.Length];
        for (int i = 0; i < str.Length; i++)
        {
            item[i] = JsonUtility.FromJson<Node>(str[i]);
        }
        return item;
    }

    void nextDialog()
    {
        if (!currentScript[currentScriptIx].isEnd)
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
