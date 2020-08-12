using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000014 RID: 20
	[HarmonyPatch(typeof(Plant), "TickLong")]
	public class Plant_TickLong
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003610 File Offset: 0x00001810
		[HarmonyPostfix]
		public static void Postfix(ref Plant __instance)
		{
			if (__instance == null)
			{
				return;
			}
			if (__instance.DestroyedOrNull())
			{
				return;
			}
			if (!(__instance.def.defName == "Plant_USStinkroot") && !(__instance.def.defName == "Plant_USTangleroot"))
			{
				return;
			}
			Map TargetMap = __instance.Map;
			IntVec3 TargetCell = __instance.Position;
			List<Thing> Plantlist = TargetCell.GetThingList(TargetMap);
			if (Plantlist.Count > 0)
			{
				for (int i = 0; i < Plantlist.Count; i++)
				{
					if (Plantlist[i] != null && Plantlist[i] is Plant)
					{
						if (Plantlist[i].def.defName == "Plant_USStinkroot" && Settings.doSRGas)
						{
							if (Plantlist[i].Position == TargetCell)
							{
								int stinkyRadius = Settings.SRGasRadius;
								float maturity = (Plantlist[i] as Plant).Growth;
								stinkyRadius = (int)((float)stinkyRadius * maturity);
								if (stinkyRadius < 1)
								{
									stinkyRadius = 1;
								}
								List<IntVec3> stinkycells = GenRadial.RadialCellsAround(TargetCell, (float)stinkyRadius, true).ToList<IntVec3>();
								if (stinkycells.Count > 0)
								{
									for (int j = 0; j < stinkycells.Count; j++)
									{
										IntVec3 chkcell = stinkycells[j];
										if (chkcell.IsValid && chkcell.InBounds(TargetMap) && !chkcell.Impassable(TargetMap))
										{
											GenSpawn.Spawn(Globals.USStinkyGas, chkcell, TargetMap, WipeMode.Vanish);
										}
									}
								}
							}
						}
						else if (Plantlist[i].def.defName == "Plant_USTangleroot" && Plantlist[i].Position == TargetCell)
						{
							List<Thing> Pawnlist = TargetCell.GetThingList(TargetMap);
							if (Pawnlist.Count > 0)
							{
								for (int k = 0; k < Pawnlist.Count; k++)
								{
									if (Pawnlist[k] is Pawn && Pawnlist[k].Position == TargetCell)
									{
										Plant_TickLong.DoUSTangleDamage(Pawnlist[k] as Pawn, Plantlist[i]);
									}
								}
							}
							if (Settings.doTRGas && TargetCell.IsValid && TargetCell.InBounds(TargetMap) && !TargetCell.Impassable(TargetMap))
							{
								GenSpawn.Spawn(Globals.USTangleGas, TargetCell, TargetMap, WipeMode.Vanish);
							}
							if (Settings.doTRCreep && __instance.Growth >= 1f && (float)Plant_TickLong.GenRndVal(0, 1) <= 0.03f)
							{
								List<IntVec3> chkcreepcells = GenRadial.RadialCellsAround(TargetCell, 1f, false).ToList<IntVec3>();
								if (chkcreepcells.Count > 0)
								{
									List<IntVec3> creepcelllist = new List<IntVec3>();
									creepcelllist.Clear();
									for (int l = 0; l < chkcreepcells.Count; l++)
									{
										IntVec3 chkcreepcell = chkcreepcells[l];
										if (chkcreepcell.IsValid && chkcreepcell.InBounds(TargetMap) && !chkcreepcell.Impassable(TargetMap) && chkcreepcell.GetTerrain(TargetMap).fertility >= Globals.USTanglePlantFertMin)
										{
											bool AddToList = true;
											List<Thing> SamePlantlist = chkcreepcell.GetThingList(TargetMap);
											if (SamePlantlist.Count > 0)
											{
												for (int m = 0; m < SamePlantlist.Count; m++)
												{
													if (SamePlantlist[m] is Fire)
													{
														AddToList = false;
													}
													if (SamePlantlist[m] is Plant && SamePlantlist[m].def.defName == "Plant_USTangleroot")
													{
														AddToList = false;
													}
												}
											}
											if (AddToList)
											{
												creepcelllist.Add(chkcreepcell);
											}
										}
									}
									if (creepcelllist.Count > 0)
									{
										int rndcreeper = Plant_TickLong.GenRndVal(0, creepcelllist.Count);
										rndcreeper--;
										if (rndcreeper < 0)
										{
											rndcreeper = 0;
										}
										IntVec3 creeperCell = creepcelllist[rndcreeper];
										if (creeperCell.IsValid && creeperCell != TargetCell)
										{
											List<Thing> ClearPlantlist = creeperCell.GetThingList(TargetMap);
											if (ClearPlantlist.Count > 0)
											{
												for (int n = 0; n < ClearPlantlist.Count; n++)
												{
													if (ClearPlantlist[n] is Plant && ClearPlantlist[n].def.defName != "Plant_USTangleroot")
													{
														bool kill = true;
														if ((ClearPlantlist[n] as Plant).def.plant.IsTree)
														{
															kill = false;
														}
														Plant_TickLong.DoUSTangleCreepDmg(ClearPlantlist[n], kill, __instance);
													}
												}
											}
											Thing plantnew = GenSpawn.Spawn(Globals.USTanglePlant, creeperCell, TargetMap, WipeMode.Vanish);
											if (!plantnew.DestroyedOrNull() && plantnew is Plant)
											{
												(plantnew as Plant).Growth = 0.05f;
												TargetMap.mapDrawer.MapMeshDirty(plantnew.Position, MapMeshFlag.Things);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003AC0 File Offset: 0x00001CC0
		public static int GenRndVal(int first, int second)
		{
			return Rand.Range(first, second);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003ACC File Offset: 0x00001CCC
		public static void DoUSTangleCreepDmg(Thing T, bool kill, Thing C)
		{
			if (T is Plant)
			{
				if (kill)
				{
					T.Destroy(DestroyMode.Vanish);
					return;
				}
				float Dmg = Rand.Range(1f, 51f);
				if (Dmg < 1f)
				{
					Dmg = 1f;
				}
				T.TakeDamage(new DamageInfo(DamageDefOf.Cut, Dmg, 0f, -1f, C, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003B2C File Offset: 0x00001D2C
		public static void DoUSTangleDamage(Pawn P, Thing T)
		{
			if (P != null)
			{
				float Dmg = Rand.Range(1f, 11f);
				if (Dmg < 1f)
				{
					Dmg = 1f;
				}
				P.TakeDamage(new DamageInfo(DamageDefOf.Cut, Dmg, 0f, -1f, T, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
				float StunDmg = Rand.Range(1f, 31f);
				if (StunDmg < 1f)
				{
					StunDmg = 1f;
				}
				P.TakeDamage(new DamageInfo(DamageDefOf.Stun, StunDmg, 0f, -1f, T, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x04000032 RID: 50
		public static float toxRatio = Settings.USToxLevels / 100f;
	}
}
