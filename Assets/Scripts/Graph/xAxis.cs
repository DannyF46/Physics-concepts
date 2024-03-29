using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;

public class xAxis : Axis
{
    
    public override void Start()
    {
        base.Start();
        SetTicks(labelCount,Vector2.right);      
        SetTickLabels(labelCount, Vector2.right);
        MakeAxis(labelCount,Vector2.right);

    }
   
   
}
