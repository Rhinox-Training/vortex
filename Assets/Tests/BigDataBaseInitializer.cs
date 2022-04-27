using System.Collections;
using System.Collections.Generic;
using Rhinox.Vortex;
using Sirenix.OdinInspector;
using UnityEngine;

public class BigDataBaseInitializer : MonoBehaviour
{
    [Button]
    private void InitTwoHunderdItems()
    {
        var table = DataLayer.GetTable<BigDatabaseItem>();
        while (table.Count < 200)
            table.StoreData(new BigDatabaseItem(table.GetNewID()));
    }
}
