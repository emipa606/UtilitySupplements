using System;
using System.Collections.Generic;
using Verse;

namespace USToxins
{
	// Token: 0x02000028 RID: 40
	public class USToxUsesData : ThingWithComps
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00005E53 File Offset: 0x00004053
		public CompUSToxUses ToxUses
		{
			get
			{
				return base.GetComp<CompUSToxUses>();
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005E5B File Offset: 0x0000405B
		public override void PostMake()
		{
			base.PostMake();
			this.USToxUses = this.ToxUses.Props.USToxUses;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005E79 File Offset: 0x00004079
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USToxUses, "USToxUses", 100, false);
			bool flag = Scribe.mode == LoadSaveMode.LoadingVars;
			bool flag2 = Scribe.mode == LoadSaveMode.PostLoadInit;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005EA6 File Offset: 0x000040A6
		public override IEnumerable<Gizmo> GetGizmos()
		{
			yield return new Gizmo_SprayerStatus
			{
				Sprayer = this
			};
			foreach (Gizmo item in base.GetGizmos())
			{
				yield return item;
			}
			IEnumerator<Gizmo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0400004E RID: 78
		public int USToxUses = 100;
	}
}
