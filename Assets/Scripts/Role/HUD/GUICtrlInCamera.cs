using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUICtrlInCamera : MonoBehaviour
{
    public GameObject bagUI;
    public GameObject character;
    public GameObject menu;

	void Start ()
    {
        character = GameObject.FindGameObjectWithTag("Character");
    }
	
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Tab))
        {
            OperateUI(bagUI);
        }
        if (character.GetComponent<CharacterCtrl>().cState == CharacterCtrl.CharacterState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //打开全局菜单
                OperateUI(menu);
            }
        }
	}

    public void OperateUI(GameObject ui)
    {
        if (ui.activeInHierarchy == true)
        {
            ui.SetActive(false);
        }
        else
        {
            ui.SetActive(true);
        }
    }
}
