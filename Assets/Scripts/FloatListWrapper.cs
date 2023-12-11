using System;
using System.Collections.Generic;

[Serializable]
public class FloatListWrapper
{
    public List<float> floatList;
    public FloatListWrapper(List<float> floatList)
    {
        this.floatList = floatList;
    }
}

