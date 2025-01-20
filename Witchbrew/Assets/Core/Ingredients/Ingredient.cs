using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ingredient
{
    public ingredient type;
    public preperation prep;
    public enum ingredient
    {
        None,
        Herbs,
        Mushrooms,
        Eyeballs,
        Bones,
        Scales
    }


    public enum preperation
    {
        raw,
        grinded,
        sliced,
        roasted

    }
}
