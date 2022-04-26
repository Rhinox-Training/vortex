using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rhinox.Lightspeed;
using Sirenix.OdinInspector;
using SQLite;
using UnityEngine;

public class DbTest : MonoBehaviour
{

    [Button]
    void Test()
    {
        // Get an absolute path to the database file
        var root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var databasePath = Path.Combine(root, "TestData.db");

        var db = new SQLiteConnection(databasePath);
        var dbItemMapping = db.GetMapping(typeof(BigDatabaseItem), CreateFlags.None);
        
        db.CreateTable<BigDatabaseItem>();

            
        db.Insert(new BigDatabaseItem
        {
            Id = 0,
            Guid = SerializableGuid.CreateNew()
        });
    }
}
