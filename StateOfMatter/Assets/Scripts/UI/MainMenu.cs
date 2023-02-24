using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [field:SerializeField] public Transform SelectionVisualizer { get; private set; }
    [field:SerializeField] public Vector3 DefaultOffset { get; private set; }

    void Start()
    {
        
    }
    void Update()
    {

    }

    public void MoveToButton(Transform buttonTransform)
    {
        //Sets position of SelectionVisualizer to button's position, with an offset to the left
        SelectionVisualizer.transform.position = buttonTransform.position + DefaultOffset;
    }
}
