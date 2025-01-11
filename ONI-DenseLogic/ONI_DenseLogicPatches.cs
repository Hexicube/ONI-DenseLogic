/*
 * Copyright 2023 Dense Logic Team
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
using HarmonyLib;
using System.Collections.Generic;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using PeterHan.PLib.Database;

namespace ONI_DenseLogic {
	/// <summary>
	/// Patches which will be applied for Dense Logic.
	/// </summary>
	public sealed class ONI_DenseLogicPatches : KMod.UserMod2 {
		public override void OnLoad(Harmony harmony) {
			base.OnLoad(harmony);
			PUtil.InitLibrary();
			LocString.CreateLocStringKeys(typeof(DenseLogicStrings.BUILDINGS));
			LocString.CreateLocStringKeys(typeof(DenseLogicStrings.UI));
			new PLocalization().Register();
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class SideScreenCreator {
			internal static void Postfix() {
				PUIUtils.AddSideScreenContent<InlineGateSideScreen>();
				PUIUtils.AddSideScreenContent<LogicGateSelectSideScreen>();
				PUIUtils.AddSideScreenContent<FourBitSelectSideScreen>();
				PUIUtils.AddSideScreenContent<RemapperSideScreen>();
			}
		}

		[HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch {
			private static readonly HashedString CATEGORY_AUTOMATION = "Automation";
			private const string TAG_LOGIC = "logic gates";

			internal static void Prefix() {
				AddBuilding(CATEGORY_AUTOMATION, DenseMultiplexerConfig.ID, TAG_LOGIC);
				AddBuilding(CATEGORY_AUTOMATION, DenseDeMultiplexerConfig.ID, TAG_LOGIC);
				AddBuilding(CATEGORY_AUTOMATION, DenseLogicGateConfig.ID, TAG_LOGIC);
				AddBuilding(CATEGORY_AUTOMATION, DenseInputConfig.ID, TAG_LOGIC,
					LogicSwitchConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, LogicGateNorConfig.ID, TAG_LOGIC,
					LogicGateOrConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, LogicGateNandConfig.ID, TAG_LOGIC,
					LogicGateAndConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, LogicGateXnorConfig.ID, TAG_LOGIC,
					LogicGateXorConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, InlineLogicGateConfig.ID, TAG_LOGIC,
					LogicGateXnorConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, SignalRemapperConfig.ID, TAG_LOGIC,
					InlineLogicGateConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, LogicSevenSegmentConfig.ID, TAG_LOGIC,
					LogicCounterConfig.ID);
				AddBuilding(CATEGORY_AUTOMATION, LogicDataConfig.ID, TAG_LOGIC,
					LogicMemoryConfig.ID);
			}

			private static void AddBuilding(HashedString category, string id,
					string subCategory, string after = null) {
				ModUtil.AddBuildingToPlanScreen(category, id, subCategory, after);
				// The new TUNING.BUILDINGS.PLANSUBCATEGORYSORTING is not updated by
				// AddBuildingToPlanScreen
				var subcategories = TUNING.BUILDINGS.PLANSUBCATEGORYSORTING;
				if (subcategories != null)
					subcategories[id] = subCategory;
			}
		}

		private static void AddToTech(string tech, params string[] items) {
			var techs = Db.Get().Techs;
			techs.Get(tech).unlockedItemIDs.AddRange(items);
		}

		[HarmonyPatch(typeof(Techs), "Load")]
		public static class Techs_Load_Patch {
			internal static void Postfix() {
				AddToTech("DupeTrafficControl", LogicGateXnorConfig.ID, LogicDataConfig.ID);
				AddToTech("Multiplexing", DenseMultiplexerConfig.ID,
					DenseDeMultiplexerConfig.ID);
				AddToTech("LogicCircuits", LogicGateNorConfig.ID, LogicGateNandConfig.ID);
				AddToTech("ParallelAutomation", DenseInputConfig.ID, DenseLogicGateConfig.ID,
					LogicSevenSegmentConfig.ID, InlineLogicGateConfig.ID,
					SignalRemapperConfig.ID);
			}
		}

		[HarmonyPatch(typeof(LogicCircuitNetwork), "AddItem")]
		public static class LogicCircuitNetwork_AddItem_Patch {
			internal static void Postfix(object item, List<ILogicEventReceiver>
					___receivers) {
				if (item is ILogicEventSender sender) {
					ILogicEventReceiver handler;
					var go = Grid.Objects[sender.GetLogicCell(),
						(int)InlineLogicGateConfig.LAYER];
					// Check to see if it occupies an inline logic gate cell
					if (go != null && go.TryGetComponent(out InlineLogicGate gate) &&
							(handler = gate.InputHandler) != null)
						___receivers.Add(handler);
				}
			}
		}
		
		[HarmonyPatch(typeof(LogicCircuitNetwork), "RemoveItem")]
		public static class LogicCircuitNetwork_RemoveItem_Patch {
			internal static void Postfix(object item, List<ILogicEventReceiver>
					___receivers) {
				if (item is ILogicEventSender sender) {
					ILogicEventReceiver handler;
					var go = Grid.Objects[sender.GetLogicCell(),
						(int)InlineLogicGateConfig.LAYER];
					// Check to see if it occupies an inline logic gate cell
					if (go != null && go.TryGetComponent(out InlineLogicGate gate) &&
							(handler = gate.InputHandler) != null)
						___receivers.Remove(handler);
				}
			}
		}
		
		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public static class Assets_OnPrefabInit_Patch {
			private static readonly List<Ordering> swaps = new List<Ordering>() {
				new Ordering("DupeTrafficControl", LogicGateXnorConfig.ID, LogicGateXorConfig.ID),
				new Ordering("DupeTrafficControl", LogicDataConfig.ID, LogicMemoryConfig.ID),
				new Ordering("LogicCircuits", LogicGateNorConfig.ID, LogicGateOrConfig.ID),
				new Ordering("LogicCircuits", LogicGateNandConfig.ID, LogicGateAndConfig.ID),
				new Ordering("Multiplexing", DenseDeMultiplexerConfig.ID, DenseMultiplexerConfig.ID)
			};

			internal static void Postfix() {
				foreach (Tech tech in Db.Get().Techs.resources) {
					foreach (Ordering ordering in swaps) {
						if (ordering.tech != tech.Id)
							continue;
						TechItem removed = null;
						foreach (TechItem item in tech.unlockedItems) {
							if (ordering.id == item.Id)
								removed = item;
						}
						tech.unlockedItems.Remove(removed);
						int pos = -1;
						for (int i = 0; i < tech.unlockedItems.Count; i++) {
							TechItem item = tech.unlockedItems[i];
							if (ordering.id_after == item.Id)
								pos = i + 1;
						}
						tech.unlockedItems.Insert(pos, removed);
					}
				}
			}

			private sealed class Ordering {
				public readonly string tech;
				public readonly string id, id_after;

				public Ordering(string tech, string id, string id_after) {
					this.tech = tech;
					this.id = id;
					this.id_after = id_after;
				}
			}
		}
	}
}
