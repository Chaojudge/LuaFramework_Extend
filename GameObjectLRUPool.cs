using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityAlgorithm
{
    /// <summary>
    /// LRU游戏对象缓冲池
    /// 1.新建自定义名称的游戏对象缓冲池，设定其大小
    /// 2.先使用Instantiate生成物体，后加入至游戏对象缓冲池中
    /// 3.在加入相对应的游戏对象缓冲池时需要判断：
    ///     1.是否存在该游戏对象，如果存在则不需要添加，否则需要判断缓冲池是否有空缺
    ///     2.如果缓冲池有空缺，则直接加入，否则需要删除最近最少使用的游戏对象后再加入
    /// 4.使用方法：先判断缓冲池中是否存在游戏对象，如果没有，则先使用Instantiate生成物体
    /// 后加入缓冲池中，如果有则直接从缓冲池中则先使用Instantiate复制一份出来即可。
    /// </summary>
    public class GameObjectLRUPool
    {
        private string poolName;
        private int maxCount;
        private List<GameObject> listGameObject;
        private GameObject gameObjectPool;
        private GameObject childGameObjectPool;

        /// <summary>
        /// 实例化LRU的游戏对象缓冲池
        /// </summary>
        /// <param name="_poolName">缓冲池名称</param>
        /// <param name="_maxCount">缓冲池最大容量</param>
        public GameObjectLRUPool(string _poolName,int _maxCount)
        {
            this.poolName = _poolName;
            this.maxCount = _maxCount;
            if (listGameObject == null)
            {
                listGameObject = new List<GameObject>();
            }
            if (GameObject.FindWithTag("GameObjectPool") == null)
            {
                gameObjectPool = new GameObject("GameObjectPool");
                gameObjectPool.tag = "GameObjectPool";
            }
            else
            {
                gameObjectPool = GameObject.FindWithTag("GameObjectPool");
            }
            if (!gameObjectPool.transform.Find(this.poolName))
            {
                childGameObjectPool = new GameObject(this.poolName);
                childGameObjectPool.transform.SetParent(gameObjectPool.transform, false);
            }
            else
            {
                Debug.LogError(string.Format("{0}缓冲池已经存在", this.poolName));
            }
        }

        /// <summary>
        /// 添加游戏对象至缓冲池中
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        public void AddGameObject(GameObject gameObject)
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            if (listGameObject.Contains(gameObject))
            {
                Debug.LogError(string.Format("{0}缓冲池中已经存在{1}，不需要再次添加",
                    poolName,gameObject.name));
                return;
            }
            else
            {
                if (listGameObject.Count < maxCount)
                {
                    listGameObject.Add(gameObject);
                }
                else
                {
                    GameObject firstGameObject = childGameObjectPool.transform.Find(listGameObject[0].name).gameObject;
                    UnityEngine.GameObject.DestroyImmediate(firstGameObject, true);
                    listGameObject.RemoveAt(0);
                    listGameObject.Add(gameObject);
                }
                gameObject.transform.SetParent(childGameObjectPool.transform, false);
            }
        }

        /// <summary>
        /// 获取缓冲池中的游戏对象
        /// </summary>
        /// <param name="gameObjectName">游戏对象名称</param>
        /// <returns></returns>
        public GameObject GetGameObject(string gameObjectName)
        {

            GameObject gameObject = null;
            if(childGameObjectPool.transform.Find(gameObjectName) != null) 
            {
                gameObject = childGameObjectPool.transform.Find(gameObjectName).gameObject;
                if (!listGameObject.Contains(childGameObjectPool.transform.Find(gameObjectName).gameObject))
                {
                    return null;
                }
                else
                {
                    listGameObject.Remove(gameObject);
                    listGameObject.Add(gameObject);
                }
                
            }
            return gameObject;
        }

        /// <summary>
        /// 清除缓冲池
        /// </summary>
        public void ClearListGameObject()
        {
            listGameObject.Clear();
            UnityEngine.GameObject.DestroyImmediate(childGameObjectPool, true);
            listGameObject = null;
        }
    }
}
