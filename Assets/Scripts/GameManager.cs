using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    static GameManager gm_Instance;
    public static GameManager Instance
    {
        get
        {
            // if instance is NULL create instance
            if (!gm_Instance)
            {
                gm_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (gm_Instance == null)
                    Debug.Log("instance is NULL_GameManager");
            }
            return gm_Instance;
        }
    }

    public Action SceneChangeAction;

    MapGen _mapGen = new MapGen();
    public static MapGen MapGen { get { return gm_Instance._mapGen; } }

    RandomEncounter _randomEncounter = new RandomEncounter();
    public static RandomEncounter Random { get { return gm_Instance._randomEncounter; } }

    StageController _stageController = new StageController();
    public static StageController StageC { get { return gm_Instance._stageController; } }

    ResourceManager _resourceManager = new ResourceManager();
    public static ResourceManager Resource { get { return gm_Instance._resourceManager; } }

    public struct SerializedGameData
    {
        public List<Resolution> resolutionList;
        // 해상도 목록
        public Resolution resolution;
        // 현재 해상도
        public bool isFullscreen;
        public float gameVolume;
        // 전체 볼륨
        public float musicVolume;
        // 배경음악 볼륨
        public float effectVolume;
        // 효과음 볼륨
    }

    public static SerializedGameData gameData;
    public GameObject persistentObject;
    // == this

    // 시스템 변수

    
    private void Awake()
    {
        DontDestroyOnLoad(persistentObject);
        gm_Instance = this;
        bool isFirstRun = PlayerPrefs.GetInt("isFirstRun", 1) == 1;
        if (isFirstRun)
        {
            // is first
        }
        else
        {
            // else
        }


        //reset game
        loadData();
        Screen.SetResolution(gameData.resolution.width, gameData.resolution.height, gameData.isFullscreen);


        Application.targetFrameRate = 30;
        init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void init()
    {
        Debug.Log("GameManager Awake Init");
        // Spell 객체를 로드하고 만드는 객체 초기화
        Random.init("");
        MapGen.init();
        StageC.init();
    }

    public void loadData()
    {

        gameData.resolution.width = PlayerPrefs.GetInt("resolutionW", Screen.currentResolution.width);
        gameData.resolution.height = PlayerPrefs.GetInt("resolutionH", Screen.currentResolution.height);
        gameData.isFullscreen = PlayerPrefs.GetInt("isFullscreen", Screen.fullScreen ? 1 : 0) == 1;
        // load resolution

        gameData.gameVolume = PlayerPrefs.GetFloat("gameVolume", 0.3f);
        gameData.effectVolume = PlayerPrefs.GetFloat("effectVolume", 0.3f);
        gameData.musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.3f);
        // load volume
    }


   
    public static void GameStart()
    {
        clear();
        StageC.gameStart();
    }

    public static void NextStage()
    {
        clear();
        StageC.nextStage();
    }

    public static void clear()
    {
        StageC.clear();
        MapGen.clear();
    }

    public void SetSeed(string seed)
    {
        Random.setSeed(seed);
    }
}
