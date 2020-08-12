using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200002D RID: 45
	public class USFilthKiller : Filth
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00006450 File Offset: 0x00004650
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000646C File Offset: 0x0000466C
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
				List<Thing> Filthlist = TargetCell.GetThingList(TargetMap);
				if (Filthlist.Count > 0)
				{
					for (int i = 0; i < Filthlist.Count; i++)
					{
						if (Filthlist[i] is Filth && Filthlist[i].Position == TargetCell && Filthlist[i].def.defName != "Filth_USFilthKillFoam")
						{
							float CleaningAmount = Filthlist[i].def.filth.cleaningWorkToReduceThickness * 60f;
							float toxRatio = Settings.USToxLevels / 100f;
							if (toxRatio > 0f)
							{
								CleaningAmount /= toxRatio;
							}
							if ((float)(Find.TickManager.TicksGame - this.USspawnTick) > CleaningAmount)
							{
								this.DoUSToxicDamage(Filthlist[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00006588 File Offset: 0x00004788
		private void DoUSToxicDamage(Thing targ)
		{
			Filth filth = targ as Filth;
			if (filth != null)
			{
				filth.ThinFilth();
			}
		}

		// Token: 0x0400004F RID: 79
		private int USspawnTick;
	}
}
