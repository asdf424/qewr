using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Sound { walk , ready, pull, catc, pearl, gold, buy }

public class SoundManager : MonoBehaviour
{
    public DataManager dataManager;

    public AudioSource bgmPlayer;
    public AudioSource feverPlayer;
    public AudioSource shopPlayer;
    public AudioSource[] sfxPlayer;
    public AudioSource starShipPlayer;
    public AudioClip[] readySfx;
    public AudioClip[] fishingSfx;
    public AudioClip starShipSfx;
    public AudioClip getGoldSfx;
    public AudioClip buySfx;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public GameObject[] muteIcons;

    AudioSource currentPlayer;
    AudioSource finalPlayer;

    public bool isLanding;

    int channel = 0;
    float downLevel;
    string currentBgm;
    bool isBgmChanging;

    private void Awake()
    {
        currentPlayer = bgmPlayer;
        currentBgm = "Normal";
    }

    private void Start()
    {
        LoadAudioVolume();
    }

    public void SfxPlay(Sound sound)
    {
        int i = channel % 8;

        switch (sound)
        {
            case Sound.walk:
                int step = Random.Range(0, 2);
                sfxPlayer[i].clip = readySfx[step];
                sfxPlayer[i].Play();
                break;
            case Sound.ready:
                int readySound = Random.Range(2, 10); // 2,3,4,5,6,7,8,9
                sfxPlayer[i].clip = readySfx[readySound];
                sfxPlayer[i].Play();
                break;
            case Sound.pull:
                int clipPull = Random.Range(0, 2);
                sfxPlayer[i].clip = fishingSfx[clipPull];
                sfxPlayer[i].Play();
                break;
            case Sound.catc:
                int clipCatch = Random.Range(2, 5);
                sfxPlayer[i].clip = fishingSfx[clipCatch];
                sfxPlayer[i].Play();
                break;
            case Sound.pearl:
                sfxPlayer[i].clip = fishingSfx[5];
                sfxPlayer[i].Play();
                break;
            case Sound.gold:
                sfxPlayer[i].clip = getGoldSfx;
                sfxPlayer[i].Play();
                break;
            case Sound.buy:
                sfxPlayer[i].clip = buySfx;
                sfxPlayer[i].Play();
                break;
        }
        sfxPlayer[i].Play();
        channel++;
    }

    /// <summary>
    /// Bgm = "Normal" or "Fever" or "Shop"
    /// </summary>
    /// <param name="Bgm"></param>
    public void SetBgm(string Bgm)
    {
        if (currentBgm == Bgm) return;

        if (Bgm == "Normal")
        {
            StartCoroutine(BgmChangeRoutine(currentPlayer, bgmPlayer));
            finalPlayer = bgmPlayer;
        }
        else if (Bgm == "Fever")
        {
            StartCoroutine(BgmChangeRoutine(currentPlayer, feverPlayer));
            finalPlayer = feverPlayer;
        }
        else if (Bgm == "Shop")
        {
            StartCoroutine(BgmChangeRoutine(currentPlayer, shopPlayer));
            finalPlayer = shopPlayer;
        }
        else
            return;

        currentBgm = Bgm;
    }

    IEnumerator BgmChangeRoutine(AudioSource cur, AudioSource next)
    {
        if (currentPlayer == next) yield break;

        yield return new WaitUntil(() => !isBgmChanging);

        isBgmChanging = true;

        next.Play();
        
        while (cur.volume > 0)
        {
            cur.volume -= Time.deltaTime * 0.2f;
            if (next.volume < bgmSlider.value)
                next.volume += Time.deltaTime * 0.2f;
            yield return null;
        }

        next.volume = bgmSlider.value;
        cur.volume = 0;

        cur.Pause();

        currentPlayer = next;

        isBgmChanging = false;

        if (currentPlayer != finalPlayer)
        {
            StartCoroutine(BgmChangeRoutine(currentPlayer, finalPlayer));
        }
    }

    public void SetBgmPitch(bool isShopOpen)
    {
        if (isShopOpen == true)
            bgmPlayer.pitch = 1.1f;
        else if (isShopOpen == false)
            bgmPlayer.pitch = 1;
    }

    /////////아래는 우주선 SFX 관리////////////
    public void StarShipSfxPlay()
    {
        starShipPlayer.clip = starShipSfx;
        starShipPlayer.Play();
    }
    public void StarShipSfxStop()
    {
        starShipPlayer.Stop();
    }
    public void StarShipSfxVolumeDown(float downLevel)
    {
        StopCoroutine(StarShipSfxVolumeDownRoutine());

        this.downLevel = downLevel;

        StartCoroutine(StarShipSfxVolumeDownRoutine());
    }
    IEnumerator StarShipSfxVolumeDownRoutine()
    {
        while (starShipPlayer.volume > 0)
        {
            starShipPlayer.volume -= Time.deltaTime * downLevel;
            yield return null;
        }

        if (isLanding)
            starShipPlayer.Pause();
        else
        {
            starShipPlayer.Stop();
            starShipPlayer.volume = 1;
        }
    }
    ////////////////여기까지////////////////////

    /////////아래는 슬라이더 음량 관리////////////
    public void MasterAudioSliderSet()
    {
        bgmPlayer.volume = masterSlider.value;
        feverPlayer.volume = masterSlider.value;
        shopPlayer.volume = masterSlider.value;
        foreach(AudioSource audioSource in sfxPlayer)
        {
            audioSource.volume = masterSlider.value;
        }
        starShipPlayer.volume = masterSlider.value;

        bgmSlider.value = masterSlider.value;
        sfxSlider.value = masterSlider.value;

        foreach(GameObject icon in muteIcons)
        {
            icon.SetActive(SetMuteIcon(masterSlider.value));
        }
    }
    public void BgmAudioSliderSet()
    {
        bgmPlayer.volume = bgmSlider.value;
        feverPlayer.volume = bgmSlider.value;
        shopPlayer.volume = bgmSlider.value;

        muteIcons[1].SetActive(SetMuteIcon(bgmSlider.value));
    }
    public void SfxAudioSliderSet()
    {
        foreach (AudioSource audioSource in sfxPlayer)
        {
            audioSource.volume = sfxSlider.value;
        }
        starShipPlayer.volume = sfxSlider.value;

        muteIcons[2].SetActive(SetMuteIcon(sfxSlider.value));
    }

    bool SetMuteIcon(float sliderValue)
    {
        if (sliderValue == 0)
        {
            return true;
        }

        return false;
    }

    ///////////////////여기까지////////////////////

    void LoadAudioVolume()
    {
        masterSlider.value = dataManager.playerData.masterAudioValue;
        MasterAudioSliderSet();

        bgmSlider.value = dataManager.playerData.bgmValue;
        BgmAudioSliderSet();

        sfxSlider.value = dataManager.playerData.sfxValue;
        SfxAudioSliderSet();

        shopPlayer.volume = 0;
    }
}