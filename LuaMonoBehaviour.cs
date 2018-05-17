using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

public class LuaMonoBehaviour : MonoBehaviour {

    public static LuaTable tempLuaTable;
    private LuaTable luaTable;
    private Dictionary<string, LuaFunction> dicLuaFun;
    private Dictionary<Button, LuaFunction> dicBtnClickFun;
    private Dictionary<Toggle, LuaFunction> dicToggleValueChangeFun;

    private void AddMethod(string luaFunName)
    {
        if (!dicLuaFun.ContainsKey(luaFunName))
        {
            dicLuaFun.Add(luaFunName, luaTable.GetLuaFunction(luaFunName));
        }
    }

    private void Awake()
    {
        luaTable = tempLuaTable;
        tempLuaTable = null;
        if (luaTable != null)
        {
            if (dicLuaFun == null)
            {
                dicLuaFun = new Dictionary<string, LuaFunction>();
            }
            AddMethod("Awake");
        }
        if (dicLuaFun["Awake"] != null)
        {
            dicLuaFun["Awake"].Call(luaTable, gameObject);
        }
    }

    private void OnEnable()
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnEnable"))
            {
                AddMethod("OnEnable");
            }
            if (dicLuaFun["OnEnable"] != null)
            {
                dicLuaFun["OnEnable"].Call(luaTable);
            }
        }
    }

    private void Start()
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("Start"))
            {
                AddMethod("Start");
            }
            if (dicLuaFun["Start"] != null)
            {
                dicLuaFun["Start"].Call(luaTable);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnTriggerEnter"))
            {
                AddMethod("OnTriggerEnter");
            }
            if (dicLuaFun["OnTriggerEnter"] != null)
            {
                dicLuaFun["OnTriggerEnter"].Call(luaTable, other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnTriggerExit"))
            {
                AddMethod("OnTriggerExit");
            }
            if (dicLuaFun["OnTriggerExit"] != null)
            {
                dicLuaFun["OnTriggerExit"].Call(luaTable, other);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnTriggerStay"))
            {
                AddMethod("OnTriggerStay");
            }
            if (dicLuaFun["OnTriggerStay"] != null)
            {
                dicLuaFun["OnTriggerStay"].Call(luaTable, other);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnCollisionEnter"))
            {
                AddMethod("OnCollisionEnter");
            }
            if (dicLuaFun["OnCollisionEnter"] != null)
            {
                dicLuaFun["OnCollisionEnter"].Call(luaTable, collision);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnCollisionStay"))
            {
                AddMethod("OnCollisionStay");
            }
            if (dicLuaFun["OnCollisionStay"] != null)
            {
                dicLuaFun["OnCollisionStay"].Call(luaTable, collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnCollisionExit"))
            {
                AddMethod("OnCollisionExit");
            }
            if (dicLuaFun["OnCollisionExit"] != null)
            {
                dicLuaFun["OnCollisionExit"].Call(luaTable, collision);
            }
        }
    }

    private void OnDisable()
    {
        if (luaTable != null)
        {
            if (!dicLuaFun.ContainsKey("OnDisable"))
            {
                AddMethod("OnDisable");
            }
            if (dicLuaFun["OnDisable"] != null)
            {
                dicLuaFun["OnDisable"].Call(luaTable);
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (luaTable != null)
            {
                if (!dicLuaFun.ContainsKey("OnDestroy"))
                {
                    AddMethod("OnDestroy");
                }
                if (dicLuaFun["OnDestroy"] != null)
                {
                    dicLuaFun["OnDestroy"].Call(luaTable);
                }
                RemoveAllClick();
                RemoveAllValueChange();
                foreach (KeyValuePair<string, LuaFunction> dicLuaFunItem in dicLuaFun)
                {
                    dicLuaFun[dicLuaFunItem.Key].Dispose();
                    dicLuaFun.Remove(dicLuaFunItem.Key);
                }
            }
        }
        catch (System.Exception) { }
    }

    public void AddClick(Button button, LuaFunction luaFun)
    {
        if (button == null || luaFun == null) return;
        if (dicBtnClickFun == null)
        {
            dicBtnClickFun = new Dictionary<Button, LuaFunction>();
        }
        if (!dicBtnClickFun.ContainsKey(button))
        {
            dicBtnClickFun.Add(button, luaFun);
        }
        else
        {
            dicBtnClickFun[button].Dispose();
            dicBtnClickFun[button] = luaFun;
        }
        button.onClick.AddListener(delegate ()
        {
            dicBtnClickFun[button].Call(luaTable);
        });
    }

    public void RemoveAllClick()
    {
        foreach (KeyValuePair<Button, LuaFunction> item in dicBtnClickFun)
        {
            item.Value.Dispose();
            item.Key.onClick.RemoveAllListeners();
        }
        dicBtnClickFun.Clear();
    }

    public void AddValueChange(Toggle toggle, LuaFunction luaFun)
    {
        if (toggle == null || luaFun == null) return;
        if (!dicToggleValueChangeFun.ContainsKey(toggle))
        {
            dicToggleValueChangeFun.Add(toggle, luaFun);
        }
        else
        {
            dicToggleValueChangeFun[toggle].Dispose();
            dicToggleValueChangeFun[toggle] = luaFun;
        }
        toggle.onValueChanged.AddListener(delegate (bool value) 
        {
            dicToggleValueChangeFun[toggle].Call(luaTable, value);
        });
    }

    public void RemoveAllValueChange()
    {
        foreach (KeyValuePair<Toggle, LuaFunction> item in dicToggleValueChangeFun)
        {
            item.Value.Dispose();
            item.Key.onValueChanged.RemoveAllListeners();
        }
        dicToggleValueChangeFun.Clear();
    }
}
