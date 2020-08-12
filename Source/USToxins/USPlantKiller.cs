using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200002E RID: 46
	public class USPlantKiller : Filth
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x000065AD File Offset: 0x000047AD
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000065C8 File Offset: 0x000047C8
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
				List<Thing> Plantlist = TargetCell.GetThingList(TargetMap);
				if (Plantlist.Count > 0)
				{
					for (int i = 0; i < Plantlist.Count; i++)
					{
						if (Plantlist[i] is Plant && Plantlist[i].Position == TargetCell && (Plantlist[i] as Plant).def.plant.purpose != PlantPurpose.Health)
						{
							this.DoUSToxicDamage(Plantlist[i]);
						}
					}
				}
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006684 File Offset: 0x00004884
		private void DoUSToxicDamage(Thing targ)
		{
			Plant plant = targ as Plant;
			if (plant != null && plant.def.defName != "Plant_USStinkroot")
			{
				float Dmg = Math.Max(1f, 2f * (Settings.USToxLevels / 100f));
				plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x04000050 RID: 80
		private int USspawnTick;
	}
}
