using System;
using System.Linq;
using KModkit;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class TheTrollScriptDecompiled : MonoBehaviour
{
	// Token: 0x06000084 RID: 132 RVA: 0x000041D0 File Offset: 0x000023D0
	private void Update()
	{
		this.solvedModules = (from x in this.Bomb.GetSolvedModuleNames()
		where !TheTrollScriptDecompiled.ignoredModules.Contains(x)
		select x).Count<string>();
		if (this.tempSolvedModules != this.solvedModules && !this.prepped)
		{
			this.ReturnToDormant();
		}
		this.tempSolvedModules = this.solvedModules;
		this.clicksToPrep = this.moduleCount % 13 + this.solvedModules % 7 + 1;
		if (this.prepped && !this.gotSolveCount)
		{
			this.solveCountAtPrep = this.solvedModules;
			this.gotSolveCount = true;
		}
		if ((this.prepped && !this.activated && this.solvedModules == this.solveCountAtPrep + 2) || (this.prepped && (this.solveCountAtPrep == this.moduleCount - 1 || this.solveCountAtPrep == this.moduleCount - 2 || this.solveCountAtPrep == this.moduleCount) && !this.activated))
		{
			this.activated = true;
			Debug.LogFormat("[The Troll #{0}] The Troll is now activated. Press The Troll when the last digit of the seconds timer is a {1}.", new object[]
			{
				this.moduleId,
				this.batteriesMod10
			});
		}
		if (this.prepped && this.activated && this.solvedModules > this.solveCountAtPrep + 2 && !this.moduleSolved)
		{
			Debug.LogFormat("[The Troll #{0}] Strike! You solved another module after activation. Returning to dormant state.", new object[]
			{
				this.moduleId
			});
			base.GetComponent<KMBombModule>().HandleStrike();
			this.ReturnToDormant();
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00004392 File Offset: 0x00002592
	private void Awake()
	{
		this.moduleId = TheTrollScriptDecompiled.moduleIdCounter++;
		KMSelectable kmselectable = this.trollButton;
		kmselectable.OnInteract = (KMSelectable.OnInteractHandler)Delegate.Combine(kmselectable.OnInteract, new KMSelectable.OnInteractHandler(delegate()
		{
			this.PressTroll();
			return false;
		}));
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000043D0 File Offset: 0x000025D0
	private void Start()
	{
		this.batteriesMod10 = this.Bomb.GetBatteryCount() % 10;
		this.moduleCount = (from x in this.Bomb.GetSolvableModuleNames()
		where !TheTrollScriptDecompiled.ignoredModules.Contains(x)
		select x).Count<string>();
		this.clicksToPrep = this.moduleCount % 13 + this.solvedModules % 7 + 1;
		Debug.LogFormat("[The Troll #{0}] Press The Troll {1} times to prep.", new object[]
		{
			this.moduleId,
			this.clicksToPrep
		});
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00004470 File Offset: 0x00002670
	public void PressTroll()
	{
		if (this.moduleSolved)
		{
			return;
		}
		base.GetComponent<KMSelectable>().AddInteractionPunch(1f);
		if (!this.prepped)
		{
			this.prePrepClicks++;
			if (this.prePrepClicks == this.clicksToPrep)
			{
				this.prepped = true;
				Debug.LogFormat("[The Troll #{0}] The Troll is now prepped. If possible, solve ONE module to activate.", new object[]
				{
					this.moduleId
				});
			}
		}
		else if (this.prepped && !this.activated)
		{
			Debug.LogFormat("[The Troll #{0}] Strike! The Troll was prepped but not activated. Returning to dormant state.", new object[]
			{
				this.moduleId
			});
			base.GetComponent<KMBombModule>().HandleStrike();
			this.ReturnToDormant();
		}
		else if (this.activated)
		{
			float num = this.Bomb.GetTime() % 60f % 10f;
			if (((float)Mathf.FloorToInt(num)).ToString() == this.batteriesMod10.ToString())
			{
				base.GetComponent<KMBombModule>().HandlePass();
				this.moduleSolved = true;
				Debug.LogFormat("[The Troll #{0}] Module disarmed.", new object[]
				{
					this.moduleId,
					this.Bomb.GetFormattedTime().Last<char>()
				});
			}
			else
			{
				Debug.LogFormat("[The Troll #{0}] Strike! You pressed The Troll when the last digit of the seconds timer was a {1}. Returning to dormant state.", new object[]
				{
					this.moduleId,
					this.Bomb.GetFormattedTime().Last<char>()
				});
				base.GetComponent<KMBombModule>().HandleStrike();
				this.ReturnToDormant();
			}
		}
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00004620 File Offset: 0x00002820
	private void ReturnToDormant()
	{
		Debug.LogFormat("[The Troll #{0}] Total solved modules is {1}. The Troll is dormant and has not been pressed.", new object[]
		{
			this.moduleId,
			this.solvedModules
		});
		this.prepped = false;
		this.activated = false;
		this.gotSolveCount = false;
		this.prePrepClicks = 0;
		this.solveCountAtPrep = 0;
		this.tempSolvedModules = this.solvedModules;
		this.clicksToPrep = this.moduleCount % 13 + this.solvedModules % 7 + 1;
		Debug.LogFormat("[The Troll #{0}] Press The Troll {1} times to prep.", new object[]
		{
			this.moduleId,
			this.clicksToPrep
		});
	}

	// Token: 0x04000079 RID: 121
	public KMBombInfo Bomb;

	// Token: 0x0400007A RID: 122
	public KMAudio Audio;

	// Token: 0x0400007B RID: 123
	public KMSelectable trollButton;

	// Token: 0x0400007C RID: 124
	private int moduleCount;

	// Token: 0x0400007D RID: 125
	private int solvedModules;

	// Token: 0x0400007E RID: 126
	private int tempSolvedModules;

	// Token: 0x0400007F RID: 127
	private int batteriesMod10;

	// Token: 0x04000080 RID: 128
	private int clicksToPrep;

	// Token: 0x04000081 RID: 129
	private int prePrepClicks;

	// Token: 0x04000082 RID: 130
	private int solveCountAtPrep;

	// Token: 0x04000083 RID: 131
	private bool gotSolveCount;

	// Token: 0x04000084 RID: 132
	private bool prepped;

	// Token: 0x04000085 RID: 133
	private bool activated;

	// Token: 0x04000086 RID: 134
	public static readonly string[] ignoredModules = new string[]
	{
		"The Troll"
	};

	// Token: 0x04000087 RID: 135
	private static int moduleIdCounter = 1;

	// Token: 0x04000088 RID: 136
	private int moduleId;

	// Token: 0x04000089 RID: 137
	private bool moduleSolved;
}
