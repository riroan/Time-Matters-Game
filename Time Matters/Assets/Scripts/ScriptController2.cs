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
    public int numEvent;
    public Node[] children;
    public Node parent;

    public bool isRandom { get { return choice.Length == 0 && childIx.Length > 1; } }
    public bool isEnd { get { return childIx[0] <= -1; } }
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
    public int randomEventStartIx = -1;
    public int[] randomEventIx;

    public Graph(string path)
    {
        nodes = loadData(path);
        connect();
    }

    public void connect()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].line == "Warning!")
            {
                randomEventStartIx = i;
                randomEventIx = nodes[i].childIx;

            }
            nodes[i].children = new Node[nodes[i].childIx.Length];
            for (int j = 0; j < nodes[i].childIx.Length; j++)
            {
                if (!nodes[i].isEnd)        // 꼬리노드이면 아들을 추가할 필요가 없음
                    nodes[i].children[j] = nodes[nodes[i].childIx[j]];
                if (!nodes[i].isHead)       // 머리노드이면 부모를 추가할 필요가 없음
                    nodes[i].parent = nodes[nodes[i].parentIx];
            }
        }
    }

    Node[] loadData(string path)
    {
        string[] str = File.ReadAllLines(path);
        Node[] item = new Node[str.Length];
        for (int i = 0; i < str.Length; i++)
            item[i] = JsonUtility.FromJson<Node>(str[i]);
        return item;
    }

    public Node this[int ix] { get { return nodes[ix]; } }
}

public class ScriptController2 : MonoBehaviour
{
    bool isTyping = false;
    bool chapterEnd = false;
    bool mustChoose = false;
    int maxChoice = 3;
    int currentScriptIx = 0;
    int rememberIx = -1;

    Graph currentScript;
    Graph graphP;
    Graph chapter1;

    [SerializeField] Image dialogImage;
    [SerializeField] Text story;
    [SerializeField] GameObject[] chooseBttn;
    [SerializeField] float dialogSpeed = 0.1f;
    bool mobile = true;

    private void Start()
    {
        if (mobile)
        {
            graphP = new Graph(Application.persistentDataPath + "/prolog.txt");   // 프롤로그 그래프
            chapter1 = new Graph(Application.persistentDataPath + "/Chapter1.txt"); // 챕터 1
        }
        else
        {
            graphP = new Graph("Assets/Story/prolog.json");   // 프롤로그 그래프
            chapter1 = new Graph("Assets/Story/Chapter1.json"); // 챕터 1
        }
        currentScript = chapter1;
    }

    void nextDialog()
    {
        if (!chapterEnd && !mustChoose)
        {
            if (currentScript[currentScriptIx].isEnd)
                chapterEnd = true;
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
            StartCoroutine(Typing(story, currentScript[currentScriptIx].line, dialogSpeed));
        }
    }

    void showChooseBttn()
    {
        for (int i = 0; i < maxChoice; i++)
        {
            if (i < currentScript[currentScriptIx].numChoice)
            {      // 선택지 활성화
                chooseBttn[i].SetActive(true);
                chooseBttn[i].transform.Find("Text").GetComponent<Text>().text = currentScript[currentScriptIx].choice[i];
                mustChoose = true;
            }
            else   // 선택지 비활성화
                chooseBttn[i].SetActive(false);
        }
    }

    public void choosed(int ix)     // 버튼 클릭한경우 인덱스 받아옴
    {
        mustChoose = false;         // 더이상 안골라도 됨
        currentScriptIx = currentScript[currentScript[currentScriptIx].parentIx].childIx[ix];     // 다음출력할 노드 위치
        showChooseBttn();           // 버튼 없애기 위해 넣었는데 연속으로 선택지가 나오면 오류발생함
        goNextDialog();             // 다음 메시지로 이동
    }

    public void goNextDialog()
    {
        if (!isTyping)          // 글자가 입력되는중이 아닐경우
        {
            nextDialog();       // 다음 대사 불러옴
        }
        else                    // 글자가 입력되는중일경우
        {
            StopAllCoroutines();    // 글자 입력 중단
            showChooseBttn();       // 버튼 보이기
            story.text = currentScript[currentScriptIx].line;       // 글자 즉시 입력
            isTyping = false;       // 입력 끝남
            updateIx();             // 다음대사 인덱스 찾음
        }
    }

    void updateIx()
    {
        if (currentScript[currentScriptIx].isRandom)    // 만약 랜덤이벤트다?
        {
            // 랜덤 결과 불러옴(확률 동일)
            int randomNumber = Random.Range(0, currentScript[currentScriptIx].childIx.Length);
            currentScriptIx = currentScript[currentScriptIx].childIx[randomNumber];
        }
        else
        {
            if (chapterEnd || currentScript[currentScriptIx].isEnd) // 아무튼 끝났다면
            {
                if (rememberIx >= 0)    // 돌아갈곳이 있는가?
                {
                    currentScriptIx = rememberIx;   // 돌아가라
                    rememberIx = -1;                // 더이상 돌아갈 곳은 없다
                    chapterEnd = false;             // 사실 끝난게 아닌거임 ㅋ
                }
                else
                {
                    print("chapter End");           // 이러면 ㄹㅇ 끝난거임
                    // loadNextChapter();
                }
            }
            else if (currentScript[currentScriptIx].numEvent > 0)       // 돌발 이벤트가 있는가?
            {
                //currentScript[currentScriptIx].numEvent--;            // 돌발이벤트 횟수 1회 감소
                rememberIx = currentScript[currentScriptIx].childIx[0]; // 돌아올곳을 기억하자
                currentScriptIx = currentScript.randomEventStartIx;     // 돌발이벤트 장소로 이동
            }
            else           // 평범한경우
                currentScriptIx = currentScript[currentScriptIx].childIx[0];    // 평범하게 이동
        }
    }

    IEnumerator Typing(Text txt, string message, float speed)
    {
        isTyping = true;
        for (int i = 0; i < message.Length; i++)
        {
            txt.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }
        isTyping = false;
        showChooseBttn();
        updateIx();
    }
}
