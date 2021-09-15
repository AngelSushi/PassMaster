using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItem", order = 1)]
public class ShopItem : ScriptableObject {

    public string name;
    public string description;
    public int price;
    public Sprite img;
    public Vector3 position;
    public Vector2 dimension;

}
