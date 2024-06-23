using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmMenuController : MonoBehaviour
{
    [SerializeField] Animator _palmMenuAnimator;
    [SerializeField] GameObject _palmMenu;

    private bool isLeftActive = false;
    private bool isRightActive = false;

    void start()
    {
        _palmMenu.SetActive(false);
    }

    public void OpenPalmMenuLeft()
    {
        if(!isRightActive)
        {
            _palmMenu.SetActive(true);
            _palmMenuAnimator.SetTrigger("isOpenLeft");
            isLeftActive = true;
        }
    }

    public void ClosePalmMenuLeft()
    {
        _palmMenuAnimator.SetTrigger("isCloseLeft");
        isLeftActive = false;
    }

    public void OpenPalmMenuRight()
    {
        if (!isLeftActive)
        {
            _palmMenu.SetActive(true);
            _palmMenuAnimator.SetTrigger("isOpenRight");
            isRightActive = true;
        }
    }

    public void ClosePalmMenuRight()
    {
        _palmMenuAnimator.SetTrigger("isCloseRight");
        isRightActive = false;
    }
}
