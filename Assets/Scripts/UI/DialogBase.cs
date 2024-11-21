using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
    public Canvas DialogCanvas;
    public virtual RTSMenus MenuType()
    {
        return RTSMenus.UnderFined;
    }
}
