using System;
using System.Collections;
using System.Collections.Generic;
using Rhinox.GUIUtils.Attributes;
using Rhinox.GUIUtils.Odin;
using Rhinox.Lightspeed;
using Rhinox.Vortex;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable, ValueTypeAsTitle]
public class Test
{
    
}

// [Title("@$value.GetType().Name")]
[ValueTypeAsTitle]
public class TablePrinter : MonoBehaviour
{
    [ValueTypeAsTitle]
    public int Test;

    public Test Test2;
    
    private void Start()
    {
        Print();
    }

    [Button]
    private void TestEditor()
    {
        Print();
    }
    
    private static void Print()
    {
        var dt = DataLayer.GetTable<SimpleType>();
        foreach (var dto in dt.GetAllData())
            Debug.Log(dto.Name);
    }

    [Button, ShowIf("@DataLayer.IsInitialized")]
    private void CleanUp()
    {
        DataLayer.Shutdown();
    }

}
