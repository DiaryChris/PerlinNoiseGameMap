using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Monster : MonoBehaviour {
    //怪物的生命值
    private int _MonsterHp;
    //怪物移动速度
    private float _moveSpeed;
    //怪物攻击范围
    private float _hitDistance;
    //怪物伤害
    private int _damage;
    //怪物攻击时间
    public float hitTime;
    //怪物是否可以造成伤害
    public bool _canHit;
    //怪物动画组件
    public Animator animator;
    //怪物受到的伤害
    public int Damage;
    //怪物视距
    private float _viewRadius;
    //怪物视距射线数
    public float rayCount;
    //怪物对象池
    public ObjectsPool monsterPool;
    //怪物当前状态
    State state = State.Idle;
    //怪物之前的状态
    public State beforeState;
    //玩家对象
    public GameObject Enemy;
    // Use this for initialization
    public enum State
    {
        //待命状态
        Idle,
        //移动
        Move,
        //进攻敌方
        Attack,
        //受伤
        Hurt,
        //逃跑
        Escape,
        //死亡
        Dead,
    }
    //获得当前状态
    public State getState()
    {
        return state;
    }
    //设置状态
    public void setState(State state)
    {
        this.state = state;
    }
    //获得怪物的伤害
    public int getDamage()
    {
        return _damage;
    }
    //设置怪物的伤害
    public void setDamage(int _damage)
    {
        this._damage = _damage;
    }
    //获得怪物生命值
    public int getMonsterHp()
    {
        return _MonsterHp;
    }
    //设置怪物生命值
    public void setMonsterHp(int _MonsterHp)
    {
        this._MonsterHp = _MonsterHp;
    }
    //获得怪物移动速度
    public float getMoveSpeed()
    {
        return _moveSpeed;
    }
    //设置怪物移动速度
    public void setMoveSpeed(float _moveSpeed)
    {
        this._moveSpeed = _moveSpeed;
    }
    //获得怪物攻击范围
    public float gethitDistance()
    {
        return _hitDistance;
    }
    //设置怪物攻击范围
    public void sethitDistance(float _hitDistance)
    {
        this._hitDistance = _hitDistance;
    }
    //获得怪物视距
    public float getViewRadius()
    {
        return _viewRadius;
    }
    //设置怪物视距
    public void setViewRadius(float _viewRadius)
    {
        this._viewRadius = _viewRadius;
    }
    //获得怪物攻击状态
    public bool getHitState()
    {
        return _canHit;
    }
    //变更怪物攻击状态
    public void setHitState()
    {
        _canHit = !_canHit;
    }
    //怪物视野
    public void Screen()
    {
        Vector3 forward_left = Quaternion.Euler(0, 0, 135) * transform.up * getViewRadius();
        for (int i = 0; i < rayCount; i++)
        {
            Vector3 v = Quaternion.Euler(0, 0, (360 / rayCount) * i) * forward_left * getViewRadius();
            Ray2D ray = new Ray2D(transform.position, v);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, getViewRadius(), LayerMask.GetMask("Character"));
            Vector3 pos = transform.position + v;
            if (hit.transform != null)
            {
                pos = hit.point;
            }
            Debug.DrawLine(transform.position, pos, Color.red);
            //这里变更状态会出现问题
            if (hit.transform != null && hit.transform.gameObject.tag == "Character")
            {
                //发现玩家
                Debug.Log("Find Character");
                //state = State.Move;
                Enemy = hit.transform.gameObject;
                //Debug.Log(state);
            }
        }
    }
    //面对玩家
    private void FaceToEnemy(GameObject Enemy)
    {
        if (gameObject.transform.position.x < Enemy.transform.position.x)
        {
            gameObject.transform.rotation = Quaternion.Euler(0.0f, -180.0f, 0.0f);
        }
        if (gameObject.transform.position.x > Enemy.transform.position.x)
        {
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
    }
    public void MoveToPosition(Vector3 pos)
    {
        Vector3 v = pos - transform.position;
        v.z = 0;
        //角色移动的方法（效果显著）
        GetComponent<Rigidbody2D>().MovePosition(transform.position += v.normalized * getMoveSpeed() * Time.deltaTime);
    }
    //怪物Idle状态
    virtual public void Idle()
    {
        animator.SetInteger("State", 0);
        if (Enemy != null && Damage == 0)
        {
            setState(State.Move);
            Debug.Log("1");
        }
        else if (Enemy != null && Damage != 0)
        {
            beforeState = State.Move;
            setState(State.Hurt);
        }
        else if (Enemy == null && Damage != 0)
        {
            beforeState = State.Escape;
            setState(State.Hurt);
        }
    }
    //怪物移动
    virtual public void Move()
    {
        if (Enemy != null)
        {
            FaceToEnemy(Enemy);
            //计算自身到玩家的距离
            float distance = (gameObject.transform.position - Enemy.transform.position).magnitude;
            //朝玩家移动
            animator.SetInteger("State", 1);
            MoveToPosition(Enemy.transform.position);
            //10.0为最大追击距离
            if (distance > 10.0f)
            {
                Debug.Log("stop");
                Enemy = null;
                setState(State.Idle);
            }
            if (Damage != 0)
            {
                beforeState = getState();
                setState(State.Hurt);
            }
            if (getMonsterHp() <= 0)
            {
                setState(State.Dead);
            }
        }
    }
    //怪物攻击
    virtual public void Attack()
    {
        if (Enemy != null)
        {
            //改变怪物朝向，使得怪物保持面朝玩家
            FaceToEnemy(Enemy);
            animator.SetInteger("State", 2);
            //攻击动画播放完计算伤害
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Debug.Log(animator.GetInteger("State"));
                setHitState();
                SetDamage();
            }
            //怪物与玩家距离超过最大追击距离时变更状态为Idle
            if ((gameObject.transform.position - Enemy.transform.position).magnitude > 10.0f)
            {
                setState(State.Idle);
                setHitState();
            }
            else if ((gameObject.transform.position - Enemy.transform.position).magnitude < gethitDistance())
            {
                setState(State.Attack);
            }
            else
            {
                setState(State.Move);
                setHitState();
            }
            //怪物收到伤害时播放受伤动画
            if (Damage != 0)
            {
                beforeState = getState();
                setState(State.Hurt);
            }
            if (getMonsterHp() <= 0)
            {
                setState(State.Dead);
            }
        }
    }
    virtual public void SetDamage()
    {

    }
    public void damage()
    {
        setMonsterHp(getMonsterHp() - Damage);
        Damage = 0;
    }
    public void Hurt()
    {
        //播放怪物受伤动画
        animator.SetInteger("State", 3);
        damage();
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && (animator.GetCurrentAnimatorStateInfo(0).IsName("Slime_Hurt")))
        {
            Debug.Log(getMonsterHp());
            
            setState(beforeState);
        }
        if (getMonsterHp() <= 0)
        {
            setState(State.Dead);
        }
        //    //伤害判断（怪物血量更新）
        //    _MonsterHp -= Damage;
        //    _animator.SetInteger("State", 1);
        //}
        //if (Damage != 0&&_animator.GetInteger("State")!=3)
        //{
        //    int nowState = _animator.GetInteger("State");
        //    _animator.SetInteger("State", 3);
        //    _MonsterHp -= Damage;
        //    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        //    {
        //        _animator.SetInteger("State", nowState);
        //        Damage = 0;
        //    }
        //}
    }
    virtual public void Escape()
    {
        
    }
    //怪物死亡动画
    public void Die()
    {
        animator.SetBool("Die", true);
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && (animator.GetCurrentAnimatorStateInfo(0).IsName("Slime_Die")))
        {
            monsterPool.ReturnInstance(gameObject);
        }
    }
    void Awake()
    {

    }
    void Start ()
    {

	}
	// Update is called once per frame
	void Update ()
    {

	}

}
