using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveInfo {
        
    public string saveName;     // 저장 이름
    
    public int curHP;           // HP
    public int curSP;           // SP
    public int curExp;          // 현재 경험치
    public int level;           // 레벨
    public Job.JOB job;         // 직업
    public string stageName;         // 스테이지 이름;
    public string stageSceneName;
    public float posX;          // 케릭터 위치    
    public float posY;          // 위치
    public float posZ;          // 위치
    public string date;         // 날짜 // System.DateTime.Now.ToString("yyyy/MM/dd");
    public string time;         // 시간 // System.DateTime.Now.ToString("hh:mm:ss"); 

    [SerializeField]
    public List<SaveItemInfo> listInventoryItem = new List<SaveItemInfo>(); // 인벤토리 아이템

    [SerializeField]
    public List<SaveItemInfo> listStorageItem = new List<SaveItemInfo>(); // 창고 아이템

    [SerializeField]
    public List<SaveItemInfo> listHotBarItem = new List<SaveItemInfo>(); // 슬롯 아이템

    [SerializeField]
    public List<SaveItemInfo> listCharacterSystemItem = new List<SaveItemInfo>(); // 장착한 아이템

    public void setInfo(string saveName, int Hp, int Sp, int Exp, int level, Job.JOB job,
        string stageName, string stageSceneName, Vector3 pos, string date, string time,
        List<Item> listInven, List<Item> listStorage, List<Item> listHorBar, List<Item> listCharSys )
    {
        this.saveName = saveName;
        curHP = Hp;
        curSP = Sp;
        curExp = Exp;
        this.level = level;
        this.job = job;
        this.stageName = stageName;
        this.stageSceneName = stageSceneName;
        posX = pos.x;
        posY = pos.y;
        posZ = pos.z;
        this.date = date;
        this.time = time;
        setSaveItemInfoFromItemList(listInventoryItem, listInven);
        setSaveItemInfoFromItemList(listStorageItem, listStorage);
        setSaveItemInfoFromItemList(listHotBarItem, listHorBar);
        setSaveItemInfoFromItemList(listCharacterSystemItem, listCharSys);
    }
    private void setSaveItemInfoFromItemList(List<SaveItemInfo> listSaveItemInfo, List<Item> listItem)
    {
        if (listItem == null)
            return;
        listSaveItemInfo.Clear();
        foreach (Item item in listItem)
        {
            SaveItemInfo iteminfo = new SaveItemInfo();
            iteminfo.itemID = item.itemID;
            iteminfo.itemValue = item.itemValue;
            listSaveItemInfo.Add(iteminfo);
        }
    }    
}
