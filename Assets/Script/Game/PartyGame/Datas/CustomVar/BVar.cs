using UnityEngine;

public class BVar  {
    
    public bool value;
    public bool lastValue;

    public SwitchValuePositive switchValuePositive;
    public SwitchValueNegative switchValueNegative;
    
    public void UpdateValues() {
        if (value && !lastValue) 
            switchValuePositive();
        
        else if (!value && lastValue)
            switchValueNegative();

        lastValue = value;
    }

    public delegate void SwitchValuePositive(); // Called When value is true and lastValue is false
    public delegate void SwitchValueNegative(); // Called when value is false and lastValue is true
    

    public static bool operator !(BVar a) {
        return !a.value;
    }

    public static bool operator true(BVar a) {
        return a.value;
    }
    
    public static bool operator false(BVar a) {
        return !a.value;
    }
    
    public static bool operator &(bool a,BVar b) {
        return a && b.value;
    }

    public static bool operator |(bool a, BVar b) {
        return a || b.value;
    }

    public static bool operator |(BVar a, BVar b) {
        return a.value || b.value;
    }
}
