using System;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200001A RID: 26
	public class USAntifreeze : Filth
	{
		// Token: 0x0600005A RID: 90 RVA: 0x0000495B File Offset: 0x00002B5B
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004978 File Offset: 0x00002B78
		public override void Tick()
		{
			this.USspawnTick++;
			int removeDelay = 120000;
			if (this.USspawnTick >= removeDelay)
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			if (Find.TickManager.TicksGame % 2497 == 0)
			{
				Map TargetMap = base.Map;
				IntVec3 TargetCell = base.Position;
				float SnowDepth = TargetMap.snowGrid.GetDepth(TargetCell);
				if (SnowDepth > 0f)
				{
					SnowDepth -= Math.Max(0f, this.GetRndMelt());
					TargetMap.snowGrid.SetDepth(TargetCell, SnowDepth);
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000049FF File Offset: 0x00002BFF
		public float GetRndMelt()
		{
			return Rand.Range(0.1f, 0.2f);
		}

		// Token: 0x0400003F RID: 63
		private int USspawnTick;
	}
}
