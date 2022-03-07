using System.Collections;
using System.Collections.Generic;
using Rhinox.Vortex;
using Sirenix.OdinInspector;
using UnityEngine;

public class TablePrinter : MonoBehaviour
{
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

}
