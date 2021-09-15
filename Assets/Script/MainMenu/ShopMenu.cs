using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShopMenu", menuName = "Shop/ShopMenu", order = 1)]
public class ShopMenu : ScriptableObject
{
    public string name;
    public int couronnes;
    public int coins;
    public string description;
}
