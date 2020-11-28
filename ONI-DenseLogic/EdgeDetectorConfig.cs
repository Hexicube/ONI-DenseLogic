/*
 * Copyright 2020 Dense Logic Team
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

using System.Collections.Generic;
using UnityEngine;

namespace ONI_DenseLogic {
	public class EdgeDetectorConfig : IBuildingConfig {
		public const string ID = "DenseLogicTeam_EdgeDetector";

		public override BuildingDef CreateBuildingDef() {
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2,
				"dense_SWITCH_kanim", 30, 4.0f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
				TUNING.MATERIALS.REFINED_METALS, 200.0f, BuildLocationRule.Anywhere,
				TUNING.BUILDINGS.DECOR.NONE, TUNING.NOISE_POLLUTION.NONE);
			buildingDef.Overheatable = false;
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.ViewMode = OverlayModes.Logic.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.ObjectLayer = ObjectLayer.LogicGate;
			buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
			buildingDef.ThermalConductivity = 0.05f;
			buildingDef.BaseTimeUntilRepair = -1.0f;
			buildingDef.AlwaysOperational = true;
			buildingDef.LogicInputPorts = new List<LogicPorts.Port>() {
				LogicPorts.Port.RibbonInputPort(
					EdgeDetector.INPUTID, CellOffset.none,
					STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT,
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_EDGEDETECTOR.PORTIN_ACTIVE,
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_EDGEDETECTOR.PORTIN_INACTIVE
				)
			};
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.RibbonOutputPort(
					EdgeDetector.OUTPUTID, new CellOffset(1, 1),
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_EDGEDETECTOR.LOGIC_PORT_OUTPUT,
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_EDGEDETECTOR.PORTOUT_ACTIVE,
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_EDGEDETECTOR.PORTOUT_INACTIVE
				)
			};
			GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go) {
			go.AddOrGet<EdgeDetector>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}
	}
}

