using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Checks for input and passes it to the active PillHolder.
public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Button buttonLeft;
    [SerializeField]
    private Button buttonDown;
    [SerializeField]
    private Button buttonRight;
    [SerializeField]
    private Button buttonClockwise;
    [SerializeField]
    private Button buttonCounterClockwise;

    private GameManager gameManager;

    void Awake()
    {
		// Could be done a bit nicer I suppose.
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnLeftPressed()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.LeftPressed();
        }
    }

    public void OnLeftReleased()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.DirectionInputReleased();
        }
    }

    public void OnDownClicked()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.DownPressed();
        }
    }

    public void OnRightClicked()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.RightPressed();
        }
    }

    public void OnRightReleased()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.DirectionInputReleased();
        }
    }

    public void OnClockwiseClicked()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.RotateClockwise();
        }
    }

    public void OnCounterClockwiseClicked()
    {
        PillHolder pillHolder = gameManager.GetActivePillHolder();
        if (pillHolder)
        {
            pillHolder.RotateCounterClockwise();
        }
    }
}
