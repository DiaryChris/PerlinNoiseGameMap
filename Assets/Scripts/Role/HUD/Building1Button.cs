using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building1Button : MonoBehaviour
{
    public GameObject buildArea;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonFunc);
    }

    public void ButtonFunc()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");
        //塔一要消耗30个石头和20个木头
        //改数值的时候记得把BuildingArea内的数值也改了
        if (character.GetComponent<CharacterCtrl>().EditStone(-30))
        {
            if (character.GetComponent<CharacterCtrl>().EditWood(-20))
            {
                GameObject area = Instantiate(buildArea, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                area.GetComponent<BuildingArea>().towerType = 1;
                character.GetComponent<CharacterCtrl>().cState = CharacterCtrl.CharacterState.PrepareToBuild;
                character.SendMessage("UpdateResourcesText");
                Camera.main.GetComponent<GUICtrlInCamera>().OperateUI(Camera.main.GetComponent<GUICtrlInCamera>().bagUI);
            }
            else
            {
                character.GetComponent<CharacterCtrl>().EditStone(30);
            }
        }
    }
}
