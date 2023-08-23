using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Player : MonoBehaviour
{
    [Header("===DebugMod===")]
    public bool isFeverDebug;

    [Header("===Managers===")]
    public GameManager gameManager;
    public DataManager dataManager;
    public SoundManager soundManager;

    [Header("===Scene===")]
    public bool isReload;

    [Header("===Player Anim===")]
    public Animator rodAnim;
    public GameObject rod;

    [Header("===Fish===")]
    public GameObject[] fishPrefabs;
    public int[] fishHealth;
    public int[] fishValues;

    [Header("===Pearl===")]
    public GameObject pearlPrefab;

    [Header("===Watch===")]
    public GameObject watchPrefab;

    [Header("===Fever===")]
    public GameObject bigStars;
    public GameObject exclamationBox;
    public Animator starFeverAnim;
    public Animator blackHoleFeverAnim;
    public MilkyWay[] milkyWays;


    [Header("===UI===")]
    public GameObject[] startUIs;
    public GameObject registPanel;
    public GameObject fishPullGaugeObj;
    public RegistManager registManager;
    public Image coolTimeGauge;
    public Image coolTimeBar;
    public Image fishPullGauge;
    public Shop shop;
    public Encyclopedia encyclopedia;
    bool[] isRegist = new bool[11];

    [Header("===Others===")]
    public GameObject fishBox;
    public GameObject fishingHook;
    public Sprite[] fishBoxes;
    public GameObject[] starShips;
    public GameObject satelliteBtn;

    Transform trans;
    Animator anim;
    Animator hookAnim;
    SpriteRenderer boxSpriteRenderer;

    bool isStep;
    bool isReady;
    public bool IsReady => isReady;
    bool isBoxReady;
    public bool IsBoxReady => isBoxReady;
    bool isRodOn;
    public bool IsRodOn => isRodOn;

    bool isSatelRun;
    bool isFever;
    bool isExclamation;
    bool isReadyForFever;
    bool isFeverGaugeCoolTime;
   
    public bool IsFever { get { return isFever; } }

    bool[] isMove = new bool[3];
    int curFish;
    int pullGauge;
    int feverGauge;
    public float FeverGauge { get { return feverGauge; } }

    int countFish;
    public int CountFish { get { return countFish; } }
    [SerializeField]
    int gold;
    public int Gold { get { return gold; } }
    [SerializeField]
    int pearl;
    public int Pearl { get { return pearl; } }

    //////// Player Specification///////
    [SerializeField]
    int rodLevel;
    [SerializeField]
    int boxLevel;
    [SerializeField]
    int strengthLevel;

    public int RodLevel
    { get { return rodLevel; } set { rodLevel = value; } }
    public int BoxLevel
    { get { return boxLevel; } set { boxLevel = value; } }
    public int StrengthLevel
    { get { return strengthLevel; } set { strengthLevel = value; } }

    int[] maxFishByRodLevel = new int[5] { 0, 3, 6, 8, 11 };
    int[] maxFishByBoxLevel = new int[5] { 100, 200, 500, 1000, 10000 };
    int[] strengthByLevel = new int[5] { 1, 2, 3, 4, 5 };

    public int MaxFishByBoxLevel => maxFishByBoxLevel[boxLevel];

    public bool IsBoxFull => countFish >= maxFishByBoxLevel[boxLevel];

    float animAccel = 1f;
    float animAccelInit = 0;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        boxSpriteRenderer = fishBox.GetComponent<SpriteRenderer>();
        hookAnim = fishingHook.GetComponent<Animator>();
    }

    private void Start()
    {
        DataFromManager();

        if (!isReload)
        {
            StartCoroutine(StartRoutine());
            StartCoroutine(StepRoutine());
        }
        else
        {
            AllSkip();
        }

        SetFishNum();
    }

    private void Update()
    {
        // user input // 
        if (shop.IsShopOn || encyclopedia.IsEncycOn) return;

        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!isReady && Input.GetMouseButtonDown(0) && isStep)
            {
                SkipStartRoutine();
            }
            else if (!isBoxReady && isReady && Input.GetMouseButtonDown(0))
            {
                SkipSetBox();
            }
            else if (isReady && isBoxReady && isRodOn && !isFever && Input.GetMouseButtonDown(0))
            {
                Pull();
                if (animAccel < 5)
                    animAccel += Time.deltaTime * 2;
                animAccelInit = 0;
            }
            else if (isReady && isBoxReady && isFever && Input.GetMouseButtonDown(0))
            {
                FeverPull();
                if (animAccel < 5)
                    animAccel += Time.deltaTime * 2;
                animAccelInit = 0;
            }
        }

        animAccelInit += Time.deltaTime;

        if (animAccelInit > 5)
            animAccel = 1;

        anim.speed = animAccel;
        rodAnim.speed = animAccel;
    }
    private void LateUpdate()
    {
        // fish box //
        if (isBoxReady)
        {
            int spriteNum = countFish == 0 ? 0 : countFish * 3 / maxFishByBoxLevel[boxLevel];

            if (countFish > maxFishByBoxLevel[boxLevel])
                spriteNum = 3;

            switch (boxLevel)
            {
                case 0:
                    boxSpriteRenderer.sprite = fishBoxes[spriteNum];
                    break;
                case 1:
                    boxSpriteRenderer.sprite = fishBoxes[4 + spriteNum];
                    break;
                case 2:
                    boxSpriteRenderer.sprite = fishBoxes[8 + spriteNum];
                    break;
                case 3:
                    boxSpriteRenderer.sprite = fishBoxes[12 + spriteNum];
                    break;
                case 4:
                    if (!fishBox.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MaxBoxRecive"))
                        boxSpriteRenderer.sprite = fishBoxes[16 + spriteNum];

                    break;

            }

        }
        
        // fish gauge //
        if (isFever)
        {
            fishPullGauge.rectTransform.sizeDelta =
                  new Vector2(fishPullGauge.rectTransform.sizeDelta.x, 60);

            if (!isFeverGaugeCoolTime)
            {
                float R, G, B;
                R = Random.Range(0, 1f);
                G = Random.Range(0, 1f);
                B = Random.Range(0, 1f);
                fishPullGauge.color = new Color(R, G, B);

                isFeverGaugeCoolTime = true;
                Invoke("FeverGaugeCool", 0.1f);
            }

        }
        else if(isReadyForFever)
        {
            fishPullGauge.rectTransform.sizeDelta =
                   new Vector2(fishPullGauge.rectTransform.sizeDelta.x, 60);

            fishPullGauge.color = new Color(0, 1, 1);
        }
        else
        {
            float y = pullGauge * 60 / fishHealth[curFish];

            fishPullGauge.rectTransform.sizeDelta =
                new Vector2(fishPullGauge.rectTransform.sizeDelta.x, y > 60 ? 60 : y);

            float R, G, B;

            R = y > 30 ? 1 - (y-30) / 30 : 1;
            G = y < 30 ? y/30 : 1;
            B = R == 0 ? 1 : 0.5f;

            fishPullGauge.color = new Color(R, G, B);

        }
    }

    /////////아래는 데이터 관리////////////
    void DataFromManager()
    {
        gold = dataManager.playerData.gold;
        countFish = dataManager.playerData.fish;
        pearl = dataManager.playerData.pearl;
        rodLevel = dataManager.playerData.rodLevel;
        boxLevel = dataManager.playerData.boxLevel;
        strengthLevel = dataManager.playerData.strengthLevel;

        for (int i = 0; i < isRegist.Length; i++)
        {
            isRegist[i] = dataManager.playerData.isRegist[i];
        }
    }

    public DataManager.PlayerData DataToManager(DataManager.PlayerData playerData)
    {
        playerData.gold = gold;
        playerData.fish = countFish;
        playerData.pearl = pearl;
        playerData.rodLevel = rodLevel;
        playerData.boxLevel = boxLevel;
        playerData.strengthLevel = strengthLevel;

        return playerData;
    }
    ////////////////여기까지////////////////////

    /////////아래는 시작 전 준비 루틴////////////
    IEnumerator StartRoutine()
    {
        isStep = true;
        anim.SetInteger("level", strengthLevel);

        while (trans.transform.position.x > -1)
        {
            trans.Translate(new Vector2(-1, 0) * Time.deltaTime);
            yield return null;

            if(trans.transform.position.x < -0.5f && !isMove[0])
            {
                StartBtnSet(0);
            }
            else if (trans.transform.position.x < -0.7f && !isMove[1])
            {
                StartBtnSet(1);
            }
            else if (trans.transform.position.x < -0.9f && !isMove[2])
            {
                StartBtnSet(2);
            }
        }

        isStep = false;

        anim.SetTrigger("doIdle");
        yield return new WaitForSeconds(3);

        anim.SetTrigger("doReady");
        yield return new WaitForSeconds(2.5f);

        while (!isReady)
        {
            soundManager.SfxPlay(Sound.ready);
            yield return new WaitForSeconds(2);
        }
    }
    void SkipStartRoutine()
    {
        StopAllCoroutines();

        trans.position = new Vector2(-1, trans.position.y);

        isStep = false;

        StartCoroutine(ReadyRoutine());

        StartBtnSet(0);
        StartBtnSet(1);
        StartBtnSet(2);
    }
    IEnumerator ReadyRoutine()
    {
        anim.SetTrigger("doReady");
        yield return new WaitForSeconds(2.5f);

        while (!isReady)
        {
            soundManager.SfxPlay(Sound.ready);
            yield return new WaitForSeconds(0.8f);
        }
    }

    void StartBtnSet(int number)
    {
        Animator btnAnim;

        switch (number)
        {
            case 0:
                btnAnim = startUIs[0].GetComponent<Animator>();
                btnAnim.SetTrigger("doMove");
                isMove[0] = true;
                break;
            case 1:
                btnAnim = startUIs[1].GetComponent<Animator>();
                btnAnim.SetTrigger("doMove");
                isMove[1] = true;
                break;
        }    
    }

    IEnumerator StepRoutine()
    {
        while (isStep)
        {
            soundManager.SfxPlay(Sound.walk);
            yield return new WaitForSeconds(0.45f);
        }
    }

    public void ReadyToFishing()
    {
        anim.SetTrigger("doStart");

        isReady = true;
    }

    void SetBox()
    {
        if (isBoxReady) return;

        fishBox.SetActive(true);
        Animator boxAmin = fishBox.GetComponent<Animator>();
        boxAmin.SetInteger("level", boxLevel);
        boxAmin.SetTrigger("doTransform");
        Invoke("EndSetBox", 2.5f);
    }
    void EndSetBox()
    {
        isBoxReady = true;
    }

    void SkipSetBox()
    {
        fishBox.SetActive(true);
        Animator boxAmin = fishBox.GetComponent<Animator>();
        boxAmin.SetInteger("level", boxLevel);
        boxAmin.SetTrigger("doIdle");
        fishBox.transform.position = new Vector2(3.25f, -1);
        isBoxReady = true;
        SetHook();
    }

    void AllSkip()
    {
        gameManager.GameStart();
        trans.position = new Vector2(-1, trans.position.y);
        rod.SetActive(true);
        isRodOn = true;
        isStep = false;
        isReady = true;
        fishBox.SetActive(true);
        Animator boxAmin = fishBox.GetComponent<Animator>();
        boxAmin.SetInteger("level", boxLevel);
        boxAmin.SetTrigger("doIdle");
        fishBox.transform.position = new Vector2(3.25f, -1);
        isBoxReady = true;
        SetHook();
        ChangeAnim();
    }
    void OnRod()
    {
        if (isReload) return;

        gameManager.OffStartUIs();
        rod.SetActive(true);
        rodAnim.SetInteger("level", rodLevel);
        isRodOn = true;
    }

    ////////////////여기까지////////////////////


    /////////아래는 낚시 관련 로직////////////
    void SetFishNum()
    {
        if (feverGauge == 7)
        {
            fishHealth[11] = strengthByLevel[strengthLevel] * 20;
            curFish = 11;
        }
        else
        {
            curFish = Random.Range(0, maxFishByRodLevel[rodLevel]);
        }
            
    }
    void SetHook()
    {
        fishingHook.SetActive(true);
        fishPullGaugeObj.SetActive(true);
    }
    void Pull()
    {
        if (isReadyForFever) return;

        int dir = Random.Range(0, 2);

        if (!isExclamation && feverGauge == 7)
        {
            isExclamation = true;
            OnExclamationBox();
        }

        if (pullGauge >= fishHealth[curFish])
        {
            Catch(curFish, dir);
        }
        else
        {
            pullGauge += strengthByLevel[strengthLevel];
            anim.SetBool("CatchR", false);
            anim.SetBool("CatchL", false);
            rodAnim.SetBool("CatchR", false);
            rodAnim.SetBool("CatchL", false);

            hookAnim.SetTrigger("Pull");

            soundManager.SfxPlay(Sound.pull);
        }

        if (dir == 0)
        {
            anim.SetTrigger("doPullR");
            rodAnim.SetTrigger("doPullR");
        }
        else
        {
            anim.SetTrigger("doPullL");
            rodAnim.SetTrigger("doPullL");
        }
    }

    void FeverPull()
    {
        if (isReadyForFever) return;

        int dir = Random.Range(0, 2);

        if (dir == 0)
        {
            anim.SetTrigger("doPullR");
            rodAnim.SetTrigger("doPullR");
        }
        else
        {
            anim.SetTrigger("doPullL");
            rodAnim.SetTrigger("doPullL");
        }

        Instant(curFish, 7);

        hookAnim.SetTrigger("Catch");
        SetFishNum();

    }

    void Catch(int curFish, int dir)
    {
        if (feverGauge == 7)
        {
            isReadyForFever = true;
            InitInstantObject(watchPrefab,false,true);
            soundManager.SfxPlay(Sound.catc);
            SetFishNum();
            StartFeverTime();
        }
        else
        {
            Instant(curFish, 9);

            pullGauge = 0;

            if (isFeverDebug)
                feverGauge = 7;
            else
                feverGauge = Random.Range(0, 100);

            SetFishNum();
        }

        if (dir == 0)
        {
            anim.SetBool("CatchR", true);
            rodAnim.SetBool("CatchR", true);
        }
        else
        {
            anim.SetBool("CatchL", true);
            rodAnim.SetBool("CatchL", true);
        }

        hookAnim.SetTrigger("Catch");
    }

    void InitInstantObject(GameObject instantObj, bool isFish, bool isRecive)
    {
        Fish fish = Instantiate(instantObj).GetComponent<Fish>();
        fish.boxAnim = fishBox.GetComponent<Animator>();
        fish.player = this;
        fish.isFish = isFish;
        fish.isRecive = isRecive;
    }

    void Instant(int curFish, int pearlProb)
    {
        int fishProbability = Random.Range(0, 11);

        if (fishProbability > pearlProb)
        {
            pearl++;
            soundManager.SfxPlay(Sound.pearl);
            InitInstantObject(pearlPrefab, false, true);
        }
        else
        {
            if (countFish + fishValues[curFish] <= maxFishByBoxLevel[boxLevel])
            {
                countFish += fishValues[curFish];
                soundManager.SfxPlay(Sound.catc);
                InitInstantObject(fishPrefabs[curFish], true, true);
                soundManager.SfxPlay(Sound.catc);
                Regist(curFish);
            }
            else
            {
                soundManager.SfxPlay(Sound.catc);
                InitInstantObject(fishPrefabs[curFish], true, false);
            }
        }
    }

    void Regist(int curFish)
    {
        if (!isRegist[curFish])
        {
            if (!registManager.IsRegisting)
            {
                registPanel.SetActive(true);
                registManager.RegistAnim(curFish);
                //dataManager.playerData.isRegist[curFish] = true;
                this.isRegist[curFish] = true;
            }
            else
            {
                registManager.EndRegistAnim();
                registPanel.SetActive(false);
                registPanel.SetActive(true);
                registManager.RegistAnim(curFish);
                //dataManager.playerData.isRegist[curFish] = true;
                this.isRegist[curFish] = true;

            }

        }
    }

    void StartFeverTime()
    {
        StartCoroutine(FeverRoutine());
    }

    void OnExclamationBox()
    {
        isExclamation = true;
        exclamationBox.SetActive(true);
        Invoke("OffExclamationBox", 1);
    }
    void OffExclamationBox()
    {
        exclamationBox.SetActive(false);
    }

    IEnumerator FeverRoutine()
    {
        isExclamation = false;

        feverGauge = 0;
        SetFishNum();

        gameManager.HideBtns();

        while(Time.timeScale > 0.5f)
        {
            Time.timeScale -= 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        
        yield return new WaitForSeconds(1);

        Time.timeScale = 1;

        isReadyForFever = false;
        isFever = true;
        soundManager.SetBgm("Fever");
        starFeverAnim.SetTrigger("doFever");
        blackHoleFeverAnim.SetTrigger("doFever");

        foreach (MilkyWay m in milkyWays)
        {
            m.speed = 20;
        }


        yield return new WaitForSeconds(5);

        isFever = false;
        pullGauge = 0;
        soundManager.SetBgm("Normal");
        starFeverAnim.SetTrigger("doNormal");
        blackHoleFeverAnim.SetTrigger("doNormal");
        foreach (MilkyWay m in milkyWays)
        {
            m.speed = 1;
        }


        gameManager.ShowBtns();
    }

    void CatchAnim()
    {
        trans.position = new Vector2(-0.7f, trans.position.y);
    }

    void FinishCatch()
    {
        trans.position = new Vector2(-1, trans.position.y);
    }

    void FeverGaugeCool()
    {
        isFeverGaugeCoolTime = false;
    }

    ////////////////여기까지////////////////////


    /////////아래는 물고기 회수 루틴////////////
    public void OnSatelliteBtn()
    {
        if (isSatelRun) return;

        isSatelRun = true;
        satelliteBtn.GetComponent<Button>().enabled = false;
        satelliteBtn.GetComponent<Animator>().SetTrigger("doCall");
        OnFallStarShip();
    }
    void OnFallStarShip()
    {
        StartCoroutine(FallStarShipRoutine());
    }
    IEnumerator FallStarShipRoutine()
    {
        Transform fallTrans = starShips[0].GetComponent<Transform>();
        Animator fallAnim = starShips[0].GetComponent<Animator>();

        yield return new WaitForSeconds(2);

        fallAnim.SetTrigger("doFly");

        while (fallTrans.position.y < 3)
        {
            fallTrans.position = Vector2.MoveTowards
                (fallTrans.position, new Vector2(2.18f, 3), Time.deltaTime * 2);
            yield return null;
        }

        soundManager.StarShipSfxPlay();
        soundManager.isLanding = true;
        soundManager.StarShipSfxVolumeDown(0.2f);

        OffFallStarShip();
        OnLandStarShip();
        fallAnim.SetTrigger("doStop");
    }
    void OffFallStarShip()
    {
        starShips[0].transform.position = new Vector2(0.5f, -4);
    }
    void OnLandStarShip()
    {
        Animator landAnim = starShips[1].GetComponent<Animator>();
        landAnim.SetTrigger("doFly");
        StartCoroutine(LandStarShipRoutine());
    }
    IEnumerator LandStarShipRoutine()
    {
        Transform landTrans = starShips[1].GetComponent<Transform>();
        Animator landAnim = starShips[1].GetComponent<Animator>();
        bool isLand = false;

        while (landTrans.position.y > -0.2f)
        {
            if (landTrans.position.y < 0.3f && !isLand)
            {
                landAnim.SetTrigger("doIdle");
                isLand = true;
            }
            landTrans.position = Vector2.MoveTowards
                (landTrans.position, new Vector2(landTrans.position.x, -0.3f), Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        gold += countFish * 1000;
        countFish = 0;
        soundManager.SfxPlay(Sound.gold);

        yield return new WaitForSeconds(1);

        soundManager.starShipPlayer.volume = 0.5f;
        soundManager.StarShipSfxPlay();
        
        yield return new WaitForSeconds(1);

        soundManager.isLanding = false;
        soundManager.StarShipSfxVolumeDown(0.15f);

        while (landTrans.position.y < 3)
        {
            if (landTrans.position.y > 0.5f && isLand)
            {
                landAnim.SetTrigger("doFly");
                isLand = false;
            }
            landTrans.position = Vector2.MoveTowards
                (landTrans.position, new Vector2(landTrans.position.x, 3), Time.deltaTime * 1.5f);
            yield return null;
        }

        landAnim.SetTrigger("doIdle");
        OffSatelliteBtn();

    }
    void OffSatelliteBtn()
    {
        isSatelRun = false;
        satelliteBtn.GetComponent<Animator>().SetTrigger("doFinish");

        StartCoroutine(SatelliteCoolTime());
    }

    IEnumerator SatelliteCoolTime()
    {
        coolTimeBar.gameObject.SetActive(true);
        coolTimeGauge.gameObject.SetActive(true);
        coolTimeBar.rectTransform.sizeDelta = new Vector2(0, 1);

        int coolTime = 0;
        float coolTimeRatio;

        while(coolTime != 30)
        {
            yield return new WaitForSecondsRealtime(1);

            coolTime++;
            coolTimeRatio = coolTime * 19 / 30;
            coolTimeBar.rectTransform.sizeDelta = new Vector2(coolTimeRatio, 1);
        }

        coolTimeBar.gameObject.SetActive(false);
        coolTimeGauge.gameObject.SetActive(false);

        satelliteBtn.GetComponent<Button>().enabled = true;

    }

    ////////////////여기까지////////////////////

    public void ChangeAnim()
    {
        anim.SetInteger("level", strengthLevel);
        rodAnim.SetInteger("level", rodLevel);
        fishBox.GetComponent<Animator>().SetInteger("level", boxLevel);
        anim.SetTrigger("AnimSet");
        rodAnim.SetTrigger("AnimSet");
    }

    public void GoldConsum(int goldSpent)
    {
        gold -= goldSpent;
    }

    public void PearlConsum(int pearlSpent)
    {
        pearl -= pearlSpent;
    }

}
