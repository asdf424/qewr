using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistManager : MonoBehaviour
{
    public List<GameObject> registFishPrefabs;
    public string[] registFishNames;
    public Encyclopedia encyclopedia;
    public Text registFishName;
    public Text registFishNumber;
    public Text registFishDate;

    int lastRegistFishNum;
    bool isRegisting;
    public bool IsRegisting => isRegisting;

    void Regist(int fishNum, string registDate)
    {
        encyclopedia.isRegist[fishNum] = true;
        encyclopedia.encyclDates[fishNum].text = registDate;
        encyclopedia.PageSet();

        for (int i = 0; i < encyclopedia.isRegist.Length; i++)
        {
            encyclopedia.dataManager.playerData.isRegist[i] = encyclopedia.isRegist[i];
            encyclopedia.dataManager.playerData.registDate[i] = encyclopedia.encyclDates[i].text;
        }
    }

    public void RegistAnim(int fishNum)
    {
        isRegisting = true;

        registFishPrefabs[fishNum].SetActive(true);
        registFishPrefabs[fishNum].GetComponent<Rigidbody>().AddTorque(new Vector3(0, 2, 0), ForceMode.Impulse);

        registFishName.text = registFishNames[fishNum];

        registFishDate.text = System.DateTime.Now.ToString("yyyy/MM/dd");

        if (fishNum < 10)
            registFishNumber.text = "#0" + fishNum.ToString();
        else
            registFishNumber.text = "#" + fishNum.ToString();

        lastRegistFishNum = fishNum;

        Regist(fishNum, registFishDate.text);
    }


    public void EndRegistAnim()
    {
        registFishPrefabs[lastRegistFishNum].transform.rotation = Quaternion.identity;
        registFishPrefabs[lastRegistFishNum].GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
        registFishPrefabs[lastRegistFishNum].SetActive(false);
        this.gameObject.SetActive(false);
        isRegisting = false;
    }


}
