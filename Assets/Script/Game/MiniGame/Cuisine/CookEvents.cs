using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookEvents : MonoBehaviour
{

    public EventHandler<OnUpdateBoxStockArgs> OnUpdateBoxStock;
    public class OnUpdateBoxStockArgs : EventArgs
    {
        public GameObject stock;
        public BasicBox box;
    }
}
