using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCtrl : MonoBehaviour
{
    //角色数n-1(因为不包括自己)
    private const int _characterNum = 1;
    //防御塔数n
    private const int _towerNum = 1;

    //角色姓名
    public string CharacterName;
    //角色血量
    public int health = 100;
    public const int totalHealth = 100;
    public Slider hpSlider;
    //角色饥饿值
    public int hungry = 100;
    public const int totalHungry = 100;
    public Slider hungrySlider;
    private float _hungryDuration = 1f;
    private float _hungryTime = 0;

    //角色技能数量
    public int skillNum = 3;
    //角色资源数：肉、水果、石头、木材
    public int[] resourses = {0, 0, 0, 0};
    public Text[] resourseText = new Text[4];
    //角色列表
    public GameObject[] characters = new GameObject[_characterNum];
    //防御塔列表
    public GameObject[] tower = new GameObject[_towerNum];
    //建造时间
    public Slider buildSlider;
    private float _buildDuration = 5f;
    private float _buildTime = 0;
    public Vector3 buildPosition;

    //角色状态机
    public enum CharacterState
    {
        Idle,
        Attack,
        PrepareToBuild, 
        BuildMove,
        Building, 
        Hurt
    }
    public CharacterState cState;

    //移动速度
    public float speed = 5;
    private Animator _animator;
    //斜运动的速度折扣
    private float discount = 1f;
    //方向名称
    private string[] _directions = new string[8];
    //鼠标坐标, 攻击角度, 鼠标角度
    private float _x, _y, _mouseAngular;
    public float attackAngular;

    public Transform weaponTrans;

    protected virtual void Awake ()
    {
        _animator = gameObject.GetComponent<Animator>();

        _directions[0] = "up";
        _directions[1] = "down";
        _directions[2] = "left";
        _directions[3] = "right";
        _directions[4] = "up_left";
        _directions[5] = "up_right";
        _directions[6] = "down_left";
        _directions[7] = "down_right";

        cState = CharacterState.Idle;

        resourseText[0] = GameObject.Find("MeatText").GetComponent<Text>();
        resourseText[1] = GameObject.Find("FruitText").GetComponent<Text>();
        resourseText[2] = GameObject.Find("StoneText").GetComponent<Text>();
        resourseText[3] = GameObject.Find("WoodText").GetComponent<Text>();

        hpSlider = GameObject.Find("HpSlider").GetComponent<Slider>();
        hpSlider.value = (float)health / totalHealth;
        hungrySlider = GameObject.Find("HungrySlider").GetComponent<Slider>();
        hungrySlider.value = (float)hungry / totalHungry;
        buildSlider = GameObject.Find("BuildSlider").GetComponent<Slider>();
        buildSlider.value = _buildTime / _buildDuration;
        buildSlider.gameObject.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            resourseText[i].text = resourses[i].ToString();
        }
    }

    protected virtual void FixedUpdate ()
    {
        if (_hungryTime < _hungryDuration)
        {
            _hungryTime += Time.deltaTime;
        }
        else
        {
            EditHungry(-1);
            _hungryTime -= _hungryDuration;
        }
        hpSlider.value = (float)health / totalHealth;
        hungrySlider.value = (float)hungry / totalHungry;
        
        if (cState == CharacterState.BuildMove)
        {
            Build(buildPosition);
        }
        if (cState == CharacterState.Building)
        {
            Building();
        }
    }

    /// <summary>
    /// 玩家攻击
    /// </summary>
    public virtual void Attack() { }

    /// <summary>
    /// 玩家移动
    /// </summary>
    public void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        if (h != 0 && v != 0)
        {
            discount = 0.70710678f;
        }
        else
        {
            discount = 1f;
        }

        GetComponent<Rigidbody2D>().MovePosition(transform.position + transform.right * h * speed * discount * Time.deltaTime + 
            transform.up * v * speed * discount * Time.deltaTime);
    }

    /// <summary>
    /// 玩家与武器面朝方向(进攻方向)
    /// </summary>
    public void FacingDirection()
    {
        //将鼠标坐标原点转至屏幕中心
        _x = Input.mousePosition.x - Screen.width / 2;
        _y = Input.mousePosition.y - Screen.height / 2;
        
        if (_x > 0 && _y > 0)
        {
            _mouseAngular = Mathf.Atan(_y / _x);
        }
        else if (_x < 0 && _y > 0)
        {
            _mouseAngular = Mathf.Atan(_y / _x) + Mathf.PI;
        }
        else if (_x < 0 && _y < 0)
        {
            _mouseAngular = Mathf.Atan(_y / _x) - Mathf.PI;
        }
        else if(_x > 0 && _y < 0)
        {
            _mouseAngular = Mathf.Atan(_y / _x);
        }
        attackAngular = _mouseAngular / Mathf.PI * 180;

        //改变武器转向
        weaponTrans.rotation = Quaternion.AngleAxis(attackAngular, Vector3.forward);
        PropsOperate.colliderFaceAngle = attackAngular + 90;

        //改变玩家转向
        //右
        if (attackAngular < 22.5f && attackAngular >= -22.5f)
        {
            Set_bool(_animator, "right", _directions);
        }
        //右下
        if (attackAngular < -22.5f && attackAngular >= -67.5f)
        {
            Set_bool(_animator, "down_right", _directions);
        }
        //下
        if (attackAngular < -67.5f && attackAngular >= -112.5f)
        {
            Set_bool(_animator, "down", _directions);
        }
        //左下
        if (attackAngular < -112.5f && attackAngular >= -157.5f)
        {
            Set_bool(_animator, "down_left", _directions);
        }
        //左
        if (attackAngular < -157.5f || attackAngular >= 157.5f)
        {
            Set_bool(_animator, "left", _directions);
        }
        //左上
        if (attackAngular < 157.5f && attackAngular >= 112.5f)
        {
            Set_bool(_animator, "up_left", _directions);
        }
        //上
        if (attackAngular < 112.5f && attackAngular >= 67.5f)
        {
            Set_bool(_animator, "up", _directions);
        }
        //右上
        if (attackAngular < 67.5f && attackAngular >= 22.5f)
        {
            Set_bool(_animator, "up_right", _directions);
        }
    }

    /// <summary>
    /// 角色建造
    /// </summary>
    /// <param name="position"></param>
    public void Build(Vector3 position)
    {
        //这里有一个问题，少了一个参数：防御塔种类，现在SendMessage只有一个参数，所以只能先这样做，但后面用更好的方法对接接口之后就要改了
        //建造位置玩家站立的地方还是不准确
        if (Mathf.Abs(position.x - transform.position.x) > Mathf.Abs(position.y - transform.position.x))
        {
            MoveToBuildy(position.y);
            MoveToBuildx(position.x);
        }
        else
        {
            MoveToBuildx(position.x);
            MoveToBuildy(position.y);
        }
        if (transform.position.x == position.x && transform.position.y == position.y)
        {
            cState = CharacterState.Building;
            buildSlider.gameObject.SetActive(true);
        }
    }

    private void MoveToBuildx(float x)
    {
        int dire = 1;
        if (x - transform.position.x > 0)
        {
            Set_bool(_animator, "right", _directions);
            dire = 1;
        }
        else if(x - transform.position.x < 0)
        {
            Set_bool(_animator, "left", _directions);
            dire = -1;
        }
        if (x - transform.position.x < -0.1 || x - transform.position.x > 0.1)
        {
            transform.position += transform.right * dire * speed * Time.deltaTime;
        }
        else
        {
            transform.position = new Vector3(x, transform.position.y, 0);
        }
    }

    private void MoveToBuildy(float y)
    {
        int dire = 1;
        if (y - transform.position.y > 0)
        {
            Set_bool(_animator, "up", _directions);
            dire = 1;
        }
        else if(y - transform.position.y < 0)
        {
            Set_bool(_animator, "down", _directions);
            dire = -1;
        }
        if (y - transform.position.y < -0.1 || y - transform.position.y > 0.1)
        {
            transform.position += transform.up * dire * speed * Time.deltaTime;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, y, 0);
        }
    }

    public void Building()
    {
        if (_buildTime < _buildDuration)
        {
            _buildTime += Time.deltaTime;
            buildSlider.value = _buildTime / _buildDuration;
        }
        else
        {
            _buildTime -= _buildDuration;
            buildSlider.value = _buildTime / _buildDuration;
            buildSlider.gameObject.SetActive(false);
            Instantiate(tower[0], buildPosition, Quaternion.Euler(0, 0, 0));
            cState = CharacterState.Idle;
        }
    }

    /// <summary>
    /// 用于上一个角色向这一个角色进行血量传值
    /// </summary>
    /// <param name="h"></param>
    public void SetHealth(int h)
    {
        health = h;
    }

    /// <summary>
    /// 用于对血量进行变化
    /// </summary>
    /// <param name="n"></param>
    public void EditHealth(int n)
    {
        health += n;
        if (health > 100)
        {
            health = 100;
        }
        else if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void Die()
    {
        Camera.main.GetComponent<GUICtrlInCamera>().OperateUI(Camera.main.GetComponent<GUICtrlInCamera>().menu);
        Destroy(gameObject);
    }

    /// <summary>
    /// 用于上一个角色向这一个角色进行饥饿值传值
    /// </summary>
    /// <param name="h"></param>
    public void SetHungry(int h)
    {
        hungry = h;
    }

    /// <summary>
    /// 用于对饥饿值进行变化
    /// </summary>
    /// <param name="n"></param>
    public void EditHungry(int n)
    {
        hungry += n;
        if (hungry > 100)
        {
            hungry = 100;
        }
        else if (hungry < 0)
        {
            hungry = 0;
            EditHealth(-1);
        }
    }

    /// <summary>
    /// 用于上一个角色向这一个角色进行资源传值
    /// </summary>
    /// <param name="props"></param>
    public void SetResources(int[] props)
    {
        for (int i = 0; i < props.Length; i++)
        {
            resourses[i] = props[i];
        }
    }

    public bool EditMeat(int n)
    {
        resourses[0] += n;
        if (resourses[0] < 0)
        {
            resourses[0] = 0;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool EditFruit(int n)
    {
        resourses[1] += n;
        if (resourses[1] < 0)
        {
            resourses[1] = 0;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool EditStone(int n)
    {
        resourses[2] += n;
        if (resourses[2] < 0)
        {
            resourses[2] -= n;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool EditWood(int n)
    {
        resourses[3] += n;
        if (resourses[3] < 0)
        {
            resourses[3] -= n;
            return false;
        }
        else
        {
            return true;
        }
    }

    public void UpdateResourcesText()
    {
        for (int i = 0; i < 4; i++)
        {
            resourseText[i].text = resourses[i].ToString();
        }
    }

    /// <summary>
    /// 改变动画状态机中用于控制的bool量
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="str"></param>
    /// <param name="strs"></param>
    private void Set_bool(Animator anim, string str, string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (str == strs[i])
            {
                anim.SetBool(str, true);
            }
            else
                anim.SetBool(strs[i], false);
        }
    }

    /// <summary>
    /// 吃到道具而更换角色
    /// </summary>
    public void ChangeCharacter()
    {
        gameObject.SetActive(false);
        GameObject character =  Instantiate(characters[Random.Range(0, _characterNum)], transform.position, transform.rotation);
        //传值
        character.SendMessage("SetHealth", health);
        character.SendMessage("SetHungry", hungry);
        character.SendMessage("SetResources", resourses);
        character.SendMessage("UpdateResourcesText");
        Camera.main.SendMessage("ChangeCharacter");
        Destroy(gameObject);
    }
}
