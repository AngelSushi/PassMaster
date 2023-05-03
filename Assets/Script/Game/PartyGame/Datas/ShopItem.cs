using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItem", order = 1)]
public class ShopItem : ScriptableObject {

    public string name;
    public Sprite sprite;
    public Vector2 spriteScale;
    public int price;
    public int maxQuantity;
    public ItemType itemType;
}

