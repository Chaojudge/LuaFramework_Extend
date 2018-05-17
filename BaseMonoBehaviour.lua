BaseMonoBehaviour=class("BaseMonoBehaviour");

function BaseMonoBehaviour:Awake(subClass,obj)
    subClass.gameObject = obj;
	subClass.transform = obj.transform;
	subClass.Behaviour = obj.transform:GetComponent(typeof(LuaMonoBehaviour));
end

function BaseMonoBehaviour:OnEnable(subClass)
    self:AddListener(subClass);
end

function BaseMonoBehaviour:OnDisable(subClass)
    self:RemoveListener(subClass);
end

function BaseMonoBehaviour:AddListener(subClass)
    if subClass ~= nil then
        if subClass.FixedUpdate ~= nil then
            FixedUpdateBeat:Add(subClass.FixedUpdate,subClass);
        end
        if subClass.Update ~= nil then
            UpdateBeat:Add(subClass.Update,subClass);
        end
        if subClass.LateUpdate ~= nil then
		    LateUpdateBeat:Add(subClass.LateUpdate,subClass)
	    end
    end
end

function BaseMonoBehaviour:RemoveListener(subClass)
    if subClass ~= nil then
        if subClass.FixedUpdate ~= nil then
		    FixedUpdateBeat:Remove(subClass.FixedUpdate,subClass)
	    end
        if subClass.Update ~= nil then
            UpdateBeat:Remove(subClass.Update,subClass);
        end
        if subClass.LateUpdate ~= nil then
		    LateUpdateBeat:Remove(subClass.LateUpdate,subClass)
	    end
    end
end