using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string playerName;

    private void Awake()
    {
        if (instance) Destroy(gameObject);
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // 저장한 데이터 불러오는 함수
    void load_data()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}