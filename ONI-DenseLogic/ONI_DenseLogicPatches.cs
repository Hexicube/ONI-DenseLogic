/*
 * Copyright 2020 Dense Logic Team
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Database;
using Harmony;
using System.Collections.Generic;
using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;

namespace ONI_DenseLogic {
	/// <summary>
	/// Patches which will be applied for Dense Logic.
	/// </summary>
	public static class ONI_DenseLogicPatches {
		public static void OnLoad() {
			PUtil.InitLibrary();
			LocString.CreateLocStringKeys(typeof(DenseLogicStrings.BUILDINGS));
			LocString.CreateLocStringKeys(typeof(DenseLogicStrings.UI));
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class SideScreenCreator {
			internal static void Postfix() {
				PUIUtils.AddSideScreenContent<LogicGateSelectSideScreen>();
				PUIUtils.AddSideScreenContent<FourBitSelectSideScreen>();
				PUIUtils.AddSideScreenContent<RemapperSideScreen>();
			}
		}

		[HarmonyPatch(typeof(LogicBitSelectorSideScreen), "RefreshToggles")]
		public static class AAA {
			internal static void Postfix(LogicBitSelectorSideScreen __instance) {
				PUIUtils.DebugObjectTree(__instance.toggles_by_int[1].gameObject);
			}
		}

		private static void AddBuildingToPlanScreen(HashedString category, string building_id,
				string building_after = null) {
			if (building_after == null)
				ModUtil.AddBuildingToPlanScreen(category, building_id);
			else {
				int index = TUNING.BUILDINGS.PLANORDER.FindIndex(x => x.category == category);
				if (index < 0)
					return;
				var lst = TUNING.BUILDINGS.PLANORDER[index].data as IList<string>;
				int after_index = lst.IndexOf(building_after);
				if (after_index < 0)
					return;
				if (after_index + 1 >= lst.Count)
					lst.Add(building_id);
				else
					lst.Insert(after_index + 1, building_id);
			}
		}

		[HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
		public static class ONIDenseGateConfigurator {
			private const string CATEGORY_AUTOMATION = "Automation";

			internal static void Prefix() {
				ModUtil.AddBuildingToPlanScreen(CATEGORY_AUTOMATION, DenseLogicGateConfig.ID);
				ModUtil.AddBuildingToPlanScreen(CATEGORY_AUTOMATION, DenseMultiplexerConfig.ID);
				ModUtil.AddBuildingToPlanScreen(CATEGORY_AUTOMATION, DenseDeMultiplexerConfig.ID);
				ModUtil.AddBuildingToPlanScreen(CATEGORY_AUTOMATION, SignalRemapperConfig.ID);
				AddBuildingToPlanScreen(CATEGORY_AUTOMATION, DenseInputConfig.ID,
					LogicSwitchConfig.ID);
				AddBuildingToPlanScreen(CATEGORY_AUTOMATION, LogicGateNorConfig.ID,
					LogicGateOrConfig.ID);
				AddBuildingToPlanScreen(CATEGORY_AUTOMATION, LogicGateNandConfig.ID,
					LogicGateAndConfig.ID);
				AddBuildingToPlanScreen(CATEGORY_AUTOMATION, LogicGateXnorConfig.ID,
					LogicGateXorConfig.ID);
				AddBuildingToPlanScreen(CATEGORY_AUTOMATION, LogicSevenSegmentConfig.ID,
					LogicCounterConfig.ID);
			}
		}

		private static void AddToTech(string tech, params string[] items) {
			string[] oldList = Techs.TECH_GROUPING[tech];
			string[] newList = new string[oldList.Length + items.Length];
			Array.Copy(oldList, newList, oldList.Length);
			Array.Copy(items, 0, newList, oldList.Length, items.Length);
			Techs.TECH_GROUPING[tech] = newList;
		}

		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class InitDenseGate {
			internal static void Prefix() {
				AddToTech("DupeTrafficControl", LogicGateXnorConfig.ID);
				AddToTech("Multiplexing", DenseMultiplexerConfig.ID, DenseDeMultiplexerConfig.ID, SignalRemapperConfig.ID);
				AddToTech("LogicCircuits", LogicGateNorConfig.ID, LogicGateNandConfig.ID);
				AddToTech("ParallelAutomation", DenseInputConfig.ID, DenseLogicGateConfig.ID, LogicSevenSegmentConfig.ID);
			}
		}
	}
}
