using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillHolder : MonoBehaviour
{
    GameManager gameManager;

    private bool active = true;

    public PillPart[] pillParts;

    private PillPart mainPillPart;
    private PillPart secondaryPillPart;


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (pillParts != null && pillParts.Length > 0)
        {
            Vector2 mainLocation = new Vector2(CurrentX(), CurrentY());
            Vector2 secondaryLocation = new Vector2(CurrentX() + 1, CurrentY());

            mainPillPart = (PillPart)GameObject.Instantiate(pillParts[Random.Range(0, pillParts.Length)], mainLocation, Quaternion.identity);
            mainPillPart.transform.parent = transform;
            mainPillPart.SetPillHolder(this);

            secondaryPillPart = (PillPart)GameObject.Instantiate(pillParts[Random.Range(0, pillParts.Length)], secondaryLocation, Quaternion.identity);
            secondaryPillPart.transform.parent = transform;
            secondaryPillPart.transform.Rotate(Vector3.forward * 180);
            secondaryPillPart.SetPillHolder(this);
        }


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
            if (gameManager.IsSquareFree(CurrentX() - 1, CurrentY()))
            {
                input = -1;
            }
        }
        else if (Input.GetKeyDown("right"))
        {
            // todo + 2 now because the left half of the pill is at zero - but this will also need to account for rotation
            if (gameManager.IsSquareFree(CurrentX() + 2, CurrentY()))
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


        if (input != 0)
        {
            transform.position = new Vector2(transform.position.x + input, transform.position.y);
        }
    }

    public void Tick()
    {
        MoveDownOrSettle();
    }

    private int CurrentX()
    {
        return (int)transform.position.x;
    }

    private int CurrentY()
    {
        return (int)transform.position.y;
    }

    private void MoveDownOrSettle()
    {
        // todo only checking for falling down here - also need to check for going sideways
        // only down-false should result in settling!

        if (!gameManager.IsSquareFree(CurrentX(), CurrentY() - 1))
        {
            SettlePill();
            return;
        }

        if (!gameManager.IsSquareFree(CurrentX() + 1, CurrentY() - 1))
        {
            SettlePill();
            return;
        }

        transform.position = new Vector2(transform.position.x, transform.position.y - 1);
    }

    private void SettlePill()
    {
        active = false;
        gameManager.SetPillSettled();
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
        if(pillPart == mainPillPart)
        {
            return secondaryPillPart;
        }
        else
        {
            return mainPillPart;
        }
    }
}
