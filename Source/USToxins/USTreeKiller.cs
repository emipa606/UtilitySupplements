using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200002F RID: 47
	public class USTreeKiller : Filth
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x000066F7 File Offset: 0x000048F7
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006714 File Offset: 0x00004914
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
						if (Plantlist[i] is Plant && Plantlist[i].Position == TargetCell && ((Plantlist[i] as Plant).def.plant.IsTree || (Plantlist[i] as Plant).def.plant.purpose == PlantPurpose.Misc))
						{
							this.DoUSToxicDamage(Plantlist[i]);
						}
					}
				}
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000067F0 File Offset: 0x000049F0
		private void DoUSToxicDamage(Thing targ)
		{
			Plant plant = targ as Plant;
			if (plant != null && plant.def.defName != "Plant_USStinkroot")
			{
				float Dmg = 4f * (Settings.USToxLevels / 100f);
				if (Dmg < 2f)
				{
					Dmg = 2f;
				}
				plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x04000051 RID: 81
		private int USspawnTick;
	}
}
