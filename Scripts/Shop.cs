using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public SoundManager soundManager;
    public Player player;
    public Encyclopedia encyclopedia;
    public Image[] shopBtns;
    public GameObject[] shopContents;
    public GameObject[] lockContents;
    public GameObject backgroundShopExitBtn;

    public GameObject[] lockRodImgs;
    public Button[] upgradeRodBtns;
    public Text[] unlockRodGoldTxts;

    public GameObject[] lockBoxImgs;
    public Button[] upgradeBoxBtns;
    public Text[] unlockBoxGoldTxts;

    public GameObject[] lockCosImgs;
    public Button[] upgradeCosBtns;
    public Text[] unlockCosGoldTxts;

    Animator anim;

    int[] rodValues = { 100000, 1000000, 5000000, 10000000 };
    int[] boxValues = { 100000, 1000000, 5000000, 10000000 };
    int[] cosValues = { 100, 500, 1000, 2000 };

    bool isShopOn;
    public bool IsShopOn { get { return isShopOn; } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ShopBtn()
    {
        if (encyclopedia.gameObject.activeSelf)
            encyclopedia.HideAnim();

        if (gameObject.activeSelf)
        {
            ExitBtn();
        }
        else
        {
            gameObject.SetActive(true);
            ShopInit();
            anim.SetTrigger("doOn");
            soundManager.SetBgm("Shop");
            backgroundShopExitBtn.SetActive(true);
            isShopOn = true;
        }
    }

    public void ExitBtn()
    {
        isShopOn = false;

        if (!gameObject.activeSelf) return;

        anim.SetTrigger("doOff");
        soundManager.SetBgm("Normal");
        backgroundShopExitBtn.SetActive(false);
        
    }

    public void HideAnim()
    {
        anim.SetTrigger("doOff");
    }
    void ShopOff()
    {
        gameObject.SetActive(false);
        isShopOn = false;
    }

    public void SetShop(int i)
    {
        for(int j = 0; j < 3; j++)
        {
            shopBtns[j].color = new Color(0.37f, 0.37f, 0.37f);
            shopContents[j].SetActive(false);
            lockContents[j].SetActive(false);
        }

        shopBtns[i].color = new Color(1, 1, 1);
        shopContents[i].SetActive(true);
        lockContents[i].SetActive(true);
    }

    public void RodUpgrade()
    {
        if (!player.IsRodOn) return;
        if (player.Gold < rodValues[player.RodLevel]) return;

        if (player.RodLevel < 3)
        {
            lockRodImgs[player.RodLevel].SetActive(false);
            upgradeRodBtns[player.RodLevel + 1].gameObject.SetActive(true);
        }
        
        upgradeRodBtns[player.RodLevel].interactable = false;
        unlockRodGoldTxts[player.RodLevel].text = "-";
        
        soundManager.SfxPlay(Sound.buy);
        player.GoldConsum(rodValues[player.RodLevel]);

        player.RodLevel++;
        player.ChangeAnim();
    }

    public void BoxUpgrade()
    {
        if (!player.IsBoxReady) return;
        if (player.Gold < boxValues[player.BoxLevel]) return;

        if (player.BoxLevel < 3)
        {
            lockBoxImgs[player.BoxLevel].SetActive(false);
            upgradeBoxBtns[player.BoxLevel + 1].gameObject.SetActive(true);
        }

        upgradeBoxBtns[player.BoxLevel].interactable = false;
        unlockBoxGoldTxts[player.BoxLevel].text = "-";
        
        soundManager.SfxPlay(Sound.buy);
        player.GoldConsum(boxValues[player.BoxLevel]);

        player.BoxLevel++;

        if (player.BoxLevel == 4)
        {
            player.fishBox.GetComponent<Animator>().SetInteger("level", 4);
        }
    }

    public void CostumeUpgrade()
    {
        if (!player.IsReady) return;
        if (player.Pearl < cosValues[player.StrengthLevel]) return;

        if (player.StrengthLevel < 3)
        {
            lockCosImgs[player.StrengthLevel].SetActive(false);
            upgradeCosBtns[player.StrengthLevel + 1].gameObject.SetActive(true);
        }

        upgradeCosBtns[player.StrengthLevel].interactable = false;
        unlockCosGoldTxts[player.StrengthLevel].text = "-";
        
        soundManager.SfxPlay(Sound.buy);
        player.PearlConsum(cosValues[player.StrengthLevel]);

        player.StrengthLevel++;
        player.ChangeAnim();

        if (player.StrengthLevel == 4)
        {
            player.GetComponent<Animator>().SetInteger("level", 4);
        }
    }

    void ShopInit()
    {
        for(int i = 0; i < 3; i++)
        {
            lockRodImgs[i].SetActive(player.RodLevel - 1 < i);
            lockBoxImgs[i].SetActive(player.BoxLevel - 1 < i);
            lockCosImgs[i].SetActive(player.StrengthLevel - 1 < i);
        }
        for(int i = 0; i < 4; i++)
        {
            upgradeRodBtns[i].gameObject.SetActive(player.RodLevel >= i);
            upgradeRodBtns[i].interactable = player.RodLevel <= i;
            upgradeBoxBtns[i].gameObject.SetActive(player.BoxLevel >= i);
            upgradeBoxBtns[i].interactable = player.BoxLevel <= i;
            upgradeCosBtns[i].gameObject.SetActive(player.StrengthLevel >= i);
            upgradeCosBtns[i].interactable = player.StrengthLevel <= i;

            if (player.RodLevel > i)
                unlockRodGoldTxts[i].text = "-";
            if (player.BoxLevel > i)
                unlockBoxGoldTxts[i].text = "-";
            if (player.StrengthLevel > i)
                unlockCosGoldTxts[i].text = "-";
        }
    }
}
