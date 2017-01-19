using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Control the currently falling pill
public class PillHolder : MonoBehaviour
{
    GameManager gameManager;

    private bool active = false;

    public PillPart[] pillParts;

    private PillPart mainPillPart;
    private PillPart secondaryPillPart;

    private enum Position { LEFTRIGHT, BOTTOMTOP, RIGHTLEFT, TOPBOTTOM }
    private Position position = Position.LEFTRIGHT;

    private float lastDownPress = float.MinValue;
    private float lastLeftPress = float.MinValue;
    private float lastRightPress = float.MinValue;
    private const float MILLIS_BETWEEN_MOVEMENT_DOWN = 60;
    int movementsDoneDuringHold;
    // int rightMovementsDoneDuringHold;
    private const float MILLIS_BETWEEN_INITIAL_MOVEMENT_SIDEWAYS = 275;
    private const float MILLIS_BETWEEN_MOVEMENT_SIDEWAYS = 100;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        int currentX = (int)transform.position.x;
        int currentY = (int)transform.position.y;

        Vector2 mainLocation = new Vector2(currentX, currentY);
        Vector2 secondaryLocation = new Vector2(currentX + 1, currentY);

        mainPillPart = (PillPart)GameObject.Instantiate(pillParts[Random.Range(0, pillParts.Length)], mainLocation, Quaternion.identity);
        mainPillPart.transform.parent = transform;
        mainPillPart.SetPillHolder(this);

        secondaryPillPart = (PillPart)GameObject.Instantiate(pillParts[Random.Range(0, pillParts.Length)], secondaryLocation, Quaternion.identity);
        secondaryPillPart.transform.parent = transform;
        secondaryPillPart.SetPillHolder(this);

        UpdatePillRotations();
    }

    void Update()
    {
        if (!active)
        {
            return;
        }

        if (Input.GetKey("left"))
        {
            if (CanMoveLeft())
            {
                bool initialMovement = movementsDoneDuringHold == 0;
                bool bigDelayMovement = movementsDoneDuringHold == 1 && (Time.time - MILLIS_BETWEEN_INITIAL_MOVEMENT_SIDEWAYS / 1000f) > lastLeftPress;
                bool regularMovement = movementsDoneDuringHold > 1 && Time.time - (MILLIS_BETWEEN_MOVEMENT_SIDEWAYS / 1000f) > lastLeftPress;

                if (initialMovement || bigDelayMovement || regularMovement)
                {
                    MoveLeft();
                }
            }
        }
        else if (Input.GetKeyUp("left") || Input.GetKeyUp("right"))
        {
            movementsDoneDuringHold = 0;
        }
        else if (Input.GetKey("right"))
        {
            if (CanMoveRight())
            {
                bool initialMovement = movementsDoneDuringHold == 0;
                bool bigDelayMovement = movementsDoneDuringHold == 1 && (Time.time - MILLIS_BETWEEN_INITIAL_MOVEMENT_SIDEWAYS / 1000f) > lastLeftPress;
                bool regularMovement = movementsDoneDuringHold > 1 && Time.time - (MILLIS_BETWEEN_MOVEMENT_SIDEWAYS / 1000f) > lastLeftPress;

                if (initialMovement || bigDelayMovement || regularMovement)
                {
                    MoveRight();
                }
            }
        }
        else if (Input.GetKey("down"))
        {
            bool canMoveAgain = (Time.time - MILLIS_BETWEEN_MOVEMENT_DOWN / 1000f) > lastDownPress;
            if (canMoveAgain)
            {
                lastDownPress = Time.time;
                MoveDownOrSettle();
            }
        }
        
        if (Input.GetKeyDown("z"))
        {
            RotateClockwise();
        }
        else if (Input.GetKeyDown("x"))
        {
            RotateCounterClockwise();
        }
    }

    private void MoveLeft()
    {
        lastLeftPress = Time.time;
        movementsDoneDuringHold++;
        transform.position = new Vector2(transform.position.x - 1, transform.position.y);
    }

    private void MoveRight()
    {
        lastLeftPress = Time.time;
        movementsDoneDuringHold++;
        transform.position = new Vector2(transform.position.x + 1, transform.position.y);
    }

    public void SetControllable()
    {
        active = true;
    }

    private bool CanMoveLeft()
    {
        Vector2 leftOfMainPill, leftOfSecondaryPill;

        switch (position)
        {
            case Position.LEFTRIGHT:
                leftOfMainPill = CreateNewMainVectorWithOffset(-1, 0);
                return gameManager.IsSquareFree(leftOfMainPill);

            case Position.BOTTOMTOP:
            case Position.TOPBOTTOM:
                leftOfMainPill = CreateNewMainVectorWithOffset(-1, 0);
                leftOfSecondaryPill = CreateNewSecondaryVectorWithOffset(-1, 0);
                return gameManager.IsSquareFree(leftOfMainPill) && gameManager.IsSquareFree(leftOfSecondaryPill);

            case Position.RIGHTLEFT:
                leftOfSecondaryPill = CreateNewSecondaryVectorWithOffset(-1, 0);
                return gameManager.IsSquareFree(leftOfSecondaryPill);

            default:
                return false;
        }
    }

    private bool CanMoveRight()
    {
        Vector2 rightOfMainPill, rightOfSecondaryPill;

        switch (position)
        {
            case Position.LEFTRIGHT:
                rightOfSecondaryPill = CreateNewSecondaryVectorWithOffset(1, 0);
                return gameManager.IsSquareFree(rightOfSecondaryPill);

            case Position.BOTTOMTOP:
            case Position.TOPBOTTOM:
                rightOfMainPill = CreateNewMainVectorWithOffset(1, 0);
                rightOfSecondaryPill = CreateNewSecondaryVectorWithOffset(1, 0);
                return gameManager.IsSquareFree(rightOfMainPill) && gameManager.IsSquareFree(rightOfSecondaryPill);

            case Position.RIGHTLEFT:
                rightOfMainPill = CreateNewMainVectorWithOffset(1, 0);
                return gameManager.IsSquareFree(rightOfMainPill);

            default:
                return false;
        }
    }

    public void Tick()
    {
        MoveDownOrSettle();
    }

    private void MoveDownOrSettle()
    {
        if (!CanMoveDown())
        {
            SettlePill();
            return;
        }

        transform.position = new Vector2(transform.position.x, transform.position.y - 1);
        // todo also do this for settling?
        gameManager.PillHolderMovedByPlayer();
    }

    private bool CanMoveDown()
    {
        Vector2 belowMainPill, belowSecondaryPill;

        switch (position)
        {
            case Position.BOTTOMTOP:
                belowMainPill = CreateNewMainVectorWithOffset(0, -1);
                return gameManager.IsSquareFree(belowMainPill);

            case Position.LEFTRIGHT:
            case Position.RIGHTLEFT:
                belowMainPill = CreateNewMainVectorWithOffset(0, -1);
                belowSecondaryPill = CreateNewSecondaryVectorWithOffset(0, -1);
                return gameManager.IsSquareFree(belowMainPill) && gameManager.IsSquareFree(belowSecondaryPill);

            case Position.TOPBOTTOM:
                belowSecondaryPill = CreateNewSecondaryVectorWithOffset(0, -1);
                return gameManager.IsSquareFree(belowSecondaryPill);

            default:
                return false;
        }
    }

    private void SettlePill()
    {
        active = false;
        gameManager.SetPillSettled();
    }

    private void RotateClockwise()
    {
        Vector2 newMain, newSecondary;

        switch (position)
        {
            case Position.LEFTRIGHT:
                newMain = CreateNewMainVectorWithOffset(0, 1);
                newSecondary = CreateNewSecondaryVectorWithOffset(-1, 0);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.TOPBOTTOM;
                }
                break;
            case Position.BOTTOMTOP:
                newMain = CreateNewMainVectorWithOffset(0, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(1, -1);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.LEFTRIGHT;
                }
                else
                {
                    newMain = CreateNewMainVectorWithOffset(-1, 0);
                    newSecondary = CreateNewSecondaryVectorWithOffset(0, -1);
                    if (RotateIfPossible(newMain, newSecondary))
                    {
                        position = Position.LEFTRIGHT;
                    }
                }
                break;
            case Position.RIGHTLEFT:
                newMain = CreateNewMainVectorWithOffset(-1, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, 1);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.BOTTOMTOP;
                }
                break;
            case Position.TOPBOTTOM:
                newMain = CreateNewMainVectorWithOffset(1, -1);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, 0);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.RIGHTLEFT;
                }
                else
                {
                    newMain = CreateNewMainVectorWithOffset(0, -1);
                    newSecondary = CreateNewSecondaryVectorWithOffset(-1, 0);
                    if (RotateIfPossible(newMain, newSecondary))
                    {
                        position = Position.RIGHTLEFT;
                    }
                }
                break;
        }

        UpdatePillRotations();
    }

    private void RotateCounterClockwise()
    {
        Vector2 newMain, newSecondary;

        switch (position)
        {
            case Position.LEFTRIGHT:
                newMain = CreateNewMainVectorWithOffset(0, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(-1, 1);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.BOTTOMTOP;
                }
                break;
            case Position.BOTTOMTOP:
                newMain = CreateNewMainVectorWithOffset(1, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, -1);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.RIGHTLEFT;
                }
                else
                {
                    newMain = CreateNewMainVectorWithOffset(0, 0);
                    newSecondary = CreateNewSecondaryVectorWithOffset(-1, -1);
                    if (RotateIfPossible(newMain, newSecondary))
                    {
                        position = Position.RIGHTLEFT;
                    }
                }
                break;
            case Position.RIGHTLEFT:
                newMain = CreateNewMainVectorWithOffset(-1, 1);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, 0);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.TOPBOTTOM;
                }
                break;
            case Position.TOPBOTTOM:
                newMain = CreateNewMainVectorWithOffset(0, -1);
                newSecondary = CreateNewSecondaryVectorWithOffset(1, 0);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.LEFTRIGHT;
                }
                else
                {
                    newMain = CreateNewMainVectorWithOffset(-1, -1);
                    newSecondary = CreateNewSecondaryVectorWithOffset(0, 0);
                    if (RotateIfPossible(newMain, newSecondary))
                    {
                        position = Position.LEFTRIGHT;
                    }
                }
                break;
        }

        UpdatePillRotations();
    }

    private Vector2 CreateNewMainVectorWithOffset(int xOffset, int yOffset)
    {
        return CreateNewVectorWithOffset(mainPillPart, xOffset, yOffset);
    }

    private Vector2 CreateNewSecondaryVectorWithOffset(int xOffset, int yOffset)
    {
        return CreateNewVectorWithOffset(secondaryPillPart, xOffset, yOffset);
    }

    private Vector2 CreateNewVectorWithOffset(PillPart pillPart, int xOffset, int yOffset)
    {
        return new Vector2(pillPart.transform.position.x + xOffset, pillPart.transform.position.y + yOffset);
    }

    private bool RotateIfPossible(Vector2 newMain, Vector2 newSecondary)
    {
        if (gameManager.IsSquareFree(newMain) && gameManager.IsSquareFree(newSecondary))
        {
            mainPillPart.transform.position = newMain;
            secondaryPillPart.transform.position = newSecondary;

            return true;
        }

        return false;
    }

    private void UpdatePillRotations()
    {
        int mainRotation = 0;
        int secondaryRotation = 0;

        switch (position)
        {
            case Position.LEFTRIGHT:
                mainRotation = 0;
                secondaryRotation = 180;
                break;
            case Position.BOTTOMTOP:
                mainRotation = 90;
                secondaryRotation = 270;
                break;
            case Position.RIGHTLEFT:
                mainRotation = 180;
                secondaryRotation = 0;
                break;
            case Position.TOPBOTTOM:
                mainRotation = 270;
                secondaryRotation = 90;
                break;
        }

        mainPillPart.transform.rotation = Quaternion.AngleAxis(mainRotation, Vector3.forward);
        secondaryPillPart.transform.rotation = Quaternion.AngleAxis(secondaryRotation, Vector3.forward);
    }

    public PillPart GetMainPillPart()
    {
        return mainPillPart;
    }

    public PillPart GetSecondaryPillPart()
    {
        return secondaryPillPart;
    }

    public void OnPillPartDestroyed(PillPart destroyedPart)
    {
        if (destroyedPart == mainPillPart)
        {
            if (secondaryPillPart)
            {
                secondaryPillPart.SetSingle();
            }
        }
        else
        {
            if (mainPillPart)
            {
                mainPillPart.SetSingle();
            }
        }
    }

    public PillPart GetCounterPart(PillPart pillPart)
    {
        if (pillPart == mainPillPart)
        {
            return secondaryPillPart;
        }
        else
        {
            return mainPillPart;
        }
    }
}
