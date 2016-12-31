using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int width = 8;
    public int height = 16;

    public Virus[] virusses;
    public PillHolder pillHolder;
    // public Pill[] pills;

    private Vector2 pillSpawnLocation = new Vector2(4, 15);

    private PillHolder activePillHolder;

    private Square[,] grid;

    List<PillPart> fallingPills = new List<PillPart>();

    // Use this for initialization
    void Start()
    {
        grid = new Square[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height / 2; j++)
            {
                if (Random.Range(0, 100) < 30)
                {
                    Vector2 position = new Vector2(i, j);
                    grid[i, j] = GameObject.Instantiate(virusses[Random.Range(0, virusses.Length)], position, Quaternion.identity) as Virus;
                }
            }
        }

        InvokeRepeating("Tick", 0, 1);
    }

    // todo doesn't really need to happen in update since we only tick once every second
    void Update()
    {
        if (!activePillHolder)
        {
            // activePillHolder = GameObject.Instantiate(pillHolder, pillSpawnLocation, Quaternion.identity) as PillHolder;
        }
    }

    private void Tick()
    {
        UpdateLoosePills();

        if (fallingPills.Count > 0)
        {
            DropLoosePills();
            return;
        }


        List<Square> matches = CheckForMatches();
        if (matches.Count > 0)
        {
            print("we have matches!!");
            RemoveMatches(matches);
            return;
        }


        if (activePillHolder)
        {
            activePillHolder.Tick();
        }
        else
        {
            activePillHolder = GameObject.Instantiate(pillHolder, pillSpawnLocation, Quaternion.identity) as PillHolder;
        }
    }

    public bool IsSquareFree(int x, int y)
    {
        if (x < 0 || x >= width || y < 0)
        {
            return false;
        }

        return grid[x, y] == null;
    }

    public void SetPillSettled()
    {
        AddSettledPillToGrid();
        activePillHolder = null;
        // List<Square> matches = CheckForMatches();
        // if (matches.Count > 0)
        // {
        //     print("we have matches!!");
        //     RemoveMatches(matches);
        //     UpdateLoosePills();
        // }
    }

    private void AddSettledPillToGrid()
    {
        AddPillPartToGrid(activePillHolder.GetMainPillPart());
        AddPillPartToGrid(activePillHolder.GetSecondaryPillPart());
    }

    private void AddPillPartToGrid(PillPart pillPart)
    {
        grid[(int)pillPart.transform.position.x, (int)pillPart.transform.position.y] = pillPart;
    }

    private List<Square> CheckForMatches()
    {
        List<Square> matches = new List<Square>();
        List<Square> currentlyCheckingSeries = new List<Square>();

        for (int i = 0; i < width; i++)
        {
            // Check column items
            for (int j = 0; j < height; j++)
            {
                Square currentSquare = grid[i, j];

                if (currentSquare == null)
                {
                    if (currentlyCheckingSeries.Count >= 4)
                    {
                        matches.AddRange(currentlyCheckingSeries);
                    }

                    currentlyCheckingSeries.Clear();
                }
                else
                {
                    if (currentlyCheckingSeries.Count == 0)
                    {
                        currentlyCheckingSeries.Add(currentSquare);
                    }
                    else
                    {
                        if (currentlyCheckingSeries[0].gameColor == currentSquare.gameColor)
                        {
                            currentlyCheckingSeries.Add(currentSquare);
                        }
                        else
                        {
                            if (currentlyCheckingSeries.Count >= 4)
                            {
                                matches.AddRange(currentlyCheckingSeries);
                            }

                            currentlyCheckingSeries.Clear();

                            currentlyCheckingSeries.Add(currentSquare);
                        }
                    }
                }
            }

            currentlyCheckingSeries.Clear();
        }

        return matches;
    }

    private void RemoveMatches(List<Square> matches)
    {
        foreach (Square item in matches)
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    private void UpdateLoosePills()
    {
        fallingPills.Clear();

        // Loop over all tiles and check the pills
        // Is single pill and nothing below it? FALL
        // Is connected pill and nothing below either part? FALL
        // else, don't fall

        // did something fall? perform this method again in one second
        // did nothing fall? we're done. spawn new activePillHolder

        // TODO can be improved - right now we're going to be checking a full pill twice, each pillpart will go through the check with its counterpart

        for (int i = 0; i < width; i++)
        {
            // Parts at y 0 will never fall, start checking at y 1.
            for (int j = 1; j < height; j++)
            {
                Square square = grid[i, j];

                if (!square)
                {
                    continue;
                }

                if (square.GetType() != typeof(PillPart))
                {
                    continue;
                }

                PillPart pillPart = (PillPart)square;

                print("checking pill part at " + i + ", " + j);

                if (pillPart.IsSingle())
                {
                    if(SquareAtLocationIsFallingPillPart(i, j) && !fallingPills.Contains(pillPart))
                    {
                        fallingPills.Add(pillPart);
                    }
                }
                else
                {
                    PillPart counterPart = pillPart.GetCounterPart();

                    if (counterPart.transform.position.x == pillPart.transform.position.x)
                    {
                        // Vertically aligned full pill 
                        // TODO handle this
                    }
                    else
                    {
                        // Horizontally positioned full pill
                        bool mainPillPartIsFalling = SquareAtLocationIsFallingPillPart(i, j);
                        bool secondaryPillPartIsFalling = SquareAtLocationIsFallingPillPart((int)counterPart.transform.position.x, j);

                        if(mainPillPartIsFalling && secondaryPillPartIsFalling)
                        {
                            if (!fallingPills.Contains(pillPart))
                            {
                                fallingPills.Add(pillPart);
                            }
                            if (!fallingPills.Contains(counterPart))
                            {
                                fallingPills.Add(counterPart);
                            }
                        }
                    }
                }
            }
        }
        
    }

    private bool SquareAtLocationIsFallingPillPart(int x, int y)
    {
        Square squareBelow = grid[x, y - 1];

        if(squareBelow == null)
        {
            return true;
        }

        bool sqaureBelowIsFallingPill = squareBelow.GetType() == typeof(PillPart) && fallingPills.Contains((PillPart)squareBelow);
        return sqaureBelowIsFallingPill;
    }

    private void DropLoosePills()
    {
        foreach (PillPart pillPart in fallingPills)
        {
            int currentX = (int) pillPart.transform.position.x;
            int currentY = (int) pillPart.transform.position.y;

            grid[currentX, currentY - 1] = pillPart;
            grid[currentX, currentY] = null;
            
            pillPart.transform.position = new Vector2(currentX, currentY - 1);

        }
    }
}
