﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BoardDecorator))]
public class GameManager : MonoBehaviour  //, VirusSpawnListener
{
    [Header("Game")]
    public int startLevel = 0;
    public Difficulty difficulty = Difficulty.LOW;
    [Header("Board")]
    public int width = 8;
    public int height = 16;
    public GameObject pipe;
    public GameObject pipeCorner;
    [Header("Entities")]
    public Virus[] viruses;
    public PillHolder pillHolder;
    // [Header("UI")]
    // public RectTransform levelPanel;
    // public RectTransform statusPanel;
    // public RectTransform scorePanel;
    [Header("Game")]
    public GameObject panelStatus;
    public Text textStatus;
    [Header("Score")]
    public Text textTop;
    public Text textScore;
    [Header("Level")]
    public Text textLevel;
    public Text textSpeed;
    public Text textVirus;

    private const float TICK_RATE_MILLIS_LOW = 625;
    private const float TICK_RATE_MILLIS_MID = 313;
    private const float TICK_RATE_MILLIS_HI = 200;
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
    private float tickRateMillis;

    private bool gameRunning;
    private bool gameOver;
    private bool gameWon;
    private bool loosePillsAreFalling;

    float lastTick = float.MinValue;

    private int score;

    void Start()
    {
        if (StateHolder.virusLevel != -1)
        {
            currentLevel = StateHolder.virusLevel;
        }
        else
        {
            currentLevel = startLevel;
        }

        if (StateHolder.difficulty != Difficulty.NONE)
        {
            difficulty = StateHolder.difficulty;
        }

        height = Mathf.Max(GetMinBoardHeight(), height);

        pillSpawnLocation = new Vector2(width / 2 - 1, height - 1);

        SetTickRateForDifficulty();

        score = 0;
        textScore.text = score.ToString();

        SetupCameraAndUi();

        SetupGame();
    }

    private void SetTickRateForDifficulty()
    {
        switch (difficulty)
        {
            default:
            case Difficulty.LOW:
                tickRateMillis = TICK_RATE_MILLIS_LOW;
                break;
            case Difficulty.MID:
                tickRateMillis = TICK_RATE_MILLIS_MID;
                break;
            case Difficulty.HI:
                tickRateMillis = TICK_RATE_MILLIS_HI;
                break;
        }
    }

    private void SetupCameraAndUi()
    {
        BoardDecorator decorator = GetComponent<BoardDecorator>();
        decorator.SetValues(width, height, pipe, pipeCorner);
    }

    private void SetupGame()
    {
        CleanUpPreviousRound();

        StartCoroutine(CreateViruses());
    }

    private void CleanUpPreviousRound()
    {
        gameOver = false;
        gameWon = false;

        ResetUi();

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

    private void ResetUi()
    {
        // TODO
        textTop.text = "TODO";

        textLevel.text = currentLevel.ToString();

        textVirus.text = "0";
    }

    private IEnumerator CreateViruses()
    {
        grid = new Square[width, height];

        int totalNumberOfViruses = GetNumberOfVirusesForCurrentLevel();
        int virusesStillToPlace = totalNumberOfViruses;

        numberOfAliveViruses = 0;

        float spawnInterval = VIRUSES_SPAWN_ANIMATION_DURATION_MILLIS / 1000f / totalNumberOfViruses;
        print(spawnInterval + " for " + totalNumberOfViruses);
        print((spawnInterval * totalNumberOfViruses));
        WaitForSeconds waitForSeconds = new WaitForSeconds(spawnInterval);
        print(Time.time);

        while (virusesStillToPlace > 0)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height - GetVirusMinDistanceFromTopForCurrentLevel());

            if (grid[x, y] == null)
            {
                Vector2 position = new Vector2(x, y);
                Virus selectedVirus = viruses[Random.Range(0, viruses.Length)];
                GameColor color = selectedVirus.gameColor;

                // Only add this virus if it will not create a 4-in-a-row match.
                // If it would we loop around and try again with a new position and color.
                // TODO: this will create problems with small, maxed out boards.
                // can more easily end up in a situation where a virus can not be placed anywhere.
                if (!WouldCauseMatch(color, x, y))
                {
                    grid[x, y] = GameObject.Instantiate(selectedVirus, position, Quaternion.identity) as Virus;

                    virusesStillToPlace--;
                    numberOfAliveViruses++;

                    UpdateVirusCounterUi();

                    // yield return waitForSeconds;
                    yield return null;
                }
            }
        }

        print(Time.time);

        SetupDone();
    }

    private int GetNumberOfVirusesForCurrentLevel()
    {
        // Number of viruses stops increasing after level 20
        int difficultyLevel = Mathf.Min(20, currentLevel);
        int virusCount = (difficultyLevel + 1) * 4;

        return Mathf.Min(virusCount, GetMaxVirusesForBoard());
    }

    private int GetMaxVirusesForBoard()
    {
        return width * height - (width * GetVirusMinDistanceFromTopForCurrentLevel());
    }

    // The number of rows at the top of the board where no viruses can spawn. Decreases with difficulty.
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

    private int GetMinBoardHeight()
    {
        return GetVirusMinDistanceFromTopForCurrentLevel() + 1;
    }



    // Checks if adding a virus at this location would cause a match
    private bool WouldCauseMatch(GameColor color, int x, int y)
    {
        return WouldCauseMatchHorizontal(color, x, y) || WouldCauseMatchVertical(color, x, y);
    }

    private bool WouldCauseMatchHorizontal(GameColor color, int x, int y)
    {
        return WouldCauseMatch(color, x, y, true);
    }

    private bool WouldCauseMatchVertical(GameColor color, int x, int y)
    {
        return WouldCauseMatch(color, x, y, false);
    }

    private bool WouldCauseMatch(GameColor color, int x, int y, bool horizontal)
    {
        int startPos;
        int endPos;

        if (horizontal)
        {
            startPos = Mathf.Max(0, x - 3);
            endPos = Mathf.Min(width - 1, x + 3);
        }
        else
        {
            startPos = Mathf.Max(0, y - 3);
            endPos = Mathf.Min(height - 1, y + 3);
        }

        int sameColorVirusesInRow = 0;


        for (int i = startPos; i <= endPos; i++)
        {
            bool atCentralPosition = horizontal ? (i == x) : (i == y);

            if (atCentralPosition)
            {
                sameColorVirusesInRow++;

                if (sameColorVirusesInRow >= MIN_TILES_IN_MATCH)
                {
                    return true;
                }

                continue;
            }

            Square square;
            if (horizontal)
            {
                square = grid[i, y];
            }
            else
            {
                square = grid[x, i];
            }

            if (square != null && IsVirus(square))
            {
                Virus virus = (Virus)square;
                if (virus.gameColor == color)
                {
                    sameColorVirusesInRow++;

                    if (sameColorVirusesInRow >= MIN_TILES_IN_MATCH)
                    {
                        return true;
                    }
                }
                else
                {
                    sameColorVirusesInRow = 0;
                }
            }
            else
            {
                sameColorVirusesInRow = 0;
            }
        }

        return false;
    }


    private void UpdateVirusCounterUi()
    {
        textVirus.text = numberOfAliveViruses.ToString();
    }

    // The viruses have been spawned. Create the first pill and start the game.
    void SetupDone()
    {
        CreateUpcomingPill();

        // Start game after a short delay so player can survey the board and first pill before the game really starts
        Invoke("StartGame", 2);
    }

    private void CreateUpcomingPill()
    {
        previewPillHolder = GameObject.Instantiate(pillHolder, new Vector2(width + 5, height - 2), Quaternion.identity) as PillHolder;
    }

    void StartGame()
    {
        gameRunning = true;
        // Continues in Update
    }

    void Update()
    {
        if (gameRunning && ShouldTick())
        {
            lastTick = Time.time;
            Tick();
        }

        if (Input.GetKeyDown("return") || Input.GetMouseButtonDown(0))
        {
            if (gameWon)
            {
                currentLevel++;
                SetupGame();
            }
            else if (gameOver)
            {
                score = 0;
                textScore.text = score.ToString();
                SetupGame();
            }

            panelStatus.SetActive(false);
        }
    }

    private bool ShouldTick()
    {
        float tickRate = loosePillsAreFalling ? TICK_RATE_PILLS_FALLING_MILLIS : tickRateMillis;
        return (Time.time - tickRate / 1000f) > lastTick;
    }

    private void Tick()
    {
        if (IsGameWon())
        {
            gameWon = true;
            gameRunning = false;
            panelStatus.SetActive(true);
            textStatus.text = "Level " + currentLevel + " complete\nTap  to continue";
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
            UpdateAndDisplayScore(matches);

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
                gameRunning = false;
                panelStatus.SetActive(true);
                textStatus.text = "GAME OVER\nPress enter";
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
        return numberOfAliveViruses == 0;
    }

    private bool IsGameOver()
    {
        return !IsSquareFree(pillSpawnLocation) || !IsSquareFree(pillSpawnLocation + Vector2.right);
    }

    private void UpdateAndDisplayScore(HashSet<Square> matches)
    {
        int viruses = GetNumberOfVirussesInMatches(matches);
        int matchScore = GetScoreForKilledViruses(viruses);
        score += matchScore;
        textScore.text = score.ToString();
    }

    private int GetNumberOfVirussesInMatches(HashSet<Square> matches)
    {
        int count = 0;
        foreach (Square item in matches)
        {
            if (IsVirus(item))
            {
                count++;
            }
        }
        return count;
    }

    public bool IsSquareFree(Vector2 vector)
    {
        return IsSquareFree((int)vector.x, (int)vector.y);
    }

    public bool IsSquareFree(int x, int y)
    {
        if (x < 0 || x >= width || y < 0)
        {
            return false;
        }

        // Pill can rotate out of the board when rotating right after it spawned
        // Makes higher difficulties less annoying because you don't have to wait a tick before rotating.
        if (y >= height)
        {
            return true;
        }

        return grid[x, y] == null;
    }

    // Call this to indicate the player moved his pill.
    // This will restart the tick timer so we don't get two movement events (1 from player and 1 from gameloop tick) happening very close together.
    public void PillHolderMovedByPlayer()
    {
        lastTick = Time.time;
    }

    public void SetPillSettled()
    {
        AddSettledPillToGrid();
        activePillHolder = null;
    }

    private void AddSettledPillToGrid()
    {
        PillPart mainPillPart = activePillHolder.GetMainPillPart();
        PillPart secondaryPillPart = activePillHolder.GetSecondaryPillPart();

        AddPillPartIfPossible(mainPillPart, secondaryPillPart);
        AddPillPartIfPossible(secondaryPillPart, mainPillPart);
    }

    // Pills placed vertically when there is only a single square free at the top will be half-on, half-off the board
    // Destroy the part that is sticking out of the board, make the other one a single pill.
    private void AddPillPartIfPossible(PillPart partToAdd, PillPart pillPartCounterPart)
    {
        if (partToAdd.transform.position.y < height)
        {
            AddPillPartToGrid(partToAdd);
        }
        else
        {
            GameObject.Destroy(partToAdd.gameObject);
            pillPartCounterPart.SetSingle();
        }
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

    // TODO move to its own class
    public int GetScoreForKilledViruses(int virusesKilled)
    {
        /* 
        This table is taken directly from the Dr. Mario instruction booklet.
        __________________ ______________________________
        |NUMBER OF VIRUSES |   LOW   |   MED   |   HIGH   |
        |   ELIMINATED     |  SPEED  |  SPEED  |  SPEED   |
        |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
        |	     1		   |   100   |   200   |   300    |
        |        2         |   200   |   400   |   600    |
        |        3         |   400   |   800   |  1200    |
        |        4         |   800   |  1600   |  2400    |
        |        5         |  1600   |  3200   |  4800    |
        |        6         |  3200   |  6400   |  9600    | 

        */

        int difficultyMultiplier = 1;

        switch (difficulty)
        {
            case Difficulty.LOW:
                difficultyMultiplier = 1;
                break;
            case Difficulty.MID:
                difficultyMultiplier = 2;
                break;
            case Difficulty.HI:
                difficultyMultiplier = 3;
                break;
        }

        float baseScore = Mathf.Pow(2, (virusesKilled - 1));
        int finalScore = (int)baseScore * 100 * difficultyMultiplier;

        return finalScore;
    }

    private void RemoveMatches(HashSet<Square> matches)
    {
        foreach (Square item in matches)
        {
            if (IsVirus(item))
            {
                numberOfAliveViruses--;
                UpdateVirusCounterUi();
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

                if (!IsPillPart(square))
                {
                    continue;
                }

                PillPart pillPart = (PillPart)square;

                if (pillPart.IsSingle())
                {
                    if (IsSquareAtLocationFallingPillPart(i, j) && !fallingPills.Contains(pillPart))
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

                        bool pillIsfalling = IsSquareAtLocationFallingPillPart(i, lowestY);

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
                        bool mainPillPartIsFalling = IsSquareAtLocationFallingPillPart(i, j);
                        bool secondaryPillPartIsFalling = IsSquareAtLocationFallingPillPart((int)counterPart.transform.position.x, j);

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

    private bool IsSquareAtLocationFallingPillPart(int x, int y)
    {
        Square squareBelow = grid[x, y - 1];

        if (squareBelow == null)
        {
            return true;
        }

        bool sqaureBelowIsFallingPill = IsPillPart(squareBelow) && fallingPills.Contains((PillPart)squareBelow);
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

    // TODO can't we do this cleaner? method/type on the object itself instead of checking type from the outside?

    private bool IsVirus(Square square)
    {
        return square.GetType() == typeof(Virus);
    }

    private bool IsPillPart(Square square)
    {
        return square.GetType() == typeof(PillPart);
    }

    public PillHolder GetActivePillHolder()
    {
        return activePillHolder;
    }
}
