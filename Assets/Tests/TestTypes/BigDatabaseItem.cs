using System.Collections;
using System.Collections.Generic;
using Rhinox.Lightspeed;
using Rhinox.Vortex;
using Rhinox.Vortex.File;
using Sirenix.OdinInspector;
using UnityEngine;

public class BigDatabaseItem
{
    public int Id;

    public SerializableGuid Guid;

    public BigDatabaseItem()
    {
        Guid = SerializableGuid.CreateNew();
    }

    public BigDatabaseItem(int id) : this()
    {
        Id = id;
    }
}

[DataEndPoint(typeof(FileEndPoint), -10)]
public class BigDT : XmlFileDT<BigDatabaseItem>
{
    protected override string _tableName => "BigTable";

    protected override int GetID(BigDatabaseItem dto) => dto.Id;
}