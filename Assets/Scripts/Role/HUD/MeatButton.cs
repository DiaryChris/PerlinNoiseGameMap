using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeatButton : MonoBehaviour
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonFunc);
    }

    public void ButtonFunc()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");
        if (character.GetComponent<CharacterCtrl>().EditMeat(-1))
        {
            character.SendMessage("UpdateResourcesText");
            character.SendMessage("EditHungry", 15);
            character.SendMessage("EditHealth", 5);
        }
    }
}
