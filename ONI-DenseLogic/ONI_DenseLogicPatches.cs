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
using PeterHan.PLib;
using PeterHan.PLib.UI;

namespace ONI_DenseLogic {
	/// <summary>
	/// Patches which will be applied for Dense Logic.
	/// </summary>
	public static class ONI_DenseLogicPatches {
		public static void OnLoad() {
			PUtil.InitLibrary();
			LocString.CreateLocStringKeys(typeof(DenseLogicStrings.BUILDINGS));
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class SideScreenCreator {
			internal static void Postfix() {
				PUIUtils.AddSideScreenContent<LogicGateSelectSideScreen>();
			}
		}

		[HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
		public static class ONIDenseGateConfigurator {
			internal static void Prefix() {
				ModUtil.AddBuildingToPlanScreen("Automation", DenseLogicGateConfig.ID);
				ModUtil.AddBuildingToPlanScreen("Automation", DenseMultiplexerConfig.ID);
				ModUtil.AddBuildingToPlanScreen("Automation", DenseDeMultiplexerConfig.ID);
			}
		}

		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class InitDenseGate {
			internal static void Prefix() {
				Techs.TECH_GROUPING["DupeTrafficControl"].Append(DenseLogicGateConfig.ID);
				Techs.TECH_GROUPING["DupeTrafficControl"].Append(DenseMultiplexerConfig.ID);
				Techs.TECH_GROUPING["DupeTrafficControl"].Append(DenseDeMultiplexerConfig.ID);
			}
		}
	}
}
