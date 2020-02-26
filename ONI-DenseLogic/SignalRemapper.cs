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
using PeterHan.PLib;
using System.Collections.Generic;
using UnityEngine;

namespace ONI_DenseLogic {
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class SignalRemapper : KMonoBehaviour, ISaveLoadable, IRender200ms {
		public static readonly HashedString INPUTID = new HashedString("SignalRemapper_IN");
		public static readonly HashedString OUTPUTID = new HashedString("SignalRemapper_OUT");

		public static readonly CellOffset INPUTOFFSET = new CellOffset(0, 0);
		public static readonly CellOffset OUTPUTOFFSET = new CellOffset(1, 0);

		private static readonly EventSystem.IntraObjectHandler<SignalRemapper>
			OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<SignalRemapper>(
			(component, data) => component.OnLogicValueChanged(data));

		public const int BITS = 4;
		public const int NO_BIT = -1;

#pragma warning disable IDE0044 // Add readonly modifier
		[MyCmpReq]
		private KBatchedAnimController kbac;
#pragma warning restore IDE0044


		[Serialize]
		private int inVal;
		private int curOut;

		[Serialize]
		[SerializeField]
		private List<int> bits;

		private int GetActualCell(CellOffset offset) {
			Rotatable component = GetComponent<Rotatable>();
			if (component != null)
				offset = component.GetRotatedCellOffset(offset);
			return Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), offset);
		}

		internal SignalRemapper() {
			bits = null;
		}

		protected override void OnSpawn() {
			base.OnSpawn();
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
		}

		protected override void OnPrefabInit() {
			base.OnPrefabInit();
			if (bits == null)
				bits = new List<int>(BITS);
			if (bits.Count <= BITS) {
				// Default config: all -1 (none)
				bits.Clear();
				for (int i = 0; i < BITS; i++)
					bits.Add(NO_BIT);
			}
		}

		public void OnLogicValueChanged(object data) {
			var logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == INPUTID)
				inVal = logicValueChanged.newValue;
			else
				return;
			UpdateLogicCircuit();
		}

		public void SetBit(bool value, int pos) {
			curOut &= ~(1 << pos);
			if (value) {
				curOut |= 1 << pos;
			}
		}

		public bool GetBit(int pos) {
			return (inVal & (1 << pos)) > 0;
		}

		private void UpdateLogicCircuit() {
			bool num0 = GetBit(GetBitMapping(0));
			bool num1 = GetBit(GetBitMapping(1));
			bool num2 = GetBit(GetBitMapping(2));
			bool num3 = GetBit(GetBitMapping(3));
			curOut = 0;
			SetBit(num0, 0);
			SetBit(num1, 1);
			SetBit(num2, 2);
			SetBit(num3, 3);
			GetComponent<LogicPorts>().SendSignal(OUTPUTID, curOut);
			UpdateVisuals();
		}

		public void Render200ms(float dt) {
			// hexi/test/peter: Do we have to do this here? Can we render only on state change?
			UpdateVisuals();
		}

		private int GetRibbonValue(int wire) {
			if (wire == 0) {
				return 0;
			} else if (wire == 0b1111) {
				return 2;
			} else {
				return 1;
			}
		}

		public void UpdateVisuals() {
			// when there is not an output, we are supposed to play the off animation
			if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(GetActualCell(OUTPUTOFFSET)) is LogicCircuitNetwork)) {
				kbac.Play("off", KAnim.PlayMode.Once, 1f, 0.0f);
				return;
			}
			int num0 = GetRibbonValue(inVal);
			int num1 = GetRibbonValue(curOut);
			int state = num0 + 3 * num1;
			kbac.Play("on_" + state, KAnim.PlayMode.Once, 1f, 0.0f);
		}

		public int GetBitMapping(int bit) {
			int mapping = 0;
			if (bits != null && bit < bits.Count)
				mapping = bits[bit].InRange(NO_BIT, BITS - 1);
			return mapping;
		}

		public void SetBitMapping(int bit, int mapping) {
			if (bits != null && bit < bits.Count) {
				bits[bit] = mapping.InRange(NO_BIT, BITS - 1);
				UpdateLogicCircuit();
			}
		}
	}
}
