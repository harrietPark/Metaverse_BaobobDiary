using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBubble : MonoBehaviour
{
    public void InsBubble()
    {
        this.gameObject.SetActive(true);
    }

    public void DeactivateBubble()
    {
        this.gameObject.SetActive(false);
    }

    public void PrintDebug()
    {
        Debug.Log("left active");
    }

    public void PrintDebugRight()
    {
        Debug.Log("right active");
    }
}
