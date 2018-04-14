using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

/// <summary>
/// 1.先将该类注册或绑定到lua中，以便于在Lua中调用该类的静态方法
/// 2.在lua中先初始化LuaMonoBehaviour的静态字段tempLuaTable。
/// 3.之后将LuaMonoBehaviour挂载到指定的物体上。
/// 注意：1~3的具体方法在Tool/GameTool.lua的AddBehaviour中
/// 4.物体挂载脚本后会直接运行生命周期函数
/// 5.在Awake方法中将tempLuaTable赋值给luaTable后将tempLuaTable置空
/// 注意：不需要新建LuaState，在LuaManager脚本运行时就已经创建了
/// 在使用protobuf时，可以修改ByteBuffer.cs中的WriteBuffer方法，将byte[]数组拼接成
/// string来输出数据，在服务端中接收数据byte[]拼接成string后输出，比对数据是否相同，
/// 如果相同则可以在服务端中将接收到的数据进行切割，获取消息区域的内容并进行proto反
/// 序列化成类：具体方法在ServerSocket.cs的AsyncReceive()方法中
/// 注意：如果服务端是C/C++写的话需要大小端转换
/// </summary>
public class LuaMonoBehaviour : MonoBehaviour
{
    //Update方法在lua中使用 UpdateBeat:Add(Update, self)
    public static LuaTable tempLuaTable;
    private LuaTable luaTable;
    private Dictionary<string, LuaFunction> dicFun;
    private Dictionary<Button, LuaFunction> dicClickFun;
    private Dictionary<Toggle, LuaFunction> dicValueOnChangeFun;

    /// <summary>
    /// 执行lua中的生命周期函数
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="obj"></param>
    private void Awake()
    {
        luaTable = tempLuaTable;
        tempLuaTable = null;
        dicFun = new Dictionary<string, LuaFunction>();
        dicClickFun = new Dictionary<Button, LuaFunction>();
        dicValueOnChangeFun = new Dictionary<Toggle, LuaFunction>();
        if (luaTable != null)
        {
            AddMethod("Awake");
            AddMethod("OnEnable");
            AddMethod("Start");
            AddMethod("OnTriggerEnter");
            AddMethod("OnTriggerStay");
            AddMethod("OnTriggerExit");
            AddMethod("OnCollisionEnter");
            AddMethod("OnCollisionStay");
            AddMethod("OnCollisionExit");
            AddMethod("OnDisable");
            AddMethod("OnDestroy");
        }
        if (dicFun.ContainsKey("Awake"))
        {
            dicFun["Awake"].Call(luaTable,gameObject);
        }
    }

    private void AddMethod(string methodName)
    {
        if (luaTable.GetLuaFunction(methodName) != null)
        {
            dicFun.Add(methodName, luaTable.GetLuaFunction(methodName));
        }
    }

    private void OnEnable()
    {
        if (dicFun.ContainsKey("OnEnable"))
        {
            dicFun["OnEnable"].Call(luaTable);
        }
    }

    private void Start()
    {
        if (dicFun.ContainsKey("Start"))
        {
            dicFun["Start"].Call(luaTable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dicFun.ContainsKey("OnTriggerEnter"))
        {
            dicFun["OnTriggerEnter"].Call(luaTable, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (dicFun.ContainsKey("OnTriggerStay"))
        {
            dicFun["OnTriggerStay"].Call(luaTable, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (dicFun.ContainsKey("OnTriggerExit"))
        {
            dicFun["OnTriggerExit"].Call(luaTable, other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (dicFun.ContainsKey("OnCollisionEnter"))
        {
            dicFun["OnCollisionEnter"].Call(luaTable, collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (dicFun.ContainsKey("OnCollisionStay"))
        {
            dicFun["OnCollisionStay"].Call(luaTable, collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (dicFun.ContainsKey("OnCollisionExit"))
        {
            dicFun["OnCollisionExit"].Call(luaTable, collision);
        }
    }

    private void OnDisable()
    {
        try
        {
            if (dicFun.ContainsKey("OnDisable"))
            {
                dicFun["OnDisable"].Call(luaTable);
            }
        }
        catch (System.Exception){ }
    }

    private void OnDestroy()
    {
        try
        {
            if (dicFun.ContainsKey("OnDestroy"))
            {
                RemoveAllClick();
                RemoveAllValueChange();
                dicFun["OnDestroy"].Call(luaTable);
                dicFun["OnDestroy"].Dispose();
            }
            foreach (KeyValuePair<string, LuaFunction> item in dicFun)
            {
                if (!item.Key.Equals("OnDestroy"))
                {
                    item.Value.Dispose();
                }
            }
            luaTable.Dispose();
            luaTable = null;
        }
        catch (System.Exception){ }
    }

    /// <summary>
    /// 添加点击按钮监听事件
    /// </summary>
    /// <param name="button">按钮</param>
    /// <param name="clickFun">函数</param>
    public void AddClick(Button button, LuaFunction clickFun)
    {
        if (button == null || clickFun == null) return;
        if (!dicClickFun.ContainsKey(button))
        {
            dicClickFun.Add(button, clickFun);
            button.onClick.AddListener(delegate () {
                dicClickFun[button].Call(luaTable);
            });
        }
    }

    /// <summary>
    /// 移除所有按钮点击监听事件
    /// </summary>
    private void RemoveAllClick()
    {
        foreach (KeyValuePair<Button, LuaFunction> item in dicClickFun)
        {
            item.Value.Dispose();
            dicClickFun[item.Key] = null;
        }
        dicClickFun.Clear();
    }

    /// <summary>
    /// 添加Toggle监听事件
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="valueChangeFun"></param>
    public void AddValueChange(Toggle toggle, LuaFunction valueChangeFun)
    {
        if (!dicValueOnChangeFun.ContainsKey(toggle))
        {
            dicValueOnChangeFun.Add(toggle, valueChangeFun);
            toggle.onValueChanged.AddListener(delegate (bool value) {
                dicValueOnChangeFun[toggle].Call(luaTable, value);
            });
        }
    }

    /// <summary>
    /// 移除所有Toggle监听事件
    /// </summary>
    private void RemoveAllValueChange()
    {
        foreach (KeyValuePair<Toggle, LuaFunction> item in dicValueOnChangeFun)
        {
            item.Value.Dispose();
            dicValueOnChangeFun[item.Key] = null;
        }
        dicValueOnChangeFun.Clear();
    }
}
