using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Player player;
    public Transform startPos;
    public GameObject[] shootingStars;

    float coolTime;

    private void Start()
    {
        coolTime = Random.Range(10, 30);
        StartCoroutine(StartShooting());
    }

    IEnumerator StartShooting()
    {
        while (true)
        {
            if (!player.IsFever && player.FeverGauge < 3)
            {
                if (coolTime < 1)
                {
                    Shooting();
                    coolTime = Random.Range(10, 30);
                }
                else
                    coolTime -= 1;
            }
            yield return new WaitForSeconds(1);
        }
    }

    void Shooting()
    {
        int starNum = Random.Range(0, 4);
        float startPosX = Random.Range(-4.5f,9.6f);

        if (shootingStars[starNum].activeSelf == false)
        {
            shootingStars[starNum].SetActive(true);
            shootingStars[starNum].transform.position
                = new Vector2(startPos.position.x + startPosX, startPos.position.y);
        }
        else
        {
            return;
        }
    }
}
