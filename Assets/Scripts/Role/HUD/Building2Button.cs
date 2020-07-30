using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building2Button : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonFunc);
    }

    public void ButtonFunc()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");

    }
}
