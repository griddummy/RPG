using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// 게임의 스테이지를 관리하고, 초기화하고, 게임 성공 or 실패 여부를 결정하는 class
public class GameManager : MonoBehaviour {

    public enum FADE { IN, OUT }
    public const string strLoadingSceneName = "SceneStageLoading";
    public GameObject prefabUIBar;
    public GameObject prefabSceneFade;
    public GameObject prefabUISaveLoad;
    public List<Stage> listStage;
    public List<Job> listJob;
    
    private Stage curStage; // 현재 Stage
    private Job.JOB jobNewSelected = Job.JOB.WARRIOR;// 선택한 직업
    private GameObject objPlayer;   // 생성한 플레이어 오브젝트
    private Player player;          // 플레이어 클래스
    private GameObject objUIBar;    // 생성한 Bar UI;
    private float fadeTime = 0.5f;
    private List<SaveInfo> listSaveInfo;
    private const string SAVE_PATH = "/Save/";
    private const string SAVE_EXTENTION = ".sav";
    private GameObject objUISaveLoad;
    private PlayerInventory playerInven;
    private ItemDataBaseList listItemDataBase;
    private static GameManager m_instance;
    public static GameManager instance
    {
        //싱글톤, 오직 한개의 인스턴스(생성)만 가지도록 한다.
        get
        {
            if (m_instance == null)
            {
                // 이미 존재하는 GameManager를 찾는다.
                m_instance = FindObjectOfType<GameManager>();
                if(m_instance == null)
                {
                    // 없으면 새 오브젝트에 추가한다.
                    GameObject newObject = new GameObject("Logic");         
                    GameObject objLogic = Instantiate(newObject);
                    m_instance = objLogic.AddComponent<GameManager>();
                }
            }
            return m_instance;
        }
    }

    void Awake()
    {
        
        if (FindObjectsOfType<GameManager>().Length >= 2)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject); // 삭제하지 않음
    }

    void Start()
    {
        curStage = listStage[0];
        listItemDataBase = (ItemDataBaseList)Resources.Load("ItemDatabase");
    }

    public Stage getCurrentStageInfo()
    {
        return curStage;
    }
    public void SelectJob(Job.JOB job)
    {
        jobNewSelected = job;
    }
    public void CreateNewPlayer()
    {
        GameObject prefab = getCurrentJobInfo().jopPrefab;
        objPlayer = Instantiate(prefab);
        player = objPlayer.GetComponent<Player>();
        player.OnDeath += EndGame;
        DontDestroyOnLoad(objPlayer);
    }
    public void MoveStage(int indexStage, Vector3 vecStartPos)
    {
        curStage = listStage[indexStage];
        StartCoroutine(LoadStage(curStage, vecStartPos));
    }
    public Job getJobInfo(Job.JOB job)
    {
        return listJob[(int)job];
    }
    public Job getCurrentJobInfo() // 직업에 해당하는 정보를 반환
    {
        return listJob[(int)jobNewSelected];
    }
    public GameObject getPlayerObject()
    {
        return objPlayer;
    }
    public Player getPlayerInfo()
    {
        return player;
    }
    public PlayerInventory getPlayerInventory()
    {
        return playerInven;
    }
    public void StartNewGame(int indexStage) // 새 게임 시작
    {
        StartNewGame(0, null);
    }
    
    public void StartNewGame(int indexStage, SaveInfo info)
    {
        // 기존 플레이어 삭제
        if (objPlayer != null)
        {
            Destroy(objPlayer);
            objPlayer = null;
        }
        // 새 플레이어 오브젝트 생성
        CreateNewPlayer();
        

        // 새 UI Bar 생성
        CreateNewUIPlayer();
        
        
        
        if (info != null) // 세이브 정보가 있으면
        {
            // 플레이어 정보 로드
            player.level = info.level;
            player.sp = info.curSP;
            player.hp = info.curHP;
            player.exp = info.curExp;

            // 아이템 로드            
            List<Item> listItem = new List<Item>();
            foreach (SaveItemInfo sItemInfo in info.listInventoryItem)
            {
                listItem.Add(sItemInfo.getItem(listItemDataBase));                
            }            
            playerInven.InitInventoryItems(listItem);
            listItem.Clear();
            foreach (SaveItemInfo sItemInfo in info.listHotBarItem)
            {
                listItem.Add(sItemInfo.getItem(listItemDataBase));
                
            }
            playerInven.InitHotbarItems(listItem);
            listItem.Clear();
            foreach (SaveItemInfo sItemInfo in info.listStorageItem)
            {
                listItem.Add(sItemInfo.getItem(listItemDataBase));
            }

            playerInven.InitStorageItems(listItem);
            listItem.Clear();
            foreach (SaveItemInfo sItemInfo in info.listCharacterSystemItem)
            {
                listItem.Add(sItemInfo.getItem(listItemDataBase));
            }
            playerInven.InitCharSystemItems(listItem);
            MoveStage(indexStage, new Vector3(info.posX, info.posY, info.posZ));
        }
        else
            MoveStage(indexStage, listStage[indexStage].transDefaultStartingPos.position);
    }
    private void CreateNewUIPlayer()
    {
        if (objPlayer == null)
            return;

        if(objUIBar != null)
        {
            Destroy(objUIBar);
        }

        objUIBar = Instantiate(prefabUIBar);
        objUIBar.SetActive(false);
        DontDestroyOnLoad(objUIBar);
        UIBar bar = objUIBar.GetComponent<UIBar>();
        Player player = objPlayer.GetComponent<Player>();
        player.OnChangedLevel += bar.setLevel;
        player.OnChangedExp += bar.ExpBar.SetupValue;
        player.OnChangedHP += bar.HpBar.SetupValue;
        player.OnChangedSP += bar.SpBar.SetupValue;

        bar.setLevel(player.level);
        bar.HpBar.SetupValue(player.hp, player.hpMax);
        bar.SpBar.SetupValue(player.sp, player.spMax);
        bar.ExpBar.SetupValue(player.exp, player.expRequired);
        playerInven = objUIBar.GetComponent<PlayerInventory>();
    }
    
    public void EndGame()
    {
        // 플레이어가 죽으면 실행

        // 메인메뉴로 5초 후 이동
        StartCoroutine(GoMainMenu());

    }
    IEnumerator GoMainMenu()
    {
        yield return new WaitForSeconds(5f);
        MoveMainMenu();
    }
    public void MoveMainMenu()
    {        
        if(objPlayer != null)
            Destroy(objPlayer);
        if(objUIBar != null)
            Destroy(objUIBar);
        SceneManager.LoadScene("SceneMain");
        
    }
    public void ExitGame() // 게임 종료
    {
        Application.Quit(); 
    }
    IEnumerator LoadStage(Stage stage, Vector3 vecPlayerPos)
    {
        Time.timeScale = 0f;
        objUIBar.SetActive(false);
        GameObject objFadeCanvas = Instantiate(prefabSceneFade);
        DontDestroyOnLoad(objFadeCanvas);
        Image img = objFadeCanvas.GetComponentInChildren<Image>();

        yield return FadeEffect(FADE.OUT, img);

        AsyncOperation ao = SceneManager.LoadSceneAsync(stage.sceneName);
        while (!ao.isDone)
        { 
            yield return null;
        }
        objPlayer.transform.position = vecPlayerPos;        
        yield return FadeEffect(FADE.IN, img);
        UIBar bar = objUIBar.GetComponent<UIBar>();
        bar.setTextStage(stage.stageName);
        objUIBar.SetActive(true); 
               
        Destroy(objFadeCanvas);
        Time.timeScale = 1f;
    }
    IEnumerator FadeEffect(FADE mode, Image img)
    {
        Color colorSource;      // 초기 색상
        Color colorDestination; // 끝 색상

        if(mode == FADE.IN) // 점점 맑아지는 효과
        {
            colorSource = Color.black;
            colorDestination = Color.clear; 
        }
        else // 점점 어두워지는 효과
        {
            colorSource = Color.clear;
            colorDestination = Color.black;            
        }        

        float startTime = Time.realtimeSinceStartup;    // 시작시간
        float endTime = startTime + fadeTime;           // 끝 시간
       
        while (endTime > Time.realtimeSinceStartup) 
        {
            img.color = Color.Lerp(colorSource, colorDestination, (Time.realtimeSinceStartup - startTime) / fadeTime);
            yield return null;
        }
    }
    public SaveInfo getSaveInfo() // 현재 플레이어의 정보를  저장정보로 얻기
    {
        if (objPlayer == null)
            return null;

        SaveInfo info = new SaveInfo();
        Player playerInfo = getPlayerInfo();

        System.DateTime time = System.DateTime.Now;
        string strDate = time.Year.ToString() + "년 " + time.Month.ToString() + "월 " + time.Day.ToString() + "일";
        string strTime = time.Hour.ToString() + "시 " + time.Minute.ToString() + "분 " + time.Second.ToString() + "초";
        string saveName = strDate + " " + strTime;

        info.setInfo(saveName, playerInfo.hp, playerInfo.sp, playerInfo.exp, playerInfo.level, jobNewSelected,
            getCurrentStageInfo().stageName, getCurrentStageInfo().sceneName, playerInfo.transform.position, strDate, strTime,
             playerInven.GetInventoryItems(), playerInven.GetStorageInventoryItems(), playerInven.GetHotBarItems(), playerInven.GetCharacterSystemInventoryItems());

        return info;
    }
    public List<SaveInfo> getAllSaveInfo() // 플레이어 정보 리스트 불러오기
    {
        listSaveInfo = new List<SaveInfo>();

        DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath + SAVE_PATH);
        if (d == null)
            return listSaveInfo;
        FileInfo[] fileInfos = d.GetFiles();

        foreach(FileInfo file in fileInfos)
        {
            if (file.Extension.CompareTo(SAVE_EXTENTION) == 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = file.Open(FileMode.Open);
                SaveInfo newInfo = (SaveInfo)bf.Deserialize(fs);
                fs.Close();
                listSaveInfo.Add(newInfo);
            }
        }  
        return listSaveInfo;
    }

    public void RemoveSaveInfo(SaveInfo info) // 저장 정보 제거
    {
        int index = listSaveInfo.IndexOf(info);
        if (index != -1)
        {
            listSaveInfo.RemoveAt(index);            
            File.Delete(Application.persistentDataPath + SAVE_PATH + info.saveName + SAVE_EXTENTION);
        }
    }

    public bool SaveGame(SaveInfo info) // 저장 정보 저장
    {
        // 디렉토리 검사
        if (!Directory.Exists(Application.persistentDataPath + SAVE_PATH))
        {
            Directory.CreateDirectory(Application.persistentDataPath + SAVE_PATH);
        }
        BinaryFormatter bf = new BinaryFormatter();        
        FileStream file = File.Create(Application.persistentDataPath + SAVE_PATH + info.saveName + SAVE_EXTENTION);
        bf.Serialize(file, info);
        file.Close();
        return true;
    }

    public bool LoadGame(SaveInfo info) // 불러오기,  게임 다시 시작
    {
        Stage stage;
        int index;
        if(!getStage(info.stageSceneName, out stage, out index))
        {
            return false;
        }
        StartNewGame(index, info);
        return true;
    }
    public bool isOpenUISaveLoad()
    {
        if (objUISaveLoad == null)
            return false;
        return objUISaveLoad.activeInHierarchy;
    }
    public void OpenUISaveLoad(UISaveLoadInfo.MODE mode, UISaveLoadInfo.OnClosedEvent addClosedEvent)
    {
        
        if (objUISaveLoad == null)
        {
            objUISaveLoad = Instantiate(prefabUISaveLoad);

        }
        objUISaveLoad.GetComponent<UISaveLoadInfo>().Open(mode, addClosedEvent);
    }

    public void CloseUISaveLoad()
    {
        if (objUISaveLoad == null)
            return;
        objUISaveLoad.GetComponent<UISaveLoadInfo>().Close();
    }

    public bool getStage(string sceneName, out Stage outStage, out int index)
    {
        int count = 0;
        foreach(Stage stage in listStage)
        {
            if(stage.sceneName.CompareTo(sceneName) == 0)
            {
                outStage = stage;
                index = count;
                return true;
            }
            count++;
        }
        outStage = new Stage();
        index = -1;
        return false;
    }
    void Update()
    {
        //Test
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            playerInven.inventory.GetComponent<Inventory>().setRandomItemTest();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(SceneManager.GetActiveScene().name != "SceneMain")
            {
                if (curStage.type == Stage.TYPE.NORMAL)
                {
                    if (isOpenUISaveLoad())
                    {
                        CloseUISaveLoad();
                    }
                    else
                    {
                        OpenUISaveLoad(UISaveLoadInfo.MODE.SAVELOAD, null);
                    }
                }                
            }
        }
    }
    public Item GetRandomItem()
    {
        ItemDataBaseList itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase");
        int randomItemNumber = Random.Range(0, itemDatabase.itemList.Count - 1);

        Item item = itemDatabase.itemList[randomItemNumber].getCopy();

        int randomValue = Random.Range(1, itemDatabase.itemList[randomItemNumber].getCopy().maxStack);
        item.itemValue = randomValue;
        return item;
    }
    
}