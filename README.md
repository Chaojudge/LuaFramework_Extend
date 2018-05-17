LuaMonoBehaviour.cs的原理：

	1.先将该类注册到Lua虚拟机中，以便于在Lua中调用该类的静态方法
	2.在lua中先初始化LuaMonoBehaviour的静态字段tempLuaTable。
	3.之后将LuaMonoBehaviour挂载到指定的物体上。
	4.物体挂载脚本后会直接运行生命周期函数
	5.在Awake方法中将tempLuaTable赋值给luaTable后将tempLuaTable置空
	6.在脚本被销毁时OnDestroy方法中要Disponse掉脚本中所有的LuaTable以及LuaFunction
	7.而FixedUpdate、Update以及LateUpdate可以在.lua中FixedUpdateBeat:Add()方法来添
	加，具体详细看BaseMonoBehaviour.lua脚本。

	-- 添加驱动指定Lua的脚本[functions.lua中]
	function AddBehaviour(gameObject,tablePath)
	    LuaMonoBehaviour.tempLuaTable = require(tablePath).new();
	    local table = LuaMonoBehaviour.tempLuaTable;
	    gameObject:AddComponent(typeof(LuaMonoBehaviour));
	    return table;
	end

LuaEventDispatcher.cs原理：
	本人之前所了解的C#中观察者模式有两种，一种是需要Object拆箱装箱来传递参数的，另一种是使用泛型的，而我想要在Lua中使用观察者模式，Lua无法使用泛型，那么只能只用Object拆箱装箱的方法，但是后面发现可以参考我自己扩的LuaMonoBehaviour.cs中驱动Lua脚本以及Lua脚本中函数的特点，我自己写了一个有类似特点的观察者模式，传递参数可以直接传LuaTable，举个例子，例如：在PackPanel.lua中监听金币增加或减少的事件：
	
	--在UI界面初始化时添加和触发观察者监听事件[.lua中]：
	PackPanel.lua = class("PackPanel.lua",BasePanelBehaviour);

	function PackPanel:Awake(obj)
		self.super.Awake(self.super,self,obj);
	end

	-- 初始化界面元素
	function PackPanel:InitUIOnAwake()
		--[[ 
		functions.lua的SubGet方法修改成：
			function subGet(parentTrans,childTrans, typeName)		
				return parentTrans:Find(childTrans):GetComponent(typeName);
			end
		--]] 
		self.coinCount = subGet(self.transform,"CoinCount","Text");
	    	self.addCoinButton = subGet(self.transform,"AddCoinButton","Button");
    		self.subCoinButton = subGet(self.transform,"SubCoinButton","Button");
	end

	function PackPanel:AddListener()
		-- 添加监听事件
		LuaEventDispatcher.AddListener("Coin","Add",self.AddCoin,self);
		LuaEventDispatcher.AddListener("Coin","Sub",self.SubCoin,self);
	end

	function PackPanel:AddCoin(table)
		-- 增加金币的UI逻辑
		self.coinCount.text = self.coinCount.text + table.coin;
	end

	function PackPanel:SubCoin(table)
		-- 减少金币的UI逻辑
		self.coinCount.text = self.coinCount.text - table.coin;
	end

	-- 点击添加金币按钮的事件
	function PackPanel:AddCoinButton_OnClick()
		local table = {};
    		table.coin = 10;
    		-- 触发添加金币的监听事件
    		LuaEventDispatcher.TriggerListener("Coin","Add",table);
	end

	-- 点击减少金币按钮的事件
	function PackPanel:SubCoinButton_OnClick()
	   	local table = {};
	    	table.coin = 10;
	    	-- 触发减少金币的监听事件
	    	LuaEventDispatcher.TriggerListener("Coin","Sub",table);
	end

	function PackPanel:OnDestroy()
		-- 移除指定事件类型的所有监听事件
		LuaEventDispatcher.RemoveEventTypeListener("Coin");
	end

	return PackPanel;
