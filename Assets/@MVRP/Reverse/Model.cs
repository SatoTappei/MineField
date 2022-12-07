using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Model : MonoBehaviour
{
    public int Value { get; }

    public UnityAction<int> OnValueChanged { get; set; }
}
