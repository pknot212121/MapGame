using UnityEngine;

[System.Serializable]
public class Action : Entity
{
    public enum ActionType
    {
        RaiseTroop,
        MoveTroop,
        BuyProvince,
    }
    public ActionType type;
    public Entity entity1;
    public Entity entity2;
    public string optionalData;

    public Action(){}
    public Action(ActionType type, Entity entity1, Entity entity2, string optionalData)
    {
        this.type = type;
        this.entity1 = entity1;
        this.entity2 = entity2;
        this.optionalData = optionalData;
    }
}
