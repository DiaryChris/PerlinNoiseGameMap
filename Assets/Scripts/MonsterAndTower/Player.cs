using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private int _hp;
	// Use this for initialization
	void Start () {
		
	}
    public int getHp()
    {
        return _hp;
    }
    private void Die()
    {
        if (_hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void setHp(int _hp)
    {
        _hp = this._hp;
    }
	// Update is called once per frame
	void Update () {
        _hp = 10;
	}
}
