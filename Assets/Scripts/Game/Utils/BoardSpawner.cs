using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpawner
{
	private int width;
	private int height;
	private int currentLevel;

	public BoardSpawner(int width, int height, int currentLevel)
	{
		this.width = width;
		this.height = height;
		this.currentLevel = currentLevel;
	}

	//  private IEnumerator CreateViruses()
    // {
        // grid = new Square[width, height];

        // int totalNumberOfViruses = GetNumberOfVirusesForCurrentLevel();
        // int virusesStillToPlace = totalNumberOfViruses;

        // numberOfAliveViruses = 0;

        // while (virusesStillToPlace > 0)
        // {
        //     int x = Random.Range(0, width);
        //     int y = Random.Range(0, height - GetVirusMinDistanceFromTopForCurrentLevel());

        //     if (grid[x, y] == null)
        //     {
        //         Vector2 position = new Vector2(x, y);
        //         grid[x, y] = GameObject.Instantiate(viruses[Random.Range(0, viruses.Length)], position, Quaternion.identity) as Virus;

        //         virusesStillToPlace--;
        //         numberOfAliveViruses++;

        //         UpdateVirusCounterUi();

        //         yield return new WaitForSeconds(VIRUSES_SPAWN_ANIMATION_DURATION_MILLIS / 1000f / totalNumberOfViruses);
        //     }
        // }

        // SetupDone();
    // }

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
}
