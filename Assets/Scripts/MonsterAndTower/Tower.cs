using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    //创造者ID
    //private int createrID;
    //塔自身ID
    //public int Id;
    //塔是否处于攻击状态
    public bool ifShoot;
    //攻击间隔
    public float shootRate = 0.5f;
    //上次攻击时间
    public float lastShootTime = 0.0f;
    //攻击范围
    private float shootRadius = 5;
    //炮弹发射点的位置
    public Transform shootSpawn;
    //炮弹发射方向
    public Vector3 shootvector;
    //炮弹速度
    public float moveSpeed;
    //炮弹预制体
    public GameObject bullet;
    //炮弹池
    public ObjectsPool BulletPool;
    //塔自身的血量
    public int Hp = 5;
    //进入范围内的对象
    public  GameObject target;
    //攻击对象的位置信息
    public Transform targetTrans;
    //攻击状态判断函数
   void OnTriggerEnter2D(Collider2D col)
    {
       if(col.CompareTag("monster"))
       {
            Debug.Log("BUDENGYU");
            targetTrans = col.transform;
            shootvector = Vector3.Normalize(targetTrans.position - shootSpawn.transform.position);
            ifShoot = true;
            Debug.Log(ifShoot);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("monster"))
        {
            targetTrans = col.transform;
            shootvector = Vector3.Normalize(targetTrans.position - shootSpawn.transform.position);
            ifShoot = true;
        }
        else
        {
            ifShoot = false;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
            Debug.Log("out");
            ifShoot = false;
    }
   
    void Shoot()
    {
        if (ifShoot&&Time.time>lastShootTime + shootRate)
        {
            //炮塔位置（A） - 攻击对象位置（B） 得到两者之间的向量 B->A
            //            Quaternion angle = Quaternion.identity;
            //          angle.SetFromToRotation(shootSpawn.transform.forward, shootvector);
            //shootSpawn.transform.LookAt(transform.position + shootvector);
            //Transform bullet = spawnPool.Spawn("bullet");
            //Instantiate(BulletPool.GetInstance(),shootSpawn.transform.position,shootSpawn.transform.rotation);
            bullet = BulletPool.GetInstanceOnPosition(shootSpawn.position, shootSpawn.rotation.eulerAngles.z);
            //bullet.GetComponent<Rigidbody2D>().velocity = shootvector * moveSpeed;
            lastShootTime = Time.time;
            ifShoot = false;
        }
    }
    void Awake()
    {
        BulletPool = GetComponent<ObjectsPool>();
        GetComponent<CircleCollider2D>().radius = shootRadius;
        ifShoot = false;
        moveSpeed = 5.0f;
    }
    // Use this for initialization
    void Start () {
        Debug.Log(BulletPool.prefab.name);
	}
	
	// Update is called once per frame
	void Update () {
        Shoot();
	}
}
