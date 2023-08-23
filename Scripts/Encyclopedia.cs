using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviour
{
    public GameObject[] pages;
    public GameObject[] encyclObjects;
    public Text[] encyclDates;
    public GameObject[] pageBtns;
    public bool[] isRegist = new bool[11];
    public SoundManager soundManager;
    public Shop shop;
    public GameObject backgroundShopExitBtn;
    public GameObject contentGroupObj;
    public GameObject[] contentFishes;
    public Player player;
    public DataManager dataManager;

    public Text[] contentFishesInfo;

    public string[] infoName;
    public string[] infoWeight;
    public string[] infoTemp;
    public string[] infoDeeps;
    public string[] infoUsage;
    public string[] infoComp;

    Animator anim;

    int curPage;

    bool isEncycOn;
    public bool IsEncycOn { get { return isEncycOn; } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        curPage = 0;
    }

    private void Start()
    {
        DataFromManager();
        BtnSet(curPage);
        PageSet();
    }

    /////////아래는 데이터 관리////////////
    void DataFromManager()
    {
        for(int i = 0; i < isRegist.Length; i++)
        {
            isRegist[i] = dataManager.playerData.isRegist[i];
            encyclDates[i].text = dataManager.playerData.registDate[i];
        }
    }
    ////////////////여기까지////////////////////

    ///////// Encyclopedia 자체 로직 /////////
    public void PageSet()
    {
        for(int i = 0; i < 11; i++)
        {
            if(isRegist[i])
            {
                encyclObjects[i].SetActive(true);
            }
        }
    }

    public void EncyclPageUp()
    {
        curPage++;

        BtnSet(curPage);

        for (int j = 0; j < 3; j++)
        {
            pages[j].SetActive(false);
        }

        pages[curPage].SetActive(true);
    }

    public void EncyclPageDown()
    {
        curPage--;

        BtnSet(curPage);

        for (int j = 0; j < 3; j++)
        {
            pages[j].SetActive(false);
        }

        pages[curPage].SetActive(true);
    }

    void BtnSet(int page)
    {
        if(page == 0)
        {
            pageBtns[0].SetActive(false);
            pageBtns[1].SetActive(true);
        }
        else if(page == 1)
        {
            pageBtns[0].SetActive(true);
            pageBtns[1].SetActive(true);
        }
        else if (page == 2)
        {
            pageBtns[0].SetActive(true);
            pageBtns[1].SetActive(false);
        }
    }

    public void EncyclBtn()
    {
        if (shop.gameObject.activeSelf)
            shop.HideAnim();

        if (gameObject.activeSelf)
        {
            ExitBtn();
        }
        else
        {
            gameObject.SetActive(true);
            backgroundShopExitBtn.SetActive(true);
            anim.SetTrigger("doOn");
            soundManager.SetBgm("Shop");
            isEncycOn = true;
        }
    }

    public void HideAnim()
    {
        anim.SetTrigger("doOff");
    }

    public void ExitBtn()
    {
        isEncycOn = false;

        if (!gameObject.activeSelf) return;

        anim.SetTrigger("doOff");
        soundManager.SetBgm("Normal");
        backgroundShopExitBtn.SetActive(false);
        
    }

    void EncyclOff()
    {
        gameObject.SetActive(false);
        isEncycOn = false;
    }

    //////////////// 여기까지 ////////////////

    ///////// Encyclopedia Contents /////////

    public void OnEncycContent(int fishNum)
    {
        contentGroupObj.SetActive(true);

        contentFishesInfo[0].text = infoName[fishNum];
        contentFishesInfo[1].text = infoWeight[fishNum];
        contentFishesInfo[2].text = infoTemp[fishNum];
        contentFishesInfo[3].text = infoDeeps[fishNum];
        contentFishesInfo[4].text = player.fishValues[fishNum].ToString() + " gold";
        contentFishesInfo[5].text = player.fishHealth[fishNum].ToString() + " pull";
        contentFishesInfo[6].text = infoUsage[fishNum];
        contentFishesInfo[7].text = infoComp[fishNum];

        contentFishes[fishNum].SetActive(true);
    }

    public void OffEncycContent()
    {
        for(int i = 0; i < contentFishes.Length; i++)
        {
            contentFishes[i].transform.rotation = Quaternion.identity;
            contentFishes[i].SetActive(false);
        }

        contentGroupObj.SetActive(false);
    }

    //////////////// 여기까지 ////////////////
}
