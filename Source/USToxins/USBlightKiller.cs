using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200001B RID: 27
	public class USBlightKiller : Filth
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00004A18 File Offset: 0x00002C18
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004A34 File Offset: 0x00002C34
		public override void Tick()
		{
			this.USspawnTick++;
			if (this.USspawnTick >= 60000)
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			if (Find.TickManager.TicksGame % 120 == 0)
			{
				Map TargetMap = base.Map;
				IntVec3 TargetCell = base.Position;
				List<Thing> Blightlist = TargetCell.GetThingList(TargetMap);
				if (Blightlist.Count > 0)
				{
					for (int i = 0; i < Blightlist.Count; i++)
					{
						if (Blightlist[i] is Blight && Blightlist[i].Position == TargetCell)
						{
							this.DoUSToxicDamage(Blightlist[i]);
						}
					}
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004AD4 File Offset: 0x00002CD4
		private void DoUSToxicDamage(Thing targ)
		{
			Blight blight = targ as Blight;
			if (blight != null)
			{
				float Dmg = 0.1f * this.toxicRatio;
				if (Dmg < 1f)
				{
					Dmg = 1f;
				}
				blight.Severity -= Dmg;
				if (blight.Severity <= 0f)
				{
					blight.Destroy(DestroyMode.Vanish);
				}
			}
		}

		// Token: 0x04000040 RID: 64
		private int USspawnTick;

		// Token: 0x04000041 RID: 65
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
