using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [SerializeField]
    Match3Skin match3;

    InputAction position;
    InputAction action;

    //Dragging
    Vector3 dragStart;
    bool isDragging;

    //Input
    InputHandler input;
    private void Awake()
    {
        input = GetComponent<InputHandler>();
        // Game is started via StartMenuUI, not here
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (match3.isPlaying)
        {
            if (!match3.isBusy)
            {
                HandleInput();
            }
            match3.DoWork();
        }
    }

    private void HandleInput() 
    {
        if (!isDragging && input.WasClickedThisFrame())
        {
            dragStart = input.getPosition();
            isDragging = true;
        }
        else if (isDragging && input.isClicked())
        {
            isDragging = match3.EvaluateDrag(dragStart, input.getPosition());
        }
        else
        {
            isDragging = false;
        }
    } 
}
