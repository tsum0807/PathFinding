using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InverseMask : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material newMaterial = new Material(base.materialForRendering);
            newMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return newMaterial;
        }
    }
}
