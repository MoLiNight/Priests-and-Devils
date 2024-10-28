using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMessage : MonoBehaviour
{
    public string role;
    public int sequenceIndex;
    public GameObject roleModel;
    public int rolePosition = 1;

    public RoleMessage(int index, GameObject gameObject, string str)
    {
        role = str;
        sequenceIndex = index;
        roleModel = gameObject;
    }
}
