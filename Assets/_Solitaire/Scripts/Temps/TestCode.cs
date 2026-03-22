using System;
using _Solitaire.Scripts.GameInput;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    [SerializeField] private InputController inputController;

    private void Start()
    {
        inputController.OnPointerDown += () => Debug.Log("Pointer Down");
        inputController.OnPointerUp += () => Debug.Log("Pointer Up");
    }
}
