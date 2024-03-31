using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputListener : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    FirstPersonController firstPersonController;
    [SerializeField]
    StarterAssetsInputs starterAssetsInputs;
    void Start()
    {
        RenderSettings.fog = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RenderSettings.fog = !RenderSettings.fog;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene("Start");
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            Cursor.visible = false;
            firstPersonController.enabled = true;
            starterAssetsInputs.UpdateCursorLock(true);
            SceneManager.LoadScene("Terrain");
        }
        if (Input.GetKeyDown(KeyCode.BackQuote) && panel != null)
        {
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf)
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
}
