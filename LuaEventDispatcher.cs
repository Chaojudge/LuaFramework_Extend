using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

/// <summary>
/// 存放指定监听事件类型的所有监听事件的类
/// </summary>
public class LuaEventListener
{
    /// <summary>
    /// 用于存放需要触发的监听事件
    /// </summary>
    public Dictionary<string, LuaFunction> dicLuaFun = new Dictionary<string, LuaFunction>();
    /// <summary>
    /// 存放需要触发的监听事件所在的LuaTable
    /// </summary>
    public LuaTable luaTable;
}

/// <summary>
/// Lua的观察者模式
/// </summary>
public class LuaEventDispatcher{

    /// <summary>
    /// 用于存放游戏中所有监听事件
    /// </summary>
    private static Dictionary<string, LuaEventListener>
        dicLuaEventListener = new Dictionary<string, LuaEventListener>();

    /// <summary>
    /// 判断指定监听事件类型是否存在
    /// </summary>
    /// <param name="eventType">监听事件的类型</param>
    /// <returns></returns>
    public static bool IsExistEventTypeListener(string _eventType)
    {
        if (dicLuaEventListener.ContainsKey(_eventType))
        {
            return true;
        }
        GameDebug.Log.Error(string.Format("不存在{0}监听事件类型的监听事件",_eventType));
        return false;
    }

    /// <summary>
    /// 判断指定监听事件类型中是否存在指定监听事件
    /// </summary>
    /// <param name="_eventType">指定监听事件类型</param>
    /// <param name="_luaFunName">指定监听事件</param>
    /// <returns></returns>
    public static bool IsExistLuaFunListener(string _eventType, string _luaFunName)
    {
        if (IsExistEventTypeListener(_eventType))
        {
            if (dicLuaEventListener[_eventType].dicLuaFun.ContainsKey(_luaFunName))
            {
                return true;
            }
            GameDebug.Log.Error(string.Format("{0}监听事件类型中不存在监听事件{1}",
                _eventType,_luaFunName));
        }
        return false;
    }

    /// <summary>
    /// 添加指定监听事件类型的监听事件
    /// </summary>
    /// <param name="_eventType">指定监听事件的类型</param>
    /// <param name="_luaFunName">指定监听事件名称</param>
    /// <param name="_luaFun">指定监听事件</param>
    /// <param name="_luaTable">需要触发的监听事件所在的LuaTable</param>
    public static void AddListener(string _eventType, string _luaFunName, LuaFunction _luaFun, LuaTable _luaTable)
    {
        if (!IsExistEventTypeListener(_eventType))
        {
            LuaEventListener luaEventListener = new LuaEventListener
            {
                luaTable = _luaTable
            };
            luaEventListener.dicLuaFun.Add(_luaFunName, _luaFun);
            dicLuaEventListener.Add(_eventType, luaEventListener);
        }

        LuaEventListener _luaEventListener = dicLuaEventListener[_eventType];

        if (_luaEventListener != null && !_luaEventListener.dicLuaFun.ContainsKey(_luaFunName))
        {
            dicLuaEventListener[_eventType].dicLuaFun.Add(_luaFunName, _luaFun);
        }
    }

    /// <summary>
    /// 移除指定监听事件类型的所有监听事件
    /// </summary>
    /// <param name="_eventType">监听事件类型</param>
    /// <returns></returns>
    public static bool RemoveEventTypeListener(string _eventType)
    {
        if (IsExistEventTypeListener(_eventType))
        {
            dicLuaEventListener[_eventType].luaTable.Dispose();
            foreach (KeyValuePair<string, LuaFunction> luaFunItem 
                in dicLuaEventListener[_eventType].dicLuaFun)
            {
                luaFunItem.Value.Dispose();
            }
            dicLuaEventListener.Remove(_eventType);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除指定监听事件类型中指定监听事件
    /// </summary>
    /// <param name="_eventType">监听事件类型</param>
    /// <param name="_luaFunName">指定监听事件名</param>
    /// <returns></returns>
    public static bool RemoveLuaFunListener(string _eventType, string _luaFunName)
    {
        if (IsExistLuaFunListener(_eventType, _luaFunName))
        {
            dicLuaEventListener[_eventType].dicLuaFun[_luaFunName].Dispose();
            dicLuaEventListener[_eventType].dicLuaFun.Remove(_luaFunName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 触发指定监听事件类型中指定监听的事件
    /// </summary>
    /// <param name="_eventType">监听事件类型</param>
    /// <param name="_luaFunName">指定监听的事件名</param>
    /// <param name="_luaTable">数据</param>
    public static void TriggerListener(string _eventType, string _luaFunName, LuaTable _luaTable)
    {
        if(IsExistLuaFunListener(_eventType, _luaFunName))
        {
            dicLuaEventListener[_eventType].dicLuaFun[_luaFunName].Call(
                dicLuaEventListener[_eventType].luaTable,_luaTable);
        }
    }
}
