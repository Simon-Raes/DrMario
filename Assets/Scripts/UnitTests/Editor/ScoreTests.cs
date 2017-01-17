using UnityEngine;
using NUnit.Framework;

public class ScoreTests
{
    [Test]
    public void TestScoring()
    {
		GameManager gm = new GameManager();
		float score = gm.GetScoreForKilledViruses(3);

		// todo use assert equals for better error logs
        Assert.That(score == 213233);
		Assert.That(score == 213233);
    }
}