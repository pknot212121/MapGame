using UnityEngine;

public class Entity
{
    public int id = 0;
    public virtual void Pack(){} // Spakowanie do wysyłki
    public virtual void Unpack(){} // Rozpakowanie po otrzymaniu
}
