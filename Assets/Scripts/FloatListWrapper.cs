using System;
using System.Collections.Generic;

// A class created to wrap a list with the purpose of making it serializable to JSON.
[Serializable]
public class FloatListWrapper
{
    public List<float> floatList;

    public FloatListWrapper(List<float> floatList)
    {
        this.floatList = floatList;
    }
}
