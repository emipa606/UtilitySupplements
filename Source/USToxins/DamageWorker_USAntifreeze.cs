using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000007 RID: 7
	public class DamageWorker_USAntifreeze : DamageWorker
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000025B8 File Offset: 0x000007B8
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
		{
			MoteMaker.ThrowSmoke(explosion.Position.ToVector3(), explosion.Map, 1f);
			this.ExplosionVisualEffectCenter(explosion);
		}
	}
}
