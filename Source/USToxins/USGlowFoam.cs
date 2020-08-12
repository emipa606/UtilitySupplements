using System;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200001C RID: 28
	public class USGlowFoam : Filth
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00004B41 File Offset: 0x00002D41
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
			Scribe_Values.Look<int>(ref this.USGlowTick, "USGlowTick", 0, false);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004B70 File Offset: 0x00002D70
		public override void Tick()
		{
			this.USspawnTick++;
			if (this.USspawnTick >= 20000)
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			this.USGlowTick--;
			if (this.USGlowTick <= 0)
			{
				MoteMaker.ThrowHeatGlow(base.Position, base.Map, 0.8f);
				this.USGlowTick = 90;
			}
		}

		// Token: 0x04000042 RID: 66
		private int USspawnTick;

		// Token: 0x04000043 RID: 67
		private int USGlowTick;
	}
}
