# LuaFramework_Extend
驱动Lua脚本的类：LuaMonoBahaviour
扩充LuaFramwework，以便所有Unity3D的逻辑全用Lua来写，这里提供思路和代码

LuaMonoBehaviour使用方法：

1.在Tolua中的LuaFramework/Editor/CustomSettings.cs注册LuaMonoBehaviour并绑定
2.在挂载LuaMonoBehaviour之前，Lua中需要先给LuaMonoBehaviour.tempLuaTable赋值后挂载脚本：
3.在LuaMonoBehaviour脚本生命周期函数Awake中对luaTable进行初始化并将tempLuaTable置空
4.由于生命周期函数OnDestroy中将每个生命周期函数调用的LuaFunction释放掉，因此需要使用字典保存各个LuaFunction
5.注意：FixedUpdate、Update、LateUpdate不要在C#中驱动，尽可能在Lua中使用:

    FixedUpdateBeat:Add()
    UpdateBeat:Add()
    LateUpdateBeat:Add()

6.使用方法，注意需要模拟面向对象的元表：https://blog.csdn.net/honey199396/article/details/50888063


    LuaMonoBehaviour.tempLuaTable = require(tablePath).new();
    local table = LuaMonoBehaviour.tempLuaTable;
    gameObject:AddComponent(typeof(LuaMonoBehaviour));
    return table;
