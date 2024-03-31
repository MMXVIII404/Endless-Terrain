using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPanel : MonoBehaviour
{
    [SerializeField]
    GameObject adjustments;
    [SerializeField]
    FirstPersonController firstPersonController;
    [SerializeField]
    StarterAssetsInputs starterAssetsInputs;

    void Start()
    {
        adjustments.SetActive(false);
    }

    public void PressButton()
    {
        adjustments.SetActive(!adjustments.activeSelf);
        if (adjustments.activeSelf)
        {
            Cursor.visible = true;
            firstPersonController.enabled = false;
            starterAssetsInputs.UpdateCursorLock(false);
        }
        else
        {
            Cursor.visible = false;
            firstPersonController.enabled = true;
            starterAssetsInputs.UpdateCursorLock(true);
        }
    }
}
