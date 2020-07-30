using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCtrl : CharacterCtrl
{
    private float _attackSpeed = 15f;
    private float _attackDuration = 0.3f;
    private float _passTime = 0f;

    private float _attackDirection;

    protected override void Awake()
    {
        base.Awake();
        speed = 6f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Attack();
        if (cState == CharacterState.Idle)
        {
            Move();
            FacingDirection();
        }
    }

    public override void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //为了避免突进时attackAngular实时在变化
            _attackDirection = attackAngular;
            cState = CharacterState.Attack;
            GetComponentInChildren<Weapon>().GetComponent<Collider2D>().enabled = true;
        }

        if (cState == CharacterState.Attack)
        {
            if (_passTime < _attackDuration)
            {
                GetComponent<Rigidbody2D>().MovePosition(transform.position + Quaternion.Euler(0, 0, _attackDirection) * transform.right * _attackSpeed * Time.deltaTime);
                _passTime += Time.deltaTime;
            }
            else
            {
                _passTime -= _attackDuration;
                GetComponentInChildren<Weapon>().GetComponent<Collider2D>().enabled = false;
                cState = CharacterState.Idle;
            }
        }
    }
}
