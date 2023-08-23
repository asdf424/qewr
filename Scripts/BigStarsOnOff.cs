using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigStarsOnOff : MonoBehaviour
{
    public GameObject bigStars;

    void BigStarsOn()
    {
        bigStars.SetActive(true);
    }

    void BigStarsOff()
    {
        bigStars.SetActive(false);
    }
}
