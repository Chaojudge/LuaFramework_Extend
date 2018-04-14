# LuaFramework_Extend
驱动Lua脚本的类：LuaMonoBahaviour
扩充LuaFramwework，以便所有Unity3D的逻辑全用Lua来写，这里提供思路和代码

LuaMonoBehaviour使用方法：

1.在Tolua中的LuaFramework/Editor/CustomSettings.cs注册LuaMonoBehaviour并绑定
2.在挂载LuaMonoBehaviour之前，Lua中需要先给LuaMonoBehaviour.tempLuaTable赋值后挂载脚本：

function GameTool.AddBehaviour(gameObject,tablePath)

    LuaMonoBehaviour.tempLuaTable = require(tablePath).new();
    
    local table = LuaMonoBehaviour.tempLuaTable;
    
    gameObject:AddComponent(typeof(LuaMonoBehaviour));
    
    return table;
    
end

