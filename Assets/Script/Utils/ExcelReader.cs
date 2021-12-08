using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonExcelClass {
    public string name,Column11,Column20;
    public int Column4,Column5,Column6,Column7,Column8;
    public int Column13,Column14,Column15,Column16,Column17;
    public int Column22,Column23,Column24,Column25,Column26;


}

[System.Serializable]
public class JsonExcelArray {
    public JsonExcelClass[] excelClass;
}

public class ExcelReader : MonoBehaviour {
    
    public static JsonExcelArray LoadJsonExcelClass(string text) {
        return JsonUtility.FromJson<JsonExcelArray>(text);
    }

    public static void AffectParameters(JsonExcelArray excelArray) {
        foreach(JsonExcelClass excelClass in excelArray.excelClass) {
            if(excelClass.name != "") {
                GameObject step = GameObject.Find(excelClass.name);
                if(step != null) {
                    if(step.GetComponent<Step>() != null) {
                        step.GetComponent<Step>().useVectors = new bool[]{excelClass.Column4 == 1 ? true : false,excelClass.Column5 == 1 ? true : false,excelClass.Column6 == 1 ? true : false,excelClass.Column7 == 1 ? true : false};
                        step.GetComponent<Step>().positive = excelClass.Column8 == 1 ? true : false;
                    }
                }
            }

            if(excelClass.Column11 != "") {
                GameObject step = GameObject.Find(excelClass.Column11);
                if(step != null) {
                    if(step.GetComponent<Step>() != null) {
                        step.GetComponent<Step>().useVectors = new bool[]{excelClass.Column13 == 1 ? true : false,excelClass.Column14 == 1 ? true : false,excelClass.Column15 == 1 ? true : false,excelClass.Column16 == 1 ? true : false};
                        step.GetComponent<Step>().positive = excelClass.Column17 == 1 ? true : false;
                    }
                }
            }

            if(excelClass.Column20 != "") {
                GameObject step = GameObject.Find(excelClass.Column20);
                if(step != null) {
                    if(step.GetComponent<Step>() != null) {
                        step.GetComponent<Step>().useVectors = new bool[]{excelClass.Column22 == 1 ? true : false,excelClass.Column23 == 1 ? true : false,excelClass.Column24 == 1 ? true : false,excelClass.Column25 == 1 ? true : false};
                        step.GetComponent<Step>().positive = excelClass.Column26 == 1 ? true : false;
                    }
                }
            }

            
        }
    }
}
