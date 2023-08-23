using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Player player;
    public Animator boxAnim;

    Rigidbody rigid;

    public bool isFish;
    public bool isRecive;
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {

        rigid.AddForce(new Vector2(4.3f, 7.5f), ForceMode.Impulse);
        int torqueX = Random.Range(1, 6);
        int torqueY = Random.Range(1, 6);
        rigid.AddTorque(new Vector3(torqueX, torqueY, 0), ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(isFish)
        {
            if (player.IsBoxFull || !isRecive)
            {
                rigid.AddForce(new Vector2(3.5f, 7.5f), ForceMode.Impulse);
                Destroy(gameObject, 2);
            }
            else
            {
                if (other.gameObject.layer != 6)
                {
                    boxAnim.SetTrigger("doRecive");

                    Destroy(gameObject);
                }
            }
        }
        else
        {
            boxAnim.SetTrigger("doRecive");

            Destroy(gameObject);
        }
    }
}
