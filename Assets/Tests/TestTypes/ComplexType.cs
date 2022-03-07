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

[DataEndPoint(typeof(FileEndPoint), -10)]
public class ComplexTypeDT : XmlFileDT<ComplexType>
{
    protected override string _tableName => "ComplexObjects";

    protected override int GetID(ComplexType dto) => dto.Id;
}