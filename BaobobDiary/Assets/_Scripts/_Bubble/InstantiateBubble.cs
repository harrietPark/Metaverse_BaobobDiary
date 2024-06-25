using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBubble : MonoBehaviour
{
    [SerializeField] GameObject sphere;

    public void InsBubble()
    {
        sphere.SetActive(true);
    }

    public void DeactivateBubble()
    {
        sphere.SetActive(false);
    }

    public void PrintDebug()
    {
        Debug.Log(" active");
    }

    public void PrintDebugRight()
    {
        Debug.Log("not active");
    }
}
