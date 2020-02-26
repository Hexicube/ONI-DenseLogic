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

namespace ONI_DenseLogic {
	[SerializationConfig(MemberSerialization.OptIn)]
	public class LogicGate : KMonoBehaviour, IRender200ms {
		public static readonly HashedString INPUTID1 = new HashedString("LogicGate_IN1");
		public static readonly HashedString INPUTID2 = new HashedString("LogicGate_IN2");
		public static readonly HashedString OUTPUTID = new HashedString("LogicGate_OUT");

		public static readonly CellOffset INPUTOFFSET1 = new CellOffset(0, 0);
		public static readonly CellOffset INPUTOFFSET2 = new CellOffset(0, 1);
		public static readonly CellOffset OUTPUTOFFSET = new CellOffset(1, 0);

		private static readonly EventSystem.IntraObjectHandler<LogicGate>
				OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicGate>(
				(component, data) => component.OnLogicValueChanged(data));

#pragma warning disable IDE0044 // Add readonly modifier
		[MyCmpReq]
		private KBatchedAnimController kbac;
#pragma warning restore IDE0044

		[Serialize]
		private int inVal1, inVal2;
		private int curOut;

		[Serialize]
		public LogicGateType gateType;

		private int GetActualCell(CellOffset offset) {
			Rotatable component = GetComponent<Rotatable>();
			if (component != null)
				offset = component.GetRotatedCellOffset(offset);
			return Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), offset);
		}

		protected override void OnSpawn() {
			base.OnSpawn();
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
		}

		public void OnLogicValueChanged(object data) {
			var logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == INPUTID1)
				inVal1 = logicValueChanged.newValue;
			else if (logicValueChanged.portID == INPUTID2)
				inVal2 = logicValueChanged.newValue;
			else
				return;
			UpdateLogicCircuit();
		}

		private void UpdateLogicCircuit() {
			if (gateType == LogicGateType.Or)
				curOut = inVal1 | inVal2;
			else if (gateType == LogicGateType.And)
				curOut = inVal1 & inVal2;
			else if (gateType == LogicGateType.Xor)
				curOut = inVal1 ^ inVal2;
			else if (gateType == LogicGateType.Nor)
				curOut = ~(inVal1 | inVal2);
			else if (gateType == LogicGateType.Nand)
				curOut = ~(inVal1 & inVal2);
			else if (gateType == LogicGateType.Xnor)
				curOut = ~(inVal1 ^ inVal2);
			else {
				// should never occur
				Debug.Log("[DenseLogicGate] WARN: Unknown operand " + gateType);
				curOut = 0;
			}
			GetComponent<LogicPorts>().SendSignal(OUTPUTID, curOut);
			UpdateVisuals();
		}

		public void Render200ms(float dt) {
			// hexi/test/peter: Do we have to do this here? Can we render only on state change?
			UpdateVisuals();
		}

		private int GetSingleValue(int wire) {
			return wire & 0b1;
		}

		public void UpdateVisuals() {
			// when there is not an output, we are supposed to play the off animation
			if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(GetActualCell(OUTPUTOFFSET)) is LogicCircuitNetwork)) {
				kbac.Play("off", KAnim.PlayMode.Once, 1f, 0.0f);
				return;
			}
			int num0 = GetSingleValue(inVal1);
			int num1 = GetSingleValue(inVal2);
			int num2 = GetSingleValue(curOut);
			int state = num0 + 2 * num1 + 4 * num2;
			kbac.Play("on_" + state, KAnim.PlayMode.Once, 1f, 0.0f);
		}
	}
}
