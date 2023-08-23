using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkyWay : MonoBehaviour
{
    public int speed;

    public MilkyWay nextMilky;
    public Transform headerTrans;
    
    Transform trans;

    public bool isHead;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        speed = 1;
    }

    void Update()
    {
        Move();
        Scrolling();
        
    }

    private void FixedUpdate()
    {
        Relocate();
    }

    void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = 0.2f * speed * Time.deltaTime * Vector3.left;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {
        if (isHead && trans.position.x <= -8)
        {
            trans.position = headerTrans.position + Vector3.right * 8;
            this.isHead = false;
            nextMilky.isHead = true;
        }
    }

    void Relocate()
    {
        if(!isHead)
        trans.position = headerTrans.position + Vector3.right * 8;
    }
}
