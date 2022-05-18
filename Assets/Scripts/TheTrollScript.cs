using KModkit;
using System.Linq;
using UnityEngine;

public class TheTrollScript : MonoBehaviour {

	public KMBombModule modSelf;
	public KMBossModule bossHandler;
	public KMBombInfo bombInfo;
	public KMAudio mAudio;
	public KMSelectable trollSelectable;

	private string[] selfIgnoredModules = new string[]
	{
		"14",
		"42",
		"501",
		"A>N<D",
		"Bamboozling Time Keeper",
		"Brainf---",
		"Busy Beaver",
		"Cookie Jars",
		"Don't Touch Anything",
		"Divided Squares",
		"Encrypted Hangman",
		"Encryption Bingo",
		"Forget Everything",
		"Forget Infinity",
		"Forget It Not",
		"Forget Me Later",
		"Forget Me Not",
		"Forget Perspective",
		"Forget The Colors",
		"Forget Them All",
		"Forget This",
		"Forget Us Not",
		"Four-Card Monte",
		"Hogwarts",
		"Iconic",
		"Kugelblitz",
		"Multitask",
		"OmegaForget",
		"Organization",
		"Purgatory",
		"Random Access Memory",
		"RPS Judging",
		"Simon Forgets",
		"Simon's Stages",
		"Souvenir",
		"The Swan",
		"Tallordered Keys",
		"The Time Keeper",
		"Timing is Everything",
		"The Troll",
		"Turn The Key",
		"The Twin",
		"Übermodule",
		"The Very Annoying Button",
		"Whiteout",
	};


	private static int moduleIdCounter = 1;
	private int moduleId;

	private bool moduleSolved, activated, isPrepped, requiredToSolve;

	private int solvedModules, pressesRequired, solvesRequiredToSolve, currentPresses;

	// Update is called once per frame
	void Update()
	{
		if (moduleSolved) return;

		var detectedSolves = bombInfo.GetSolvedModuleIDs().Where(a => a != modSelf.ModuleType).Count();

		if (solvedModules < detectedSolves)
		{
			solvedModules = detectedSolves;
			if (!activated && isPrepped)
            {
				Debug.LogFormat("[The Troll #{0}]: You have pressed the Troll {1} time(s) when it expected {2}. Returning to dormant state.", moduleId, currentPresses, pressesRequired);
				ResetTroll();
			}
		}
		if (activated)
		{
			if (!requiredToSolve)
			{
				int solvableCount = bombInfo.GetSolvableModuleNames().Where(a => !selfIgnoredModules.Contains(a)).Count();
				if (solvesRequiredToSolve > solvableCount)
				{
					Debug.LogFormat("[The Troll #{0}]: The Troll is requesting to be solved. Press it when the last seconds digit is equal to the number of batteries, modulo 10.", moduleId);
					requiredToSolve = true;
				}
				else if (solvesRequiredToSolve == solvedModules)
				{
					Debug.LogFormat("[The Troll #{0}]: You have solved enough non-Troll modules. Press it when the last seconds digit is equal to the number of batteries, modulo 10.", moduleId);
					requiredToSolve = true;
				}
			}
			else if (solvesRequiredToSolve < solvedModules)
            {
				Debug.LogFormat("[The Troll #{0}]: You have solved one too many non-Troll modules! Returning into its dormant state.", moduleId);
				modSelf.HandleStrike();
				ResetTroll();
            }
		}
	}

	// Use this for initialization
	void Start () {
		moduleId = moduleIdCounter++;
		var ignoredRepo = bossHandler.GetIgnoredModules(modSelf);
		if (ignoredRepo != null)
			selfIgnoredModules = ignoredRepo;


		trollSelectable.OnInteract += delegate {
			ProcessTroll();

			return false;
		};
	}

	void ResetTroll()
    {
		currentPresses = 0;
		activated = false;
		isPrepped = false;
		requiredToSolve = false;
    }

	void ProcessTroll()
    {
		if (moduleSolved) return;
		trollSelectable.AddInteractionPunch(1f);
		if (!isPrepped)
		{
			var solvableCount = bombInfo.GetSolvableModuleIDs().Where(a => a != modSelf.ModuleType).Count();
			Debug.LogFormat("[The Troll #{0}]: Attempting at {1} non-Troll solve(s):", moduleId, solvedModules);

			pressesRequired = solvedModules % 7 + solvableCount % 13 + 1;
			Debug.LogFormat("[The Troll #{0}]: Press the Troll {1} time(s) to prep.", moduleId, pressesRequired);
			isPrepped = true;
		}

		if (activated)
		{
			if (requiredToSolve)
			{
				var batteriesModulo10 = bombInfo.GetBatteryCount() % 10;
				var timeMod10 = (long)bombInfo.GetTime() % 10;
				if (batteriesModulo10 == timeMod10)
				{
					Debug.LogFormat("[The Troll #{0}]: You correctly pressed the Troll when the last digit of the seconds timer was a {1}. Module disarmed.", moduleId, batteriesModulo10);
					moduleSolved = true;
					modSelf.HandlePass();
				}
				else
				{
					Debug.LogFormat("[The Troll #{0}] Strike! You pressed The Troll when the last digit of the seconds timer was a {1}. Returning to dormant state.", moduleId, timeMod10);
					modSelf.HandleStrike();
					ResetTroll();
				}
			}
			else
            {
				Debug.LogFormat("[The Troll #{0}] Strike! You pressed The Troll is not requesting to be solved. Returning to dormant state.", moduleId);
				modSelf.HandleStrike();
				ResetTroll();
			}
		}
		else if (currentPresses < pressesRequired)
		{
			currentPresses++;
			if (currentPresses == pressesRequired)
			{
				Debug.LogFormat("[The Troll #{0}]: The Troll is now prepped. If possible solve TWO non-Troll modules to activate it.", moduleId, pressesRequired);
				activated = true;
				solvesRequiredToSolve = solvedModules + 2;
			}
		}
    }
}

