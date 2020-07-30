using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCtrl : CharacterCtrl
{
    public static ObjectsPool shellsPool;
    public GameObject shell;
    public float fireSpeed = 20;

    private float _attackTime = 0;
    private float _attackDuration = 1f;
    private bool _canAttack = true;

    protected override void Awake()
    {
        base.Awake();
        shellsPool = gameObject.GetComponent<ObjectsPool>();
        shellsPool.poolsTrans = GameObject.Find("ObjectsPool").transform;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_canAttack)
        {
            Attack();
        }

        if (_attackTime < _attackDuration)
        {
            _attackTime += Time.deltaTime;
        }
        else
        {
            _attackTime -= _attackDuration;
            _canAttack = true;
        }

        if (cState == CharacterState.Idle)
        {
            FacingDirection();
        }
        Move();
    }

    public override void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cState = CharacterState.Attack;
        }

        if (cState == CharacterState.Attack)
        {
            CameraMove.attackDirection = attackAngular;
            CameraMove.isShake = true;
            for (int i = -5; i < 6; i++)
            {
                GameObject go = shellsPool.GetInstanceOnPosition(transform.position, i * 10 + attackAngular);
                go.GetComponent<Rigidbody2D>().velocity = go.transform.right * fireSpeed;
            }
            _canAttack = false;
            cState = CharacterState.Idle;
        }
    }
}
