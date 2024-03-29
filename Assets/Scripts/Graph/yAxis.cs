using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class yAxis : Axis
{
    public override void Start()
    {
        base.Start();
        SetTicks(labelCount, Vector2.up);
        SetTickLabels(labelCount, Vector2.up);
        MakeAxis(labelCount,Vector2.up);
    }

   
}

