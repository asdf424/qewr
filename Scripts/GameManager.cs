using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] startUIs;
    public Animator[] inGameUIsAnim;
    public DataManager dataManager;    
    public SoundManager soundManager;
    public Player player;
    public Shop shop;
    public Encyclopedia encyclopedia;
    public GameObject optionPanel;
    public GameObject optionBackGroundBtn;
    public GameObject initPanel;
    public GameObject initBackGroundBtn;
    public Text fishCountText;
    public Text goldText;
    public Text pearlText;

    [Header("===Scene===")]
    public bool isReload;

    public bool[] isCounting = new bool[3];

    bool isOption;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    public void GameStart()
    {
        if (!isReload)
        {
            foreach (GameObject ui in startUIs)
            {
                Animator uiAnim = ui.GetComponent<Animator>();
                uiAnim.SetTrigger("doOff");
                player.ReadyToFishing();
                soundManager.bgmPlayer.Play();
            }
            foreach (Animator anim in inGameUIsAnim)
            {
                anim.SetTrigger("doOn");
            }
        }
        else
        {
            OffStartUIs();

            player.ReadyToFishing();
            soundManager.bgmPlayer.Play();
            foreach (Animator anim in inGameUIsAnim)
            {
                anim.SetTrigger("doOn");
            }
        }

        StartCoroutine(AutoSave());
    }

    public void HideBtns()
    {
        inGameUIsAnim[1].SetTrigger("doOff");
    }

    public void ShowBtns()
    {
        inGameUIsAnim[1].SetTrigger("doOn");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (shop.IsShopOn)
            {
                shop.ExitBtn();
            }
            else if (encyclopedia.IsEncycOn)
            {
                encyclopedia.ExitBtn();
                encyclopedia.OffEncycContent();
            }
            else
            {
                if (!isOption)
                {
                    OnOptionPanel();
                }
                else if (isOption)
                {
                    OffOptionPanel();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!isCounting[0])
        {
            StartCoroutine(SetUIs(fishCountText, dataManager.playerData.fish, player.CountFish,  0));
            dataManager.playerData.fish = player.CountFish;
        }
        if (!isCounting[1])
        { 
            StartCoroutine(SetUIs(goldText, dataManager.playerData.gold, player.Gold,  1));
            dataManager.playerData.gold = player.Gold;
        }
        if (!isCounting[2])
        { 
            StartCoroutine(SetUIs(pearlText, dataManager.playerData.pearl, player.Pearl, 2));
            dataManager.playerData.pearl = player.Pearl;
        }
    }

    public void OffStartUIs()
    {
        foreach (GameObject ui in startUIs)
        {
            ui.SetActive(false);
        }
    }

    IEnumerator SetUIs(Text text, int dataCost, int playerCost, int check)
    {
        isCounting[check] = true;

        int offset = playerCost - dataCost;


        if (dataCost < playerCost)
        {
            while (dataCost < playerCost)
            {
                dataCost += (int)(offset * Time.deltaTime * 0.5f) > 1 ?
                     (int)(offset * Time.deltaTime * 0.5f) : 1;

                text.text = dataCost.ToString();
                yield return null;
            }
        }
        else if (dataCost > playerCost)
        {
            while (dataCost > playerCost)
            {
                dataCost += (int)(offset * Time.deltaTime * 0.5f) < -1 ?
                     (int)(offset * Time.deltaTime * 0.5f) : -1;

                text.text = dataCost.ToString();
                yield return null;
            }
        }

        dataCost = playerCost;
        text.text = dataCost.ToString();

        isCounting[check] = false;
    }

    public void HideAllUIs()
    {
        shop.ExitBtn();
        encyclopedia.ExitBtn();
    }

    public void OnOptionPanel()
    {
        Time.timeScale = 0;
        optionPanel.SetActive(true);
        isOption = true;
    }
    public void OffOptionPanel()
    {
        SaveAudioData();

        Time.timeScale = 1;
        optionPanel.SetActive(false);
        isOption = false;
    }

    IEnumerator AutoSave()
    {
        while (true)
        {
            SaveGameData();
            
            yield return new WaitForSeconds(3);
        }
    }

    void SaveGameData()
    {
        dataManager.playerData = player.DataToManager(dataManager.playerData);
        dataManager.Save();
    }

    void SaveAudioData()
    {
        dataManager.playerData.masterAudioValue = soundManager.masterSlider.value;
        dataManager.playerData.bgmValue = soundManager.bgmSlider.value;
        dataManager.playerData.sfxValue = soundManager.sfxSlider.value;
    }

    public void SceneToSoalrSystem()
    {
        dataManager.Save();
        SceneManager.LoadScene(1);
    }

    public void InitializeData()
    {
        DataManager.PlayerData initPlayerData = new DataManager.PlayerData(true);
        dataManager.playerData = initPlayerData;
        dataManager.Save();
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void OnInitPanel()
    {
        initPanel.SetActive(true);
        initBackGroundBtn.SetActive(true);
    }

    public void OffInitPanel()
    {
        initPanel.SetActive(false);
        initBackGroundBtn.SetActive(false);
    }

    public void ExitGame()
    {
        dataManager.Save();
        Application.Quit();
    }
}
