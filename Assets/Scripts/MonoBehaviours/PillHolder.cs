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

        float input = 0;

        if (Input.GetKeyDown("left"))
        {
            if (CanMoveLeft())
            {
                input = -1;
            }
        }
        else if (Input.GetKeyDown("right"))
        {
            // TODO right now rotations get block when pill is vertical against the right wall (because can't rotate into the wall)
            // In the real game this pushes the pill one block to the left so it can rotate.
            if (CanMoveRight())
            {
                input = 1;
            }
        }
        else if (Input.GetKeyDown("down"))
        {
            // TODO also need to reset the tickclock here - now you have both this and the tick moving the block down at the same time
            // looks and feels bad
            MoveDownOrSettle();
        }
        else if (Input.GetKeyDown("z"))
        {
            RotateClockwise();
        }
        else if (Input.GetKeyDown("x"))
        {
            RotateCounterClockwise();
        }

        if (input != 0)
        {
            transform.position = new Vector2(transform.position.x + input, transform.position.y);
        }
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

                if (gameManager.IsSquareFree(newMain) && gameManager.IsSquareFree(newSecondary))
                {
                    position = Position.TOPBOTTOM;
                    mainPillPart.transform.position = newMain;
                    secondaryPillPart.transform.position = newSecondary;
                }
                break;
            case Position.BOTTOMTOP:
                newMain = CreateNewMainVectorWithOffset(0, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(1, -1);

                if (gameManager.IsSquareFree(newSecondary))
                {
                    position = Position.LEFTRIGHT;
                    secondaryPillPart.transform.position = newSecondary;
                }
                break;
            case Position.RIGHTLEFT:
                newMain = CreateNewMainVectorWithOffset(-1, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, 1);

                if (gameManager.IsSquareFree(newMain) && gameManager.IsSquareFree(newSecondary))
                {
                    position = Position.BOTTOMTOP;
                    mainPillPart.transform.position = newMain;
                    secondaryPillPart.transform.position = newSecondary;
                }
                break;
            case Position.TOPBOTTOM:
                newMain = CreateNewMainVectorWithOffset(1, -1);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, 0);

                if (gameManager.IsSquareFree(newMain))
                {
                    position = Position.RIGHTLEFT;
                    mainPillPart.transform.position = newMain;
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

                if (gameManager.IsSquareFree(newSecondary))
                {
                    position = Position.BOTTOMTOP;
                    secondaryPillPart.transform.position = newSecondary;
                }
                break;
            case Position.BOTTOMTOP:
                newMain = CreateNewMainVectorWithOffset(1, 0);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, -1);

                if (gameManager.IsSquareFree(newMain) && gameManager.IsSquareFree(newSecondary))
                {
                    position = Position.RIGHTLEFT;
                    mainPillPart.transform.position = newMain;
                    secondaryPillPart.transform.position = newSecondary;
                }
                break;
            case Position.RIGHTLEFT:
                newMain = CreateNewMainVectorWithOffset(-1, 1);
                newSecondary = CreateNewSecondaryVectorWithOffset(0, 0);

                if (gameManager.IsSquareFree(newMain))
                {
                    position = Position.TOPBOTTOM;
                    mainPillPart.transform.position = newMain;
                }
                break;
            case Position.TOPBOTTOM:
                newMain = CreateNewMainVectorWithOffset(0, -1);
                newSecondary = CreateNewSecondaryVectorWithOffset(1, 0);

                if (RotateIfPossible(newMain, newSecondary))
                {
                    position = Position.LEFTRIGHT;
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
