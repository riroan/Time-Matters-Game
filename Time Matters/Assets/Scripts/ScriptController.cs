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
    [SerializeField] int Flag;
    [SerializeField] int LoseCo;
    [SerializeField] int LoseTime;
    public int numEvent;
    public Node[] children;
    public Node parent;

    public bool isRandom { get => choice.Length == 0 && childIx.Length > 1; } 
    public bool isEnd { get => childIx[0] <= -1;  }
    public bool isHead { get => ParentIx <= -1; } 
    public bool isLoseCo { get => LoseCo > 0; }
    public bool isLoseTime { get => LoseTime > 0; }
    public int flag { get => Flag; }
    public int ix { get => Ix;  }
    public int loseCo { get => LoseCo; }
    public int loseTime { get => LoseTime; }
    public string imagePath { get => ImagePath; }
    public string line { get => Line.Replace(' ', '\u00A0').Replace("OO",GameManager.instance.playerName); }
    public string[] choice { get => Choice; } 
    public int parentIx { get => ParentIx; }
    public int[] childIx { get => ChildIx; } 
    public Node this[int ix] { get => children[ix]; }
    public int numChoice { get => choice.Length; } 
    public bool hasChoice { get => numChoice > 0; }
    public bool hasImage { get => imagePath.Length > 0; } 
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
        {
            nodes[i] = JsonUtility.FromJson<Node>(str[i]);
        }
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
    const int minPerHour = 60;      // 1시간= 60분

    Graph currentScript;
    List<Graph> chapters = new List<Graph>();
    List<int>[] history;

    InGameManager theInGame;

    IEnumerator typeCoroutine;

    [SerializeField] Image dialogImage;
    [SerializeField] Image Frame;
    [SerializeField] Text story;
    [SerializeField] GameObject[] chooseBttn;
    [SerializeField] float dialogSpeed = 0.2f;

    public GameObject gameOverObject;

    readonly string[] chapterPath = new string[] { "Story/prolog", "Story/Chapter1","Story/Chapter2"};

    public void TTT()
    {
        StartCoroutine("eee");
    }

    IEnumerator textEffect(Text T)
    {
        string str = T.text;
        for (int i = 0; i < 4; i++)
        {
            T.text = "<color=#ff0000>" + str +"</color>";
            yield return new WaitForSeconds(0.15f);
            T.text = "<color=#ffffff>" + str + "</color>";
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void Start()
    {
        theInGame = FindObjectOfType<InGameManager>();
        foreach (string path in chapterPath)
        {
            print("path");
            chapters.Add(new Graph(path));
        }
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
            StopCoroutine(typeCoroutine);
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
            print(GameManager.instance.eventFlag);
            history[chapterIx].Add(currentScriptIx);    // 기록 저장
            GameManager.instance.eventFlag |= currentScript[currentScriptIx].flag;   // 이벤트가 있는지 확인하는 플래그
            if (currentScript[currentScriptIx].isLoseCo)            // 대원을 잃는가?
                updateCo();
            if (currentScript[currentScriptIx].isLoseTime)
                updateTime();
            // 해당 라인에 이미지가 있으면 이미지 불러옴
            if (currentScript[currentScriptIx].hasImage)
                showImage();
            else
                hideImage();
            typeCoroutine = Typing(story, currentScript[currentScriptIx].line, dialogSpeed);
            StartCoroutine(typeCoroutine);
        }
    }

    void updateCo()
    {
        GameManager.instance.numCo -= currentScript[currentScriptIx].loseCo;
        theInGame.numCo.text = GameManager.instance.numCo.ToString();
        StartCoroutine("textEffect", theInGame.numCo);
    }

    void updateTime()
    {
        GameManager.instance.remainTime -= currentScript[currentScriptIx].loseTime;
        theInGame.hour.text = (GameManager.instance.remainTime / minPerHour).ToString();
        theInGame.minute.text = (GameManager.instance.remainTime % minPerHour).ToString();
        StartCoroutine("textEffect", theInGame.hour);
        StartCoroutine("textEffect", theInGame.minute);
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
            gameOverObject.SetActive(true);
            if (!GameManager.instance.canViewAds)
                gameOverObject.transform.Find("Ads").gameObject.SetActive(false);
        }
    }

    void ending()
    {
        if (!isGameEnd)
            isGameEnd = true;
        else
        {
            story.text = "도시를 지켜냈다.\n(대충 이겼다는 뜻)";
            //gameO.SetActive(true);
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
                        nextChapter();
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

    void nextChapter()
    {
        currentScript = chapters[++chapterIx];      // 이러면 ㄹㅇ 끝난거임, 다음 챕터불러오셈
        if (chapterIx == 2 && !GameManager.instance.isFixedElevator())  // 엘리베이터 안고친경우
            currentScriptIx = 11;
        else
            currentScriptIx = 0;
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