using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitButton : MonoBehaviour
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonFunc);
    }

    public void ButtonFunc()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");
        if (character.GetComponent<CharacterCtrl>().EditFruit(-1))
        {
            character.SendMessage("UpdateResourcesText");
            character.SendMessage("EditHungry", 5);
            character.SendMessage("EditHealth", 1);
        }
    }
}
