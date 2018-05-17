一 CfgData是读取配置表.txt的类库，其内容格式和.CSV格式一样，原本读取配置表的方法是带有泛型的，而Lua中有userdata类型，
为了方便在lua中使用，对原先的读取配置表的方法稍做修改，具体详细看Cfg.cs，例如testCfg.txt的内容为：
	
	ID,GoodName,Price,limitLevel
	//物品ID,//物品名称,//物品价格,//使用物品的等级限制
	-1,-1,-1,-1
	1001,低级药草,50,1
	1002,低级药水,100,1
	1003,较多杂质的铁矿,10,1
	1004,破损的牛皮,20,1
	1005,粗糙的树枝,30,1
	1006,低级磨具,100,5

  读取testCfg.txt配置表的具体方法：

	-- 注意配置表文件必须为.txt格式且字符集为UTF8且必须存放在Resources/Cfg文件夹中
	-- 注意要在customSetting.cs中_GT(typeof(Dictionary<string, Dictionary<string, string>>))
	local testCfg = CfgData.Cfg.LoadCfgDataFormCSV("testCfg");	--读取配置表
	log(CfgData.Cfg.ReadCfg("GoodName",1002,testCfg));		--读取指定ID的物品名称

二 CfgFileCreateEditor是CSV文件编辑器扩展的类库，其包含以下功能：
	
	1.CSV文件创建，以便于编辑配置表
	2.批量或单个CSV转换成指定字符集格式的TXT文件，即在Project面板中选择文件夹
	即可批量转换，选择文件即可单个转换
	3.批量或单个TXT文件转换成CSV文件，即在Project面板中选择文件夹即可批量转换，
	选择文件即可单个转换

三.LuaScriptEditor是Lua文件编辑器扩展的类库，可创建指定内容格式，指定字符集格式的Lua文件
