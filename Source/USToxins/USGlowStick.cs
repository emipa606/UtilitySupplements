using System;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200001D RID: 29
	public class USGlowStick : Filth
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00004BDD File Offset: 0x00002DDD
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
			Scribe_Values.Look<int>(ref this.USGlowTick, "USGlowTick", 0, false);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004C0C File Offset: 0x00002E0C
		public override void Tick()
		{
			this.USspawnTick++;
			if (this.USspawnTick > 30000)
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			this.USGlowTick--;
			if (this.USGlowTick <= 0)
			{
				MoteMaker.ThrowHeatGlow(base.Position, base.Map, 2f);
				this.USGlowTick = 90;
			}
		}

		// Token: 0x04000044 RID: 68
		private int USspawnTick;

		// Token: 0x04000045 RID: 69
		private int USGlowTick;
	}
}
