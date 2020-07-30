using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour {
    //炮弹伤害
    public int Damage;
    //炮弹速度
    public float shootSpeed;
    //炮弹飞行时间
    public float time;
    //敌人方向
    public Vector3 EnemyVector;
    //塔对象
    Tower tower;
    //炮弹对象池
    public ObjectsPool bulletPool;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("monster"))
        {
            bulletPool.ReturnInstance(gameObject);
            time = 0;
        }
    }
    void Destroy()
    {
        if (gameObject.activeSelf && time < 2)
        {
            time += Time.deltaTime;
        }
        else
        {
            bulletPool.ReturnInstance(gameObject);
            time = 0;
        }
    }
    void Awake()
    {
         shootSpeed = 5.0f;
         time = 0.0f;
         Damage = 1;
         tower = GameObject.Find("tower").GetComponent<Tower>();
         bulletPool = tower.GetComponent<ObjectsPool>();
    }
    void OnEnable()
    {
        EnemyVector = tower.shootvector;
        gameObject.GetComponent<Rigidbody2D>().velocity = EnemyVector * shootSpeed;
    }

    // Use this for initialization
    void Start () {
        //这里使用tower.shootvector会报错
        //Debug.Log(movement+"move");
    }
	
	// Update is called once per frame
	void Update () {
        Destroy();
	}
}
