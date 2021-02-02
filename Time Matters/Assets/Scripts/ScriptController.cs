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
    [SerializeField] int TimeThreshold;
    [SerializeField] int EventFlag;
    public int numEvent;
    public Node[] children;
    public Node parent;

    public bool isRandom { get => choice.Length == 0 && childIx.Length > 1; } 
    public bool isEnd { get => childIx[0] <= -1;  }
    public bool isHead { get => ParentIx <= -1; } 
    public bool isLoseCo { get => LoseCo > 0; }
    public bool isLoseTime { get => LoseTime > 0; }
    public int timeThreshold { get => TimeThreshold; }
    public int eventFlag { get => EventFlag; }
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
            nodes[i] = JsonUtility.FromJson<Node>(str[i]);
    }

    public Node this[int ix] { get { return nodes[ix]; } }
}

public class ScriptController : MonoBehaviour
{
    bool isTyping = false;          // 지금 스토리가 입력되는중인지
    bool mustChoose = false;        // 선택지가 있는지
    bool isGameOver = false;        // 게임오버?
    bool isGameEnd = false;
    int endFlag = 0;                // 게임 끝?
    int currentScriptIx = 0;        // 현재 스크립트 인덱스
    int rememberIx = -1;            // 돌발이벤트에서 돌아갈곳이 있는지
    int chapterIx = 0;              // 챕터 인덱스
    string currentTypeText = "";
    const int maxChoice = 3;        // 최대 선택지 개수
    const int minPerHour = 60;      // 1시간= 60분
    const int happyEnding = -3;
    const int deathEnding = -4;
    const int fireEnding = -5;

    Graph currentScript;
    List<Graph> chapters = new List<Graph>();
    List<int>[] history;

    InGameManager theInGame;
    AudioManager theAudio;

    IEnumerator typeCoroutine;

    public Text story;
    [SerializeField] Image dialogImage;
    [SerializeField] Image Frame;
    [SerializeField] GameObject[] chooseBttn;
    [SerializeField] float dialogSpeed = 0.2f;

    [SerializeField] GameObject endObject;

    public GameObject gameOverObject;

    readonly string[] chapterPath = new string[] { "Story/prolog", "Story/Chapter1","Story/Chapter2","Story/Chapter3"};


    private void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
        theInGame = FindObjectOfType<InGameManager>();
        foreach (string path in chapterPath)
            chapters.Add(new Graph(path));
        history = new List<int>[chapterPath.Length];
        for (int i = 0; i < history.Length; i++)
            history[i] = new List<int>();
        currentScript = chapters[chapterIx];
    }

    public void goNextDialog()  // 화면터치시 실행되는 함수
    {
        if (isGameOver && !isTyping)
        {
            gameOver();
            return;
        }
        else if (isGameEnd)
        {
            ending();
            return;
        }
        if (!isGameOver && !isTyping)
        {
            isCoOver();
            isTimeOver();
            if (isGameOver)
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
            story.text = currentTypeText;       // 글자 즉시 입력
            isTyping = false;       // 입력 끝남
            updateIx();             // 다음대사 인덱스 찾음
        }
    }

    void nextDialog()
    {
        if (!mustChoose)    // 선택해야되면 선택이나 하셈
        {
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
            currentTypeText = currentScript[currentScriptIx].line;
            typeCoroutine = Typing(story, currentTypeText, dialogSpeed);
            StartCoroutine(typeCoroutine);
        }
    }

    void updateCo()
    {
        GameManager.instance.numCo -= currentScript[currentScriptIx].loseCo;
        if (GameManager.instance.numCo < 0)
            GameManager.instance.numCo = 0;
        theInGame.numCo.text = GameManager.instance.numCo.ToString();
        StartCoroutine("textEffect", theInGame.numCo);
    }

    void updateTime()
    {
        GameManager.instance.remainTime -= currentScript[currentScriptIx].loseTime;
        if (GameManager.instance.remainTime < 0)
            GameManager.instance.remainTime = 0;
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
            theAudio.switchAudio("death");
        }
    }

    void ending()
    {
        Text endText = endObject.transform.Find("Text").GetComponent<Text>();
        Image endImage = endObject.transform.Find("Back").GetComponent<Image>();
        GameObject endBttn = endObject.transform.Find("BackBttn").gameObject;
        endObject.SetActive(true);
        IEnumerator fadeCoroutine = FadeIn(endImage, 1f, 1f, 1f);
        if (endFlag == happyEnding)
        {
            fadeCoroutine = FadeIn(endImage, 1f,1f, 1f);
            typeCoroutine = Typing(endText, "서울은 무사히 지켜내고 당신도 돌아갈 수 있었다...", dialogSpeed, endBttn);
            theAudio.switchAudio("win");
        }
        else if (endFlag == deathEnding)
        {
            fadeCoroutine = FadeIn(endImage, 0.3f, 0f, 0f);
            endText.color = new Color(1f, 1f, 1f);
            typeCoroutine = Typing(endText, "테러리스트의 총격을 맞고 사망했다...", dialogSpeed, endBttn);
            theAudio.switchAudio("death");
        }
        else if (endFlag == fireEnding)
        {
            fadeCoroutine = FadeIn(endImage, 0.7f, 0.7f, 0.7f);
            typeCoroutine = Typing(endText, "당신은 목숨을 건졌지만 서울은 그만 불타고 말았다...", dialogSpeed, endBttn);
            theAudio.switchAudio("win");
        }
        StartCoroutine(fadeCoroutine);
        StartCoroutine(typeCoroutine);
    }

    void isCoOver()   // 대원수 엔딩
    {
        if (chapterIx == 2)
            return;
        if (GameManager.instance.numCo == 0)
        {
            currentTypeText = "마지막 남은 대원이 죽었다...\n\n 어떠한 무기도 가지고 있지 않은 나는 그저 죽음만을 기다릴 뿐이다...";
            typeCoroutine = Typing(story, currentTypeText, dialogSpeed);
            StartCoroutine(typeCoroutine);
            isGameOver = true;
        }
    }

    void isTimeOver()  // 시간 엔딩
    {
        if (chapterIx == 2)
            return;
        if (GameManager.instance.remainTime == 0)
        {
            currentTypeText = "시간은 더 이상 없고 폭발물은 발사되었다...\n\n";
            typeCoroutine = Typing(story, currentTypeText, dialogSpeed);
            StartCoroutine(typeCoroutine);
            isGameOver = true;
        }
    }

    void updateIx()
    {
        if (currentScript[currentScriptIx].childIx[0] == -2)    // 게임 오버?
        {
            gameOver();
            return;
        }
        if (currentScript[currentScriptIx].childIx[0] < -2)     // 게임 엔딩?
        {
            isGameEnd = true;
            endFlag = currentScript[currentScriptIx].childIx[0];
            return;
        }
        if (currentScript[currentScriptIx].eventFlag > 0)
        {
            if((currentScript[currentScriptIx].eventFlag & GameManager.instance.eventFlag) > 0)
                currentScriptIx = currentScript[currentScriptIx].childIx[1];
            else
                currentScriptIx = currentScript[currentScriptIx].childIx[0];
        }
        else if (currentScript[currentScriptIx].timeThreshold > 0)   // 남은 시간에 따라 달라지는가?
        {
            currentScriptIx = currentScript[currentScriptIx].childIx[System.Convert.ToInt32(currentScript[currentScriptIx].timeThreshold > GameManager.instance.remainTime)];
        }
        else if (currentScript[currentScriptIx].isRandom)    // 만약 랜덤이벤트다?
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
                    nextChapter();
                    return;
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
        switch (chapterIx)
        {
            case 2:
                theAudio.switchAudio("2F");
                break;
            case 3:
                theAudio.switchAudio("3F");
                break;
        }
    }

    void showImage()
    {
        dialogImage.gameObject.SetActive(true);
        Frame.gameObject.SetActive(true);
        dialogImage.sprite = Resources.Load<Sprite>(currentScript[currentScriptIx].imagePath);
    }

    void hideImage()
    {
        dialogImage.gameObject.SetActive(false);
        Frame.gameObject.SetActive(false);
    }

    public void revive()
    {
        isGameOver = false;
        if (rememberIx >= 0)
        {
            currentScriptIx = rememberIx;
            rememberIx = -1;
        }
        else
            currentScriptIx++;
        theInGame.updateCo();
        theInGame.updateTime();
        currentTypeText = "모두 끝난줄 알았지만 나는 잠시 기절한 것 뿐이었고 대원들과 시간도 남아있었다.";
        typeCoroutine = Typing(story, currentTypeText, dialogSpeed);
        StartCoroutine(typeCoroutine);
        switch (chapterIx)
        {
            case 1:
                theAudio.switchAudio("1F");
                break;
            case 2:
                theAudio.switchAudio("2F");
                break;
            case 3:
                theAudio.switchAudio("3F");
                break;
        }
    }

    IEnumerator Typing(Text txt, string message, float speed, GameObject bttn = null)
    {
        isTyping = true;
        for (int i = 0; i < message.Length; i++)
        {
            txt.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }
        isTyping = false;
        if (bttn != null)
            bttn.SetActive(true);
        else
        {
            showChooseBttn();
            updateIx();
        }
    }

    IEnumerator textEffect(Text T)
    {
        string str = T.text;
        for (int i = 0; i < 4; i++)     // 4번 반짝거림
        {
            T.text = "<color=#ff0000>" + str + "</color>";
            yield return new WaitForSeconds(0.15f);
            T.text = "<color=#ffffff>" + str + "</color>";
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator FadeIn(Image image, float r, float g, float b)
    {
        for (float i = 0f; i < 1f; i += 0.02f)      // 서서히 나타남
        {
            image.color = new Color(r, g, b, i);
            yield return new WaitForSeconds(0.01f);
        }
    }
}