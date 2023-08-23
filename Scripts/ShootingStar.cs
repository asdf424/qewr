using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    public Transform endPos;

    Transform trans;

    private void Awake()
    {
        trans = GetComponent<Transform>();
    }

    void Update()
    {
        trans.Translate(new Vector2(8, -6.4f) * Time.deltaTime * 1.3f);
        if (trans.position.x > endPos.position.x || trans.position.y < endPos.position.y)
        {
            gameObject.SetActive(false);
        }
    }
}
