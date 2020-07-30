using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {
    public float speed;
    public float fireSpeed = 10;
    public GameObject shell;
    private Animator animator;
    private float angular;
    private float discount;
    private string[] directions = new string[8];

	// Use this for initialization
	void Start () {
        animator = gameObject.GetComponent<Animator>();
        angular = 0;
        discount = 1;

        directions[0] = "up";
        directions[1] = "down";
        directions[2] = "left";
        directions[3] = "right";
        directions[4] = "up_left";
        directions[5] = "up_right";
        directions[6] = "down_left";
        directions[7] = "down_right";
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0 && v == 1)
        {
            Set_bool(animator, "up", directions);
            angular = 90;
            discount = 1;
        }
        else if (h == 0 && v == -1)
        {
            Set_bool(animator, "down", directions);
            angular = 270;
            discount = 1;
        }
        else if (h == -1 && v == 0)
        {
            Set_bool(animator, "left", directions);
            angular = 180;
            discount = 1;
        }
        else if (h == 1 && v == 0)
        {
            Set_bool(animator, "right", directions);
            angular = 0;
            discount = 1;
        }
        else if (h == -1 && v == 1)
        {
            Set_bool(animator, "up_left", directions);
            angular = 135;
            discount = 0.70710678f;
        }
        else if (h == 1 && v == 1)
        {
            Set_bool(animator, "up_right", directions);
            angular = 45;
            discount = 0.70710678f;
        }
        else if (h == -1 && v == -1)
        {
            Set_bool(animator, "down_left", directions);
            angular = 225;
            discount = 0.70710678f;
        }
        else if (h == 1 && v == -1)
        {
            Set_bool(animator, "down_right", directions);
            angular = 315;
            discount = 0.70710678f;
        }

        transform.position += transform.right * h * speed * discount * Time.deltaTime;
        transform.position += transform.up * v * speed * discount * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = -5; i < 6; i++)
            {
                GameObject go = Instantiate(shell);
                go.transform.position = transform.position;
                go.transform.rotation = Quaternion.Euler(0, 0, i * 10 + angular);
                go.GetComponent<Rigidbody2D>().velocity = go.transform.right * fireSpeed;
            }
        }

        
    }

    private void Set_bool(Animator anim, string str, string[] strs)
    {
        for(int i = 0; i < strs.Length; i++)
        {
            if (str == strs[i])
            {
                //if (anim.GetBool(str) == false)
                anim.SetBool(str, true);
            }
            else
                anim.SetBool(strs[i], false);
        }
    }
}
