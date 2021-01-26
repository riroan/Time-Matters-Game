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
    public bool isHead { get { return ParentIx <= -1; } }
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
        loadData(path);
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

    void loadData(string path)
    {
        TextAsset txt = (TextAsset)Resources.Load(path);
        string[] str = txt.text.Split('\n');
        nodes = new Node[str.Length];
        for (int i = 0; i < str.Length; i++)
            nodes[i] = JsonUtility.FromJson<Node>(str[i]);
    }

    public Node this[int ix] { get { return nodes[ix]; } }
}

public class ScriptController : MonoBehaviour
{
    bool isTyping = false;          // 지금 스토리가 입력되는중인지
    bool mustChoose = false;        // 선택지가 있는지
    bool isGameOver = false;        // 게임오버?
    bool isGameEnd = false;         // 게임 끝?
    int currentScriptIx = 0;        // 현재 스크립트 인덱스
    int rememberIx = -1;            // 돌발이벤트에서 돌아갈곳이 있는지
    int chapterIx = 0;              // 챕터 인덱스
    const int maxChoice = 3;        // 최대 선택지 개수

    Graph currentScript;
    List<Graph> chapters = new List<Graph>();
    List<int>[] history;

    [SerializeField] Image dialogImage;
    [SerializeField] Image Frame;
    [SerializeField] Text story;
    [SerializeField] GameObject[] chooseBttn;
    [SerializeField] GameObject backBttn;
    [SerializeField] float dialogSpeed = 0.2f;

    readonly string[] chapterPath = new string[] { "Story/prolog", "Story/Chapter1","Story/Chapter2"};

    private void Start()
    {
        foreach (string path in chapterPath)
            chapters.Add(new Graph(path));
        history = new List<int>[chapterPath.Length];
        for (int i = 0; i < history.Length; i++)
            history[i] = new List<int>();
        currentScript = chapters[chapterIx];
    }

    public void goNextDialog()  // 화면터치시 실행되는 함수
    {
        if (isGameOver)
        {
            gameOver();
            return;
        }
        else if (isGameEnd)
        {
            ending();
            return;
        }
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

    void nextDialog()
    {
        if (!mustChoose)    // 선택해야되면 선택이나 하셈
        {
            history[chapterIx].Add(currentScriptIx);    // 기록 저장
            // 해당 라인에 이미지가 있으면 이미지 불러옴
            if (currentScript[currentScriptIx].hasImage)
                showImage();
            else
                hideImage();
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

    void gameOver()
    {
        if (!isGameOver)
            isGameOver = true;
        else
        {
            story.text = "Game Over...";
            backBttn.SetActive(true);
        }
    }

    void ending()
    {
        if (!isGameEnd)
            isGameEnd = true;
        else
        {
            story.text = "도시를 지켜냈다.\n(대충 이겼다는 뜻)";
            backBttn.SetActive(true);
        }
    }

    void updateIx()
    {
        if (currentScript[currentScriptIx].childIx[0] == -2)
        {
            gameOver();
            return;
        }
        if (currentScript[currentScriptIx].isRandom)    // 만약 랜덤이벤트다?
        {
            // 랜덤 결과 불러옴(확률 동일)
            int randomNumber = Random.Range(0, currentScript[currentScriptIx].childIx.Length);
            currentScriptIx = currentScript[currentScriptIx].childIx[randomNumber];
        }
        else
        {
            if (currentScript[currentScriptIx].isEnd) // 아무튼 끝났다면
            {
                if (rememberIx >= 0)    // 돌아갈곳이 있는가?
                {
                    currentScriptIx = rememberIx;   // 돌아가라
                    rememberIx = -1;                // 더이상 돌아갈 곳은 없다
                }
                else
                {
                    try
                    {
                        currentScript = chapters[++chapterIx];      // 이러면 ㄹㅇ 끝난거임, 다음 챕터불러오셈
                        currentScriptIx = 0;
                        return;
                    }
                    catch 
                    {
                        ending();
                    }
                }
            }
            if (currentScript[currentScriptIx].numEvent > 0)            // 돌발 이벤트가 있는가?
            {
                currentScript[currentScriptIx].numEvent--;              // 돌발이벤트 횟수 1회 감소
                rememberIx = currentScript[currentScriptIx].ix;         // 돌아올곳을 기억하자
                int randomNumber = Random.Range(0, currentScript[currentScript.randomEventStartIx].childIx.Length);
                currentScriptIx = currentScript[currentScript.randomEventStartIx].childIx[randomNumber];     // 돌발이벤트 장소로 이동
            }
            else           // 평범한경우
                currentScriptIx = currentScript[currentScriptIx].childIx[0];    // 평범하게 이동
        }
    }

    void showImage()
    {
        //story.transform.position = new Vector3(0, -1.2f);
        dialogImage.gameObject.SetActive(true);
        Frame.gameObject.SetActive(true);
        dialogImage.sprite = Resources.Load<Sprite>(currentScript[currentScriptIx].imagePath);
    }

    void hideImage()
    {
        //story.transform.position = new Vector3(0, 0.8f);
        dialogImage.gameObject.SetActive(false);
        Frame.gameObject.SetActive(false);
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