using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Commodity currentCommodity;
    public bool dragging;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too may {this} instances");
        }
    }

    private void FixedUpdate() {
        if (dragging)
        {
            
        }
    }

    public void SetDrag(bool _isDragging)
    {
        dragging = _isDragging;
    }
    
    public void SetCommodity(Commodity _commodity)
    {
        currentCommodity = _commodity;
    }
}
