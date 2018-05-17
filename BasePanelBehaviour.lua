BasePanelBehaviour=class("BasePanelBehaviour",BaseMonoBehaviour);

function BasePanelBehaviour:Awake(subClass,obj)
    self.super:Awake(subClass,obj);	
    if subClass.InitUIOnAwake ~= nil then
        subClass:InitUIOnAwake();
    end
    if subClass.InitDataOnAwake ~= nil then
        subClass:InitDataOnAwake();
    end
    if subClass.AddListener ~= nil then
        subClass:AddListener();
    end
end

return BasePanelBehaviour;
