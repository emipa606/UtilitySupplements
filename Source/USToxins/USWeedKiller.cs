using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000030 RID: 48
	public class USWeedKiller : Filth
	{
		// Token: 0x060000AD RID: 173 RVA: 0x00006867 File Offset: 0x00004A67
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006884 File Offset: 0x00004A84
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
						if (Plantlist[i] is Plant && Plantlist[i].Position == TargetCell && (Plantlist[i] as Plant).def.plant.purpose == PlantPurpose.Misc && !(Plantlist[i] as Plant).def.plant.IsTree)
						{
							this.DoUSToxicDamage(Plantlist[i]);
						}
					}
				}
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006960 File Offset: 0x00004B60
		private void DoUSToxicDamage(Thing targ)
		{
			Plant plant = targ as Plant;
			if (plant != null && plant.def.defName != "Plant_USStinkroot")
			{
				float Dmg = 2f * (Settings.USToxLevels / 100f);
				if (Dmg < 1f)
				{
					Dmg = 1f;
				}
				plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x04000052 RID: 82
		private int USspawnTick;
	}
}
