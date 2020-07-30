using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    //需要加载和实例化的预制体对象资源
    public GameObject prefab;
    //对象池（利用队列来实现）
    private Queue<GameObject> _pooledInstanceQueue = new Queue<GameObject>();
    public Transform poolsTrans;

    //得到对象函数
    public GameObject GetInstanceOnPosition(Vector3 position, float angle)
    {
        //判断当前队列数量是否为0（即对象池是否为空），若不为空则从池子中取得对象返回
        if (_pooledInstanceQueue.Count > 0)
        {
            GameObject instanceToReuse = _pooledInstanceQueue.Dequeue();
            //设置对象的产生位置
            instanceToReuse.transform.position = position;
            instanceToReuse.transform.rotation = Quaternion.Euler(0, 0, angle);
            //将从队列中取得的对象设为可用
            instanceToReuse.SetActive(true);
            //返回该对象
            return instanceToReuse;
        }
        //若为空则创建资源
        return Instantiate(prefab, position, Quaternion.Euler(0, 0, angle));
    }

    public void ReturnInstance(GameObject gameObjectToPool)
    {
        _pooledInstanceQueue.Enqueue(gameObjectToPool);
        gameObjectToPool.SetActive(false);
        //设置父物体
        gameObjectToPool.transform.SetParent(poolsTrans);
    }
}
