using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOwner : MonoBehaviour
{
    public bool available = true;

    public virtual bool CheckCanLoad(Commodity _commodity)
    {
        return true;
    }

}
