using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Character")
        {
            if (gameObject.tag == "Fruit")
            {
                collider.SendMessage("EditFruit", 1);
                PropsOperate.propsPools[0].ReturnInstance(gameObject);
            }
            else if (gameObject.tag == "Wood")
            {
                collider.SendMessage("EditWood", 1);
                PropsOperate.propsPools[1].ReturnInstance(gameObject);
            }
            else if (gameObject.tag == "StoneProp")
            {
                collider.SendMessage("EditStone", 1);
                PropsOperate.propsPools[2].ReturnInstance(gameObject);
            }
            else if (gameObject.tag == "Meat")
            {
                collider.SendMessage("EditMeat", 1);
                PropsOperate.propsPools[3].ReturnInstance(gameObject);
            }
            collider.SendMessage("UpdateResourcesText");
        }
    }
}
