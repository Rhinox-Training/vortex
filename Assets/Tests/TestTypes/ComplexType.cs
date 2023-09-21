using System;
using System.Collections;
using System.Collections.Generic;
using Rhinox.Vortex;
using Rhinox.Vortex.File;
using UnityEngine;

public class ComplexType
{
    [Serializable]
    public class NestedComplexType
    {
        public string Name;
        public NestedComplexType Nested;
    }

    public int Id;
    public string Name;

    public NestedComplexType Nested;
}

public class ComplexTypeDT : DataTable<ComplexType>
{
    protected override string _tableName => "ComplexObjects";

    protected override int GetID(ComplexType dto) => dto.Id;
    protected override ComplexType SetID(ComplexType dto, int id)
    {
        dto.Id = id;
        return dto;
    }
}