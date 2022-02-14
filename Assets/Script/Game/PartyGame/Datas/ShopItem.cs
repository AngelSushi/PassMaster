using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItem", order = 1)]
public class ShopItem : ScriptableObject {

    public string name;
    public Sprite sprite;
    public int price;
    public int maxQuantity;
}
