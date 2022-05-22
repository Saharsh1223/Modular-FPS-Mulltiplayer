using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapons : Item
{
    public abstract override void Use();
    
    public GameObject bulletImpactPrefab;
}
