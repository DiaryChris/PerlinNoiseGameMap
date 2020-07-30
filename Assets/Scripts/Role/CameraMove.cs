using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform characterTrans;
    public Vector3 offset;

    public static bool isShake;
    private float _shakeDuration;
    private float _shakePassTime = 0;
    private int _shakeTimes = 0;
    private int _shakeDirection;
    private float _shakeSpeed;
    public static float attackDirection;
    private Vector3 _shakeOffset;

    public Texture2D cursorTexture;
    
	void Start ()
    {
        _shakeDuration = 0.03f;
        _shakeDirection = -1;
        _shakeSpeed = 3;
        isShake = false;
        _shakeOffset = new Vector3(0, 0, 0);

        characterTrans = GameObject.FindGameObjectWithTag("Character").transform;
        offset = transform.position - characterTrans.position;

        Cursor.SetCursor(cursorTexture, new Vector2(200, 200), CursorMode.Auto);
    }
	
	void LateUpdate ()
    {
        transform.position = characterTrans.position + offset;
        if (isShake == true)
        {
            ShakeScene();
        };
    }

    public void ChangeCharacter()
    {
        characterTrans = GameObject.FindGameObjectWithTag("Character").transform;
        GetComponent<GUICtrlInCamera>().character = characterTrans.gameObject;
    }

    public void ShakeScene()
    {
        if (_shakeTimes < 2)
        {
            if (_shakePassTime < _shakeDuration)
            {
                _shakeOffset += Quaternion.Euler(0, 0, attackDirection) * transform.right * _shakeSpeed * _shakeDirection * Time.deltaTime;
                //相机的方向和后座力的方向是相反的
                transform.position += _shakeOffset;
                _shakePassTime += Time.deltaTime;
            }
            else
            {
                _shakeTimes++;
                _shakeDirection *= -1;
                _shakePassTime = 0;
            }
        }
        else
        {
            isShake = false;
            _shakeTimes = 0;
            _shakeOffset = new Vector3(0, 0, 0);
        }
    }
}
