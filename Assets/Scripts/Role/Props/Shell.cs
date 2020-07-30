using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float duration = 0.2f;
    private float _nowTime = 0f;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		if (_nowTime < duration)
        {
            _nowTime += Time.deltaTime;
        }
        else
        {
            RedCtrl.shellsPool.ReturnInstance(gameObject);
            _nowTime -= duration;
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "monster")
        {
            collider.GetComponent<Slime>().Damage = 1;
            collider.GetComponent<Slime>().setState(Monster.State.Hurt);
            RedCtrl.shellsPool.ReturnInstance(gameObject);
        }
        else if (collider.tag == "Tree" || collider.tag == "Stone")
        {
            RedCtrl.shellsPool.ReturnInstance(gameObject);
        }
    }
}
