using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster {
    //怪物受伤害来源方向
    private Vector3 DamageVector;
    public override void Idle()
    {
        base.Idle();
    }
    public override void Escape()
    {
        MoveToPosition(-DamageVector);
        float EscapeTime = 0;
        if (Time.time > EscapeTime + 2.5 && Damage == 0)
            setState(State.Idle);
        EscapeTime = Time.time;
    }
    public override void Attack()
    {
        base.Attack();
    }
    public override void SetDamage()
    {
        if (getHitState() && Time.time > hitTime + 1.025)
        {
            //Debug.Log("1");
            if (Mathf.Abs(gameObject.transform.position.y - Enemy.transform.position.y) < 0.25f)
            {
                Enemy.SendMessage("EditHealth", -10);
                Debug.Log(Enemy.GetComponent<CharacterCtrl>().health);
                // Debug.Log("2");
            }
            hitTime = Time.time;
            setHitState();
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Character"))
        {
             Debug.Log("begin Attack");
             setState(State.Attack);
        }
        if (col.collider.CompareTag("bullet"))
        {
            Damage=col.gameObject.GetComponent<BulletMove>().Damage;
            ///

        }
    }
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        setDamage(1);
        setMonsterHp(5);
        setMoveSpeed(1);
        setViewRadius(1.5f);
        rayCount = 30.0f;
        sethitDistance(1.5f);
        hitTime = 0;
        _canHit = false;
        monsterPool = GameObject.Find("ObjectsPool").GetComponent<ObjectsPool>();
        setState(State.Idle);
        Damage = 0;
    }
    // Use this for initialization
    void Start()
    {

    }
    // Update is called once per frame
    void Update () {
        Debug.Log(getState());
        if (gameObject.activeSelf)
        {
            Screen();
            switch (getState())
            {
                case State.Idle:
                    Idle();
                    break;
                case State.Move:
                    Move();
                    break;
                case State.Attack:
                    Attack();
                    break;
                case State.Hurt:
                    Hurt();
                    break;
                case State.Escape:
                    Escape();
                    break;
                case State.Dead:
                    Die();
                    break;
            }
        }
    }
}
