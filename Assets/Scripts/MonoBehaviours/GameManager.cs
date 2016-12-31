using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int width = 8;
    public int height = 16;

    public GameObject pipe;
    public GameObject piperCorner;
    public Virus[] virusses;
    public PillHolder pillHolder;
    // public Pill[] pills;

    private Vector2 pillSpawnLocation = new Vector2(3, 15);

    private PillHolder activePillHolder;

    private Square[,] grid;

    List<PillPart> fallingPills = new List<PillPart>();

    // Use this for initialization
    void Start()
    {
        SetUpBoardContent();
        SetUpBoardBorder();

        InvokeRepeating("Tick", 0, 1);
    }

    private void SetUpBoardContent()
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
    }

    private void SetUpBoardBorder()
    {
        // Horizontal pipes
        for (int i = 0; i < width; i++)
        {
            GameObject.Instantiate(pipe, new Vector3(i, -1, 0), Quaternion.AngleAxis(90, Vector3.forward));

            if (i < 2 || i > 5)
            {
                GameObject.Instantiate(pipe, new Vector3(i, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
            }
        }

        // Vertical pipes
        for (int i = 0; i < height; i++)
        {
            GameObject.Instantiate(pipe, new Vector3(-1, i, 0), Quaternion.identity);
            GameObject.Instantiate(pipe, new Vector3(width, i, 0), Quaternion.AngleAxis(180, Vector3.forward));
        }

        // Corners
        GameObject.Instantiate(piperCorner, new Vector3(-1, -1, 0), Quaternion.identity);
        GameObject.Instantiate(piperCorner, new Vector3(-1, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
        GameObject.Instantiate(piperCorner, new Vector3(width, -1, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(piperCorner, new Vector3(width, height, 0), Quaternion.AngleAxis(180, Vector3.forward));

        // Top opening
        GameObject.Instantiate(piperCorner, new Vector3(2, height, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(piperCorner, new Vector3(5, height, 0), Quaternion.identity);

        GameObject.Instantiate(pipe, new Vector3(2, height + 1, 0), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(5, height + 1, 0), Quaternion.identity);

        GameObject.Instantiate(piperCorner, new Vector3(2, height + 2, 0), Quaternion.AngleAxis(180, Vector3.forward));
        GameObject.Instantiate(piperCorner, new Vector3(5, height + 2, 0), Quaternion.AngleAxis(270, Vector3.forward));

        GameObject.Instantiate(piperCorner, new Vector3(1, height + 2, 0), Quaternion.identity);
        GameObject.Instantiate(piperCorner, new Vector3(6, height + 2, 0), Quaternion.AngleAxis(90, Vector3.forward));

        GameObject.Instantiate(pipe, new Vector3(1, height + 3, 0), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(6, height + 3, 0), Quaternion.identity);
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


        HashSet<Square> matches = CheckForMatches();
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

    private HashSet<Square> CheckForMatches()
    {
        HashSet<Square> matches = new HashSet<Square>();
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
                        matches.UnionWith(currentlyCheckingSeries);
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
                                matches.UnionWith(currentlyCheckingSeries);
                            }

                            currentlyCheckingSeries.Clear();

                            currentlyCheckingSeries.Add(currentSquare);
                        }
                    }
                }
            }

            if(currentlyCheckingSeries.Count > 0)
            {
                matches.UnionWith(currentlyCheckingSeries);
            }

            currentlyCheckingSeries.Clear();
        }


        // TODO clean up this duplicate code
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Square currentSquare = grid[j, i];

                if (currentSquare == null)
                {
                    if (currentlyCheckingSeries.Count >= 4)
                    {
                        matches.UnionWith(currentlyCheckingSeries);
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
                                matches.UnionWith(currentlyCheckingSeries);
                            }

                            currentlyCheckingSeries.Clear();

                            currentlyCheckingSeries.Add(currentSquare);
                        }
                    }
                }
            }

            if(currentlyCheckingSeries.Count > 0)
            {
                matches.UnionWith(currentlyCheckingSeries);
            }

            currentlyCheckingSeries.Clear();
        }

        return matches;
    }

    private void RemoveMatches(HashSet<Square> matches)
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
                    if (SquareAtLocationIsFallingPillPart(i, j) && !fallingPills.Contains(pillPart))
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

                        if (mainPillPartIsFalling && secondaryPillPartIsFalling)
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

        if (squareBelow == null)
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
            int currentX = (int)pillPart.transform.position.x;
            int currentY = (int)pillPart.transform.position.y;

            grid[currentX, currentY - 1] = pillPart;
            grid[currentX, currentY] = null;

            pillPart.transform.position = new Vector2(currentX, currentY - 1);

        }
    }
}
