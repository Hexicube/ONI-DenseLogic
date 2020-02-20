using Database;
using Harmony;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ONI_DenseLogic
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class ONIDenseGateConfigurator
    {
        public static void Prefix()
        {
            string prefix = "STRINGS.BUILDINGS.PREFABS." + DenseLogicGateConfig.ID.ToUpper();
            Strings.Add(prefix + ".NAME", UI.FormatAsLink("Dense Logic Gate", DenseLogicGateConfig.ID));
            Strings.Add(prefix + ".DESC", "Performs logic.");
            Strings.Add(prefix + ".EFFECT", "Performs logic.");
            ModUtil.AddBuildingToPlanScreen("Automation", DenseLogicGateConfig.ID);
        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class InitDenseGate
    {
        public static void Prefix(Db __instance)
        {
            List<string> list = new List<string>(Techs.TECH_GROUPING["DupeTrafficControl"]) { DenseLogicGateConfig.ID };
            Techs.TECH_GROUPING["DupeTrafficControl"] = list.ToArray();
        }
    }

    public class DenseLogicGateConfig : IBuildingConfig {
        public const string ID = "DenseGate";

        public override BuildingDef CreateBuildingDef()
        {
            int width = 2;
            int height = 3;
            string anim = "dense_MULTI_kanim";
            int hitpoints = 30;
            float construction_time = 4f;
            float melting_point = 800f;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                ID, width, height, anim,
                hitpoints, construction_time, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS,
                melting_point, BuildLocationRule.LogicBridge, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER1);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>() {
                LogicPorts.Port.RibbonInputPort(
                    DenseLogicGate.INPUTID1,
                    new CellOffset(0, -1),
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_INACTIVE,
                    true
                ),
                LogicPorts.Port.RibbonInputPort(
                    DenseLogicGate.INPUTID2,
                    new CellOffset(0, 1),
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_INACTIVE,
                    true
                )
            };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>() {
                LogicPorts.Port.RibbonOutputPort(
                    DenseLogicGate.OUTPUTID,
                    new CellOffset(1, 0),
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT_OUTPUT,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.OUTPUT_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.OUTPUT_PORT_INACTIVE,
                    true
                )
            };
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<DenseLogicGate>();
        }
    }
    
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DenseLogicGate : KMonoBehaviour, IRender200ms
    {
        public static readonly HashedString INPUTID1 = new HashedString("DenseGate_IN1");
        public static readonly HashedString INPUTID2 = new HashedString("DenseGate_IN2");
        public static readonly HashedString OUTPUTID = new HashedString("DenseGate_OUT");

        private static readonly EventSystem.IntraObjectHandler<DenseLogicGate> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DenseLogicGate>((component, data) => component.OnLogicValueChanged(data));
        
        private KAnimHashedString[] IN_A = { "in_a1", "in_a2", "in_a3", "in_a4" };
        private KAnimHashedString[] IN_B = { "in_b1", "in_b2", "in_b3", "in_b4" };
        private KAnimHashedString[] OUT = { "out_1", "out_2", "out_3", "out_4" };
        
        private KAnimHashedString GATE_OR = "gate";
        private KAnimHashedString GATE_AND = "and_gate";
        private KAnimHashedString GATE_XOR = "xor_gate";

        private KAnimHashedString[] LIGHTS = {
            "on_0", "on_1", "on_2", "on_3", "on_4", "on_5", "on_6", "on_7", "on_8",
            "on_9", "on_10", "on_11", "on_12", "on_13", "on_14", "on_15", "on_16", "on_17",
            "on_18", "on_19", "on_20", "on_21", "on_22", "on_23", "on_24", "on_25", "on_26"
        };

        private Color colorOn = new Color(0.3411765f, 0.7254902f, 0.3686275f);
        private Color colorOff = new Color(0.9529412f, 0.2901961f, 0.2784314f);

        private KBatchedAnimController kbac;
        private LogicPorts ports;

        enum SwitchMode {
            AND, OR, XOR
        }
        
        [Serialize]
        private int inVal1, inVal2;
        private int curOut;
        [Serialize]
        private SwitchMode mode = SwitchMode.AND;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
            ports = GetComponent<LogicPorts>();
            kbac = GetComponent<KBatchedAnimController>();
            UpdateLogicCircuit();
            kbac.SetSymbolVisiblity(GATE_OR, mode == SwitchMode.OR);
            kbac.SetSymbolVisiblity(GATE_AND, mode == SwitchMode.AND);
            kbac.SetSymbolVisiblity(GATE_XOR, mode == SwitchMode.XOR);
        }

        public void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID == INPUTID1) inVal1 = logicValueChanged.newValue;
            else if (logicValueChanged.portID == INPUTID2) inVal2 = logicValueChanged.newValue;
            else return;
            UpdateLogicCircuit();
        }

        private void UpdateLogicCircuit()
        {
            if (mode == SwitchMode.OR) curOut = inVal1 | inVal2;
            else if (mode == SwitchMode.AND) curOut = inVal1 & inVal2;
            else if (mode == SwitchMode.XOR) curOut = inVal1 ^ inVal2;
            else { // should never occur
                Debug.Log("[DenseLogicGate] WARN: Unknown operand " + mode);
                curOut = 0;
            }
            GetComponent<LogicPorts>().SendSignal(OUTPUTID, curOut);
            UpdateVisuals();
        }

        public void Render200ms(float dt)
        {
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            int state = 0;
            if (inVal1 == 15) state += 2;
            else if (inVal1 > 0) state += 1;
            if (inVal2 == 15) state += 6;
            else if (inVal2 > 0) state += 3;
            if (curOut == 15) state += 18;
            else if (curOut > 0) state += 9;
            kbac.Play(LIGHTS[state], KAnim.PlayMode.Once, 1f, 0.0f);
            for (int a = 0; a < 4; a++) {
                int mask = 1 << a;
                kbac.SetSymbolTint(IN_A[a], (inVal2 & mask) != 0 ? colorOn : colorOff);
                kbac.SetSymbolTint(IN_B[a], (inVal1 & mask) != 0 ? colorOn : colorOff);
                kbac.SetSymbolTint(OUT[a], (curOut & mask) != 0 ? colorOn : colorOff);
            }
        }
    }
}
