using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "monster")
            if (GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterCtrl>().cState == CharacterCtrl.CharacterState.Attack)
            {
                //Slime的血量是5
                collider.GetComponent<Slime>().Damage = Random.Range(1, 3);
                collider.GetComponent<Slime>().setState(Monster.State.Hurt);
            }
    }
}
