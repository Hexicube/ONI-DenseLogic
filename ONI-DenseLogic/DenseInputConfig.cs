using System.Collections.Generic;
using UnityEngine;

namespace ONI_DenseLogic {
	public class DenseInputConfig : IBuildingConfig {
		public const string ID = "DenseLogicTeam_DenseInput";

		public override BuildingDef CreateBuildingDef() {
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 1,
				"dense_INPUT_kanim", 30, 4.0f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
				TUNING.MATERIALS.REFINED_METALS, 200.0f, BuildLocationRule.Anywhere,
				TUNING.BUILDINGS.DECOR.NONE, TUNING.NOISE_POLLUTION.NONE);
			buildingDef.Overheatable = false;
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.PermittedRotations = PermittedRotations.R90;
			buildingDef.ViewMode = OverlayModes.Logic.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.AlwaysOperational = true;
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.RibbonOutputPort(
					DenseInput.OUTPUTID,
					new CellOffset(1, 0),
					STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.LOGIC_PORT_OUTPUT,
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_DENSEINPUT.PORTOUT_ACTIVE,
					DenseLogicStrings.BUILDINGS.PREFABS.DENSELOGICTEAM_DENSEINPUT.PORTOUT_INACTIVE
				)
			};
			GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go) {
			go.AddOrGet<DenseInput>();
		}
	}
}

