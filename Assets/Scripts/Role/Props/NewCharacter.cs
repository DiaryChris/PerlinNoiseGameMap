using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacter : MonoBehaviour
{
    //若被玩家吃掉，则向玩家对象传值，调用换角色方法
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Character")
        {
            collider.SendMessage("ChangeCharacter");
            Destroy(gameObject);
        }
    }
}
