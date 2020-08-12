using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000006 RID: 6
	public class DamageWorker_USAmmoniaFert : DamageWorker
	{
		// Token: 0x0600000D RID: 13 RVA: 0x0000257C File Offset: 0x0000077C
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
		{
			MoteMaker.ThrowSmoke(explosion.Position.ToVector3(), explosion.Map, 1f);
			this.ExplosionVisualEffectCenter(explosion);
		}
	}
}
