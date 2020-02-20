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

using KSerialization;
using UnityEngine;

namespace ONI_DenseLogic
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class DenseLogicGate : KMonoBehaviour, IRender200ms, IConfigurableLogicGate
    {
		public static readonly HashedString INPUTID1 = new HashedString("DenseGate_IN1");
		public static readonly HashedString INPUTID2 = new HashedString("DenseGate_IN2");
		public static readonly HashedString OUTPUTID = new HashedString("DenseGate_OUT");

		private static readonly EventSystem.IntraObjectHandler<DenseLogicGate>
            OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DenseLogicGate>(
            (component, data) => component.OnLogicValueChanged(data));

		private static readonly KAnimHashedString[] IN_A = { "in_a1", "in_a2", "in_a3", "in_a4" };
		private static readonly KAnimHashedString[] IN_B = { "in_b1", "in_b2", "in_b3", "in_b4" };
		private static readonly KAnimHashedString[] OUT = { "out_1", "out_2", "out_3", "out_4" };

		private static readonly KAnimHashedString GATE_OR = "or_gate";
		private static readonly KAnimHashedString GATE_AND = "and_gate";
		private static readonly KAnimHashedString GATE_XOR = "xor_gate";
        private static readonly KAnimHashedString GATE_XNOR = "xnor_gate";
        private static readonly KAnimHashedString GATE_NAND = "nand_gate";
        private static readonly KAnimHashedString GATE_NOR = "nor_gate";

        private static readonly KAnimHashedString[] LIGHTS =
            {
			"on_0", "on_1", "on_2", "on_3", "on_4", "on_5", "on_6", "on_7", "on_8",
			"on_9", "on_10", "on_11", "on_12", "on_13", "on_14", "on_15", "on_16", "on_17",
			"on_18", "on_19", "on_20", "on_21", "on_22", "on_23", "on_24", "on_25", "on_26"
		};

		public LogicGateType GateType
        {
			get
            {
				return mode;
			}
			set
            {
				mode = value;
				kbac.SetSymbolVisiblity(GATE_OR, mode == LogicGateType.Or);
				kbac.SetSymbolVisiblity(GATE_AND, mode == LogicGateType.And);
				kbac.SetSymbolVisiblity(GATE_XOR, mode == LogicGateType.Xor);
                kbac.SetSymbolVisiblity(GATE_XNOR, false);
                kbac.SetSymbolVisiblity(GATE_NAND, false);
                kbac.SetSymbolVisiblity(GATE_NOR, false);
                UpdateLogicCircuit();
			}
		}

		private Color colorOn = new Color(0.3411765f, 0.7254902f, 0.3686275f);
		private Color colorOff = new Color(0.9529412f, 0.2901961f, 0.2784314f);

#pragma warning disable IDE0044 // Add readonly modifier
        [MyCmpReq]
        private KBatchedAnimController kbac;
#pragma warning restore IDE0044

        [Serialize]
		private int inVal1, inVal2;
		private int curOut;
		[Serialize]
		private LogicGateType mode;

		protected override void OnSpawn()
        {
			base.OnSpawn();
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
            // Implicitly invokes the initial setup through the setter
			GateType = LogicGateType.And;
		}

		public void OnLogicValueChanged(object data)
        {
			var logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == INPUTID1)
                inVal1 = logicValueChanged.newValue;
			else if (logicValueChanged.portID == INPUTID2)
                inVal2 = logicValueChanged.newValue;
			else
                return;
			UpdateLogicCircuit();
		}

		private void UpdateLogicCircuit()
        {
			if (mode == LogicGateType.Or)
                curOut = inVal1 | inVal2;
			else if (mode == LogicGateType.And)
                curOut = inVal1 & inVal2;
			else if (mode == LogicGateType.Xor)
                curOut = inVal1 ^ inVal2;
			else
            {
                // should never occur
				Debug.Log("[DenseLogicGate] WARN: Unknown operand " + mode);
				curOut = 0;
			}
			GetComponent<LogicPorts>().SendSignal(OUTPUTID, curOut);
			UpdateVisuals();
		}

		public void Render200ms(float dt)
        {
            // hexi/test: Do we have to do this here? Can we render only on state change?
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
			for (int a = 0; a < 4; a++)
            {
				int mask = 1 << a;
				kbac.SetSymbolTint(IN_A[a], (inVal2 & mask) != 0 ? colorOn : colorOff);
				kbac.SetSymbolTint(IN_B[a], (inVal1 & mask) != 0 ? colorOn : colorOff);
				kbac.SetSymbolTint(OUT[a], (curOut & mask) != 0 ? colorOn : colorOff);
			}
		}
	}
}
