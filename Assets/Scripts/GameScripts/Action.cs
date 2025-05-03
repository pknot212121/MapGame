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
    public int id1;
    public int id2;
    public string optionalData;

    public Action(){}
    public Action(ActionType type, int id1, int id2, string optionalData)
    {
        this.type = type;
        this.id1 = id1;
        this.id2 = id2;
        this.optionalData = optionalData;
    }
}
