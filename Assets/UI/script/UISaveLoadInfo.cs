using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UISaveLoadInfo : MonoBehaviour {

    public enum MODE { LOAD, SAVELOAD } // 저장, 저장/불러오기 모드
    public delegate void OnClosedEvent(); // 닫기 이벤트
    public event OnClosedEvent OnClosed;

    public GameObject objScrollParent;     // 스크롤뷰 내용들의 부모
    public GameObject objNode;
    public Text txtCurrentPlayerInfo;   // 현재 플레이어 정보
    public Text txtSelectSaveInfo;            // 클릭한 저장 정보
    public Button btnNewSave;           // 새로저장 버튼
    public Button btnLoad;              // 로드 버튼
    public Button btnRemove;            // 삭제 버튼
    public GameObject objPopupNewSave;         // 새 저장 팝업
    public InputField inputNewSaveName;              // 새저장 이름 텍스트
    private SaveInfo curSaveInfo;           // 현재 정보
    private SaveInfo curSelectSaveInfo; // 선택한 저장 정보
    private List<GameObject> listBtnSaveInfo = new List<GameObject>();      // 저장 정보 버튼들
    private List<SaveInfo> listSaveInfo = new List<SaveInfo>();             // 저장 정보 들
    private MODE mode;
    private List<OnClosedEvent> listCloseEvent = new List<OnClosedEvent>();
    private float preTimeScale;
    private void Init()
    {
        // UI 초기화
        txtCurrentPlayerInfo.text = "";
        txtSelectSaveInfo.text = "";
        btnLoad.gameObject.SetActive(false);
        btnRemove.gameObject.SetActive(false);
        PopupClose();

        

        setCurrentPlayinfo(); // 현재 플레이어 정보 표시

        InitScrollViewItem();

    }
    private void InitScrollViewItem()
    {
        // 스크롤 초기화
        foreach (GameObject objBtn in listBtnSaveInfo)
        {
            Destroy(objBtn);
        }

        listBtnSaveInfo.Clear();
        listSaveInfo.Clear();
        // 모든 저장정보 불러오기
        listSaveInfo = GameManager.instance.getAllSaveInfo();

        // 저장정보 갯수만큼 스크롤뷰에 버튼 채우기
        foreach (SaveInfo info in listSaveInfo)
        {
            GameObject objNewNode = Instantiate(objNode);
            objNewNode.transform.SetParent(objScrollParent.transform);
            objNewNode.transform.localScale = Vector3.one;
            UISaveInfoNode uiNode = objNewNode.GetComponent<UISaveInfoNode>();
            uiNode.setSaveInfo(info);
            uiNode.OnClick += OnClickSaveInfoNode;
            listBtnSaveInfo.Add(objNewNode);
        }
        
    }
    private void setCurrentPlayinfo()
    {
        if (mode == MODE.SAVELOAD)
        {
            // 현재 정보 얻기
            curSaveInfo = GameManager.instance.getSaveInfo();

            // 텍스트 정보로 가공하기
            txtCurrentPlayerInfo.text =
                "Lv." + curSaveInfo.level + " " +
                GameManager.instance.getJobInfo(curSaveInfo.job).jobName + "\n" +                
                curSaveInfo.stageName + "\n" +
                curSaveInfo.date + " " +
                curSaveInfo.time;
        }
    }
    public void Open(MODE mode, OnClosedEvent closedEvent)
    {
        preTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        this.mode = mode;
        Init();
        if (mode == MODE.SAVELOAD) // 저장/불러오기 동시
        {
            btnNewSave.gameObject.SetActive(true); // 새 저장 버튼 보이기
        }
        else // 불러오기만
        {
            btnNewSave.gameObject.SetActive(false); // 새 저장 버튼 감추기
        }
        if (closedEvent != null)
        {
            listCloseEvent.Add(closedEvent);
            OnClosed += closedEvent;
        }            
        gameObject.SetActive(true);  
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if(OnClosed != null)
        {
            OnClosed();
            foreach (OnClosedEvent addedEvent in listCloseEvent)
            {
                OnClosed -= addedEvent;
            }
        }
        Time.timeScale = preTimeScale; 
    }

    public void OnClickNewSave() // 새로 저장 클릭
    {
        PopupOpen();
    }

    public void OnClickRemoveSaveInfo() // 삭제 클릭
    {
        if (curSelectSaveInfo == null)
            return;

        int index = -1;

        for(int i = 0; i < listSaveInfo.Count; i++)
        {
            if (listSaveInfo[i].saveName == curSelectSaveInfo.saveName)
            {
                index = i;
                break;
            }
            
        }
        if (index >= 0 && index < listSaveInfo.Count)
        {
            Destroy(listBtnSaveInfo[index]);
            listBtnSaveInfo.RemoveAt(index);
        }

        GameManager.instance.RemoveSaveInfo(curSelectSaveInfo);
        
        txtSelectSaveInfo.text = "";
        curSelectSaveInfo = null;
    }

    public void OnClickLoadSaveInfo() // 불러오기 클릭
    {
        if (curSelectSaveInfo == null)
            return;
        Close();
        GameManager.instance.LoadGame(curSelectSaveInfo);        
    }

    public void OnClickSaveInfoNode(GameObject obj, SaveInfo info) // 저장 정보 클릭 시
    {
        // 정보 패널 표시
        setTextSaveInfoInSelectInfoPanel(info);
        // 불러오기 버튼 , 삭제 버튼 활성화
        btnRemove.gameObject.SetActive(true);
        btnLoad.gameObject.SetActive(true);
        curSelectSaveInfo = info;       
    }

    public void OnClickPopupOK() // 팝업 - 저장 
    {        
        string saveName = inputNewSaveName.text;
        if(saveName.Length == 0)
        {
            return;
        }
        curSaveInfo.saveName = saveName;
        if (GameManager.instance.SaveGame(curSaveInfo))
        {
            InitScrollViewItem();
        }
        PopupClose();
    }

    public void PopupOpen()
    {
        inputNewSaveName.text = "";
        objPopupNewSave.SetActive(true);
    }

    public void PopupClose()
    {
        objPopupNewSave.SetActive(false);
    }

    private void setTextSaveInfoInSelectInfoPanel(SaveInfo saveInfo)
    {
        txtSelectSaveInfo.text = saveInfo.saveName + "\n" +
            "Lv." + saveInfo.level +
            " " + GameManager.instance.getJobInfo(saveInfo.job) +
            " Exp : " + saveInfo.curExp +
            "\nHP : " + saveInfo.curHP + ", SP : " + saveInfo.curSP +
            "\n" + saveInfo.stageName +
            "\n" + saveInfo.date + " " + saveInfo.time;
         
    }
}
