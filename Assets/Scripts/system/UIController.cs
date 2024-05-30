using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject optionsScreen;
    public GameObject customScreen;

    /*
        怀念react的一天...

        "" no panel

        "options" options panel

        
    */
    public string curState = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*

            There are several states

            no -> options (esc)

            options -> no (esc)

            custom ui panel -> no (esc)

            no -> custom ui panel (trigger)

        */
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(curState == "" || curState == "options") {
                ShowHideOptions();
            }
            else {
                closeCustomScreen();
            }
        }

        if(curState != "") {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // if (optionsScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        // {
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        // }
    }

    public void ShowHideOptions()
    {
        if(!optionsScreen.activeInHierarchy)
        {
            optionsScreen.SetActive(true);

            curState = "options";
        } else
        {
            optionsScreen.SetActive(false);
            curState = "";
            // Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void showCustomScreen(GameObject customPanel) {
        if(curState == "custom") {
            customScreen.SetActive(false);
        }
        customScreen = customPanel;
        customScreen.SetActive(true);
        curState = "custom";
    }

    public void openExternalLink(string link = "") {
        
        if(link == "") {
            link = "https://github.com/Rising-Stars-by-Sunshine/stats201-Final_Project-Xintong";
        }
        Application.OpenURL(link);
    }

    public bool canInteract() {
        return curState != "options";
    }

    public void closeCustomScreen() {
        customScreen.SetActive(false);
        curState = "";
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
