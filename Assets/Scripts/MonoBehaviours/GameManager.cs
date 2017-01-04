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
    public Virus[] viruses;
    public PillHolder pillHolder;
    public Text levelText;

    private const float TICK_RATE_MILLIS = 600;
    private const float TICK_RATE_PILLS_FALLING_MILLIS = 100;
    private const float VIRUSES_SPAWN_ANIMATION_DURATION_MILLIS = 1000;
    private const int MIN_TILES_IN_MATCH = 4;

    private Vector2 pillSpawnLocation;

    private PillHolder previewPillHolder;
    private PillHolder activePillHolder;

    private Square[,] grid;

    private List<PillPart> fallingPills = new List<PillPart>();

    private int currentLevel;
    private int numberOfAliveViruses;

    private bool gameOver;
    private bool gameWon;
    private bool loosePillsAreFalling;

    void Start()
    {
        pillSpawnLocation = new Vector2(width / 2 - 1, height - 1);

        SetupBoardBorder();

        SetupGame();
    }

    private void SetupBoardBorder()
    {
        BorderPlacer borderPlacer = new BorderPlacer(width, height, pipe, pipeCorner);
        borderPlacer.CreateBorders();
    }

    void SetupGame()
    {
        CleanUpPreviousRound();

        StartCoroutine(CreateViruses());
    }

    private void CleanUpPreviousRound()
    {
        gameOver = false;
        gameWon = false;
        levelText.text = "";

        if (grid != null)
        {
            for (int k = 0; k < grid.GetLength(0); k++)
            {
                for (int l = 0; l < grid.GetLength(1); l++)
                {
                    Square square = grid[k, l];
                    if (square)
                    {
                        GameObject.Destroy(square.gameObject);
                        grid[k, l] = null;
                    }
                }
            }
        }

        if (fallingPills != null && fallingPills.Count > 0)
        {
            foreach (PillPart pill in fallingPills)
            {
                GameObject.Destroy(pill.gameObject);
            }
        }

        if (previewPillHolder != null)
        {
            GameObject.Destroy(previewPillHolder.gameObject);
        }

        if (activePillHolder != null)
        {
            GameObject.Destroy(activePillHolder.gameObject);
        }
    }

    // TODO the real game will never spawn more than 3 viruses of the same color next to each other
    // if you do you already get matches and virus kills before you even started playing
    private IEnumerator CreateViruses()
    {
        grid = new Square[width, height];

        int virusesStillToPlace = GetNumberOfVirusesForCurrentLevel();
        numberOfAliveViruses = virusesStillToPlace;

        while (virusesStillToPlace > 0)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height - GetVirusMinDistanceFromTopForCurrentLevel());

            if (grid[x, y] == null)
            {
                Vector2 position = new Vector2(x, y);
                grid[x, y] = GameObject.Instantiate(viruses[Random.Range(0, viruses.Length)], position, Quaternion.identity) as Virus;

                virusesStillToPlace--;

                yield return new WaitForSeconds(VIRUSES_SPAWN_ANIMATION_DURATION_MILLIS / 1000f / numberOfAliveViruses);
            }
        }

        SetupDone();
    }

    // The viruses have been spawned. Create the first pill and start the game.
    void SetupDone()
    {
        CreateUpcomingPill();

        // Start game after a short delay so player can survey the board and first pill before the game really starts
        Invoke("StartGame", 2);
    }

    void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    private int GetNumberOfVirusesForCurrentLevel()
    {
        // Number of viruses stops increasing after level 20
        int difficultyLevel = Mathf.Min(20, currentLevel);
        return (difficultyLevel + 1) * 4;
    }

    private int GetVirusMinDistanceFromTopForCurrentLevel()
    {
        int difficultyLevel = Mathf.Min(20, currentLevel);
        // Guestimate based on the real game
        if (difficultyLevel < 10)
        {
            return 5;
        }
        else if (difficultyLevel < 15)
        {
            return 4;
        }
        else
        {
            return 3;
        }
    }

    private void CreateUpcomingPill()
    {
        previewPillHolder = GameObject.Instantiate(pillHolder, new Vector2(width + 5, height - 2), Quaternion.identity) as PillHolder;
    }

    IEnumerator GameLoop()
    {
        while (!gameOver && !gameWon)
        {
            Tick();
            float tickRate = loosePillsAreFalling ? TICK_RATE_PILLS_FALLING_MILLIS : TICK_RATE_MILLIS;
            yield return new WaitForSeconds(tickRate / 1000f);
        }
    }

    private void Tick()
    {
        if (IsGameWon())
        {
            gameWon = true;
            levelText.text = "Level " + currentLevel + " complete\nPress enter.";
            return;
        }

        UpdateLoosePills();

        if (fallingPills.Count > 0)
        {
            loosePillsAreFalling = true;
            DropLoosePills();
            return;
        }
        else
        {
            loosePillsAreFalling = false;
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
                levelText.text = "GAME OVER\nPress enter";
            }
            else
            {
                activePillHolder.SetControllable();
                CreateUpcomingPill();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            if (gameWon)
            {
                currentLevel++;
                SetupGame();
            }
            else if (gameOver)
            {
                currentLevel = 0;
                SetupGame();
            }
        }
    }

    private bool IsGameWon()
    {
        return numberOfAliveViruses == 0;
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
                numberOfAliveViruses--;
            }

            GameObject.Destroy(item.gameObject);
        }
    }

    private void UpdateLoosePills()
    {
        fallingPills.Clear();

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
