using System.Collections;
using System.Collections.Generic;
using Rhinox.Vortex;
using Rhinox.Vortex.File;
using UnityEngine;

public class SimpleType
{
    public int Id;
    public string Name;
}

[DataEndPoint(typeof(FileEndPoint), -10)]
public class SimpleTypeDT : XmlFileDT<SimpleType>
{
    protected override string _tableName => "SimpleObjects";

    protected override int GetID(SimpleType dto) => dto.Id;
}