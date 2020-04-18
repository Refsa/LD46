using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] HandUI handUI;

    static UIManager instance;

    public static HandUI HandUI;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        HandUI = handUI;
    }
}
