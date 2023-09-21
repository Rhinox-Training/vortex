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

public class SimpleTypeDT : DataTable<SimpleType>
{
    protected override string _tableName => "SimpleObjects";

    protected override int GetID(SimpleType dto) => dto.Id;
    
    protected override SimpleType SetID(SimpleType dto, int id)
    {
        dto.Id = id;
        return dto;
    }
}