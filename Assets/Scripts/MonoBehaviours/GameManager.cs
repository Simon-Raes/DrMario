using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int width = 8;
    public int height = 16;


    public GameObject pipe;
    public GameObject pipeCorner;

    public Virus[] virusses;
    public PillHolder pillHolder;
    public Text levelText;

    private Vector2 pillSpawnLocation = new Vector2(3, 15);

    private PillHolder previewPillHolder;
    private PillHolder activePillHolder;

    private Square[,] grid;

    private List<PillPart> fallingPills = new List<PillPart>();

    private const float TICK_RATE_MILLIS = 600;
    private const int MIN_TILES_IN_MATCH = 4;
    private const int MAX_VIRUS_HEIGHT = 10;
    private const int SQUARE_VIRUS_CHANCE = 30;

    // todo level finished and starting the next, harder level


    private int currentLevel;
    private int numberOfVirusses;

    private bool gameOver;
    private bool gameWon;



    // void OnEnable()
    // {
    //     //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
    //     SceneManager.sceneLoaded += OnLevelFinishedLoading;
    // }

    // void OnDisable()
    // {
    //     //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
    //     SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    // }

    // void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    // {
    //     StateHolder.level++;
    //     // currentLevel++;
    //     levelText.text = "Level " + StateHolder.level;
    // }

    void Start()
    {
        SetUpBoardContent();
        SetupBoardBorder();

        CreateUpcomingPill();

        // todo make this a coroutine so we can speed up tickrate when it's just pill parts falling
        // InvokeRepeating("Tick", 0, TICK_RATE_MILLIS / 1000f);
        StartCoroutine(GameLoop());
    }

    private void CreateUpcomingPill()
    {
        previewPillHolder = GameObject.Instantiate(pillHolder, new Vector2(width + 5, height - 2), Quaternion.identity) as PillHolder;
    }

    private void SetUpBoardContent()
    {
        grid = new Square[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < MAX_VIRUS_HEIGHT; j++)
            {
                if (Random.Range(0, 100) < SQUARE_VIRUS_CHANCE)
                {
                    numberOfVirusses++;
                    Vector2 position = new Vector2(i, j);
                    grid[i, j] = GameObject.Instantiate(virusses[Random.Range(0, virusses.Length)], position, Quaternion.identity) as Virus;
                }
            }
        }
    }

    private void SetupBoardBorder()
    {
        BorderPlacer borderPlacer = new BorderPlacer(width, height, pipe, pipeCorner);
        borderPlacer.CreateBorders();
    }

    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            if (gameWon)
            {
                // TODO go to next level
                print("go to next level here");
            }
            else if (gameOver)
            {
                // TODO restart level or go back to menu
                print("restart level here");
            }
        }
    }

    IEnumerator GameLoop()
    {
        while (!gameOver && !gameWon)
        {
            Tick();
            yield return new WaitForSeconds(TICK_RATE_MILLIS / 1000f);
        }
    }

    private void Tick()
    {
        if (IsGameWon())
        {
            gameWon = true;
            levelText.text = "You da man";
            return;
        }

        UpdateLoosePills();

        if (fallingPills.Count > 0)
        {
            DropLoosePills();
            return;
        }

        HashSet<Square> matches = CheckForMatches();
        if (matches.Count > 0)
        {
            RemoveMatches(matches);
            return;
        }

        if (activePillHolder)
        {
            activePillHolder.Tick();
        }
        else
        {
            // TODO animate the preview pill becoming the active pill (real game throws the pill into the game)
            activePillHolder = previewPillHolder;
            activePillHolder.transform.position = pillSpawnLocation;

            if (IsGameOver())
            {
                gameOver = true;
                levelText.text = "GAME OVER";
            }
            else
            {
                activePillHolder.SetControllable();
                CreateUpcomingPill();
            }
        }
    }

    private bool IsGameWon()
    {
        return numberOfVirusses == 0;
    }

    private bool IsGameOver()
    {
        return !IsSquareFree(pillSpawnLocation);
    }

    private void GoToNextLevel()
    {
        print("you da man");
        levelText.text = "you da man!";
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsSquareFree(Vector2 vector)
    {
        return IsSquareFree((int)vector.x, (int)vector.y);
    }

    public bool IsSquareFree(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
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
                    if (currentlyCheckingSeries.Count >= MIN_TILES_IN_MATCH)
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
                            if (currentlyCheckingSeries.Count >= MIN_TILES_IN_MATCH)
                            {
                                matches.UnionWith(currentlyCheckingSeries);
                            }

                            currentlyCheckingSeries.Clear();

                            currentlyCheckingSeries.Add(currentSquare);
                        }
                    }
                }
            }

            if (currentlyCheckingSeries.Count >= MIN_TILES_IN_MATCH)
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
                    if (currentlyCheckingSeries.Count >= MIN_TILES_IN_MATCH)
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
                            if (currentlyCheckingSeries.Count >= MIN_TILES_IN_MATCH)
                            {
                                matches.UnionWith(currentlyCheckingSeries);
                            }

                            currentlyCheckingSeries.Clear();

                            currentlyCheckingSeries.Add(currentSquare);
                        }
                    }
                }
            }

            if (currentlyCheckingSeries.Count >= MIN_TILES_IN_MATCH)
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
            if (item.GetType() == typeof(Virus))
            {
                numberOfVirusses--;
            }

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
                        float pillPartY = pillPart.transform.position.y;
                        float counterPartY = counterPart.transform.position.y;

                        int lowestY = (int)Mathf.Min(pillPartY, counterPartY);

                        if (lowestY == 0)
                        {
                            // Pill is on the floor, don't bother checking the rest
                            continue;
                        }

                        bool pillIsfalling = SquareAtLocationIsFallingPillPart(i, lowestY);

                        if (pillIsfalling)
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
