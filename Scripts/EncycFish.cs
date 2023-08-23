using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EncycFish : MonoBehaviour, IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    public int speed;

    bool isHandling;

    void Update()
    {
        if(!isHandling)
        { 
            transform.Rotate(0, 30 * Time.deltaTime, 0, Space.World);
        }
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        isHandling = true;
    }

   public void OnDrag(PointerEventData pointerEventData)
    {
        float x = pointerEventData.delta.x * Time.deltaTime * speed;
        float y = pointerEventData.delta.y * Time.deltaTime * speed;

        transform.Rotate(y, -x, 0, Space.World);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        isHandling = false;
    }
}
