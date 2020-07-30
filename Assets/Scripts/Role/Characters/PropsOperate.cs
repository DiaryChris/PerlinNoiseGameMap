using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsOperate : MonoBehaviour
{
    public static float colliderFaceAngle = 0;
    public GameObject[] props = new GameObject[3];
    public Transform propsParent;

    public static ObjectsPool[] propsPools = new ObjectsPool[3];

    void Start ()
    {
        propsParent = GameObject.Find("Props").transform;

        for (int i = 0; i < 3; i++)
        {
            propsPools[i] = gameObject.AddComponent<ObjectsPool>();
            propsPools[i].prefab = props[i];
            propsPools[i].poolsTrans = GameObject.Find("ObjectsPool").transform;
        }
    }
	
	void Update ()
    {
        transform.rotation = Quaternion.AngleAxis(colliderFaceAngle, Vector3.forward);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (collider.tag == "Tree")
            {
                //生成水果
                for (int i = 0; i < Random.Range(0, 2); i++)
                {
                    Vector3 offset = new Vector3(Random.Range(-100, 100) / 50, Random.Range(-100, 100) / 50, 0);
                    GameObject fruit = propsPools[0].GetInstanceOnPosition(collider.transform.position + offset, 0);
                    fruit.transform.SetParent(propsParent); 
                }
                //生成木材
                for (int j = 0; j < Random.Range(1, 4); j++)
                {
                    Vector3 offset = new Vector3(Random.Range(-100, 100) / 50, Random.Range(-100, 100) / 50, 0);
                    GameObject wood = propsPools[1].GetInstanceOnPosition(collider.transform.position + offset, 0);
                    wood.transform.SetParent(propsParent);
                }
                Destroy(collider.gameObject);
            }
            else if (collider.tag == "Stone")
            {
                //生成石头
                for (int i = 0; i < Random.Range(1, 5); i++)
                {
                    Vector3 offset = new Vector3(Random.Range(-100, 100) / 50, Random.Range(-100, 100) / 50, 0);
                    GameObject stone = propsPools[2].GetInstanceOnPosition(collider.transform.position + offset, 0);
                    stone.transform.SetParent(propsParent);
                }
                Destroy(collider.gameObject);
            }
        }
        
    }
}
