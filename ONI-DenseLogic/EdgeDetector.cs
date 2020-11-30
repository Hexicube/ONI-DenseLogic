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
	public sealed class EdgeDetector : KMonoBehaviour, ISaveLoadable {
		public static readonly HashedString INPUTID = new HashedString("EdgeDetector_IN");
		public static readonly HashedString OUTPUTID = new HashedString("EdgeDetector_OUT");

		public static readonly CellOffset INPUTOFFSET = new CellOffset(0, 0);
		public static readonly CellOffset OUTPUTOFFSET = new CellOffset(1, 1);

		private static readonly EventSystem.IntraObjectHandler<EdgeDetector>
			OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<EdgeDetector>(
			(component, data) => component.OnLogicValueChanged(data));

		private static readonly Color COLOR_ON = new Color(0.3411765f, 0.7254902f, 0.3686275f);
		private static readonly Color COLOR_OFF = new Color(0.9529412f, 0.2901961f, 0.2784314f);
		private static readonly Color COLOR_DISABLED = new Color(1.0f, 1.0f, 1.0f);
		
		private static readonly KAnimHashedString[] IN_LINE = { "in_1", "in_2", "in_3", "in_4" };
		private static readonly KAnimHashedString[] OUT_LINE = { "out_1", "out_2", "out_3", "out_4" };

#pragma warning disable IDE0044, CS0649 // Add readonly modifier
		[MyCmpReq]
		private KBatchedAnimController kbac;

		[MyCmpReq]
		private LogicPorts ports;

		[MyCmpGet]
		private Rotatable rotatable;
#pragma warning restore IDE0044, CS0649

		[Serialize]
		private int inVal;
		[Serialize]
		private int lastTickInVal;
		private int curOut;

		private int GetActualCell(CellOffset offset) {
			if (rotatable != null)
				offset = rotatable.GetRotatedCellOffset(offset);
			return Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), offset);
		}

		private MeterController piston1, piston2, piston3, piston4;

		// AnimTarg: Symbol name for animation
		// AnimName: Animation name for animation
		private MeterController MakePistonController(int id) {
			return new MeterController((KAnimControllerBase)kbac, "piston_"+id+"_target", "piston_on", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]{ "piston_"+id+"_target" });
		}

		// This is over here to make it easy to verify names match.
		private void DoPiston(MeterController piston, bool start, bool end) {
			if (start == end) return;
			if (start) piston.meterController.Play("piston_turn_off");
			else piston.meterController.Play("piston_turn_on");
		}

		// Note: We initially subscribe to the tick event just in case a save happened during a pulse, otherwise that pulse may stick.

		protected override void OnSpawn() {
			base.OnSpawn();
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
			Game.Instance.logicCircuitManager.onLogicTick += OnLogicTick;

			kbac.Play("on");
			UpdateVisuals();

			// TODO: position these
			piston1 = MakePistonController(1);
			piston2 = MakePistonController(2);
			piston3 = MakePistonController(3);
			piston4 = MakePistonController(4);
		}

		private bool added = true;

		protected override void OnCleanUp() {
			base.OnCleanUp();
			if (added) Game.Instance.logicCircuitManager.onLogicTick -= OnLogicTick;
		}

		protected override void OnPrefabInit() {
			base.OnPrefabInit();
			UpdateVisuals();
		}

		public void OnLogicTick() {
			bool change = false;
			if (inVal != lastTickInVal) {
				curOut = (inVal ^ lastTickInVal);
				change = true;
			}
			else if (curOut != 0) {
				curOut = 0;
				change = true;
			}
			if (change) {
				ports.SendSignal(OUTPUTID, curOut);
				PerformAnim();
				UpdateVisuals();
				lastTickInVal = inVal;
			}
			else if (added) {
				added = false;
				Game.Instance.logicCircuitManager.onLogicTick -= OnLogicTick;
			}
		}

		public void OnLogicValueChanged(object data) {
			var logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == INPUTID) {
				inVal = logicValueChanged.newValue;
				if (!added) {
					added = true;
					Game.Instance.logicCircuitManager.onLogicTick += OnLogicTick;
				}
			}
		}

		private string Light(int pos, int state) {
			return $"light_bloom_{pos}_{state}";
		}

		private bool BitOn(int wire, int pos) {
			return (wire & (0x1 << pos)) > 0;
		}

		public void PerformAnim() {
			DoPiston(piston1, (lastTickInVal & 1) > 0, (inVal & 1) > 0);
			DoPiston(piston2, (lastTickInVal & 2) > 0, (inVal & 2) > 0);
			DoPiston(piston3, (lastTickInVal & 4) > 0, (inVal & 4) > 0);
			DoPiston(piston4, (lastTickInVal & 8) > 0, (inVal & 8) > 0);
			// NOTE: This may break if fed signal pulses, the animation will assume 2 logic ticks minimum.
			// TODO: Is there a better method?
			// NOT IMPLEMENTED: kbac.Play(lastTickInVal+"to"+inVal, KAnim.PlayMode.Once, 1f, 0.0f);
		}

		public void UpdateVisuals() {
			int cell = GetActualCell(OUTPUTOFFSET);
			// when there is not an output, we are supposed to play the off animation
			if (Game.Instance.logicCircuitSystem.GetNetworkForCell(cell) is LogicCircuitNetwork) {
				// set the tints for the wiring bits on the edges of the remapping (not the central connectors)
				for (int i = 0; i < DenseLogicGate.NUM_BITS; i++) {
					kbac.SetSymbolTint(IN_LINE[i], BitOn(inVal, i) ? COLOR_ON : COLOR_OFF);
					kbac.SetSymbolTint(OUT_LINE[i], BitOn(curOut, i) ? COLOR_ON : COLOR_OFF);
				}

				// turn off all of the lights (there are two pairs of lights, one on each side)
				/*for (int i = 0; i < DenseLogicGate.NUM_BITS * 2; i++) {
					kbac.SetSymbolVisiblity(Light(i, 0), false);
					kbac.SetSymbolVisiblity(Light(i, 1), false);
				}
				// turn on only the lights that should be shown (pick green vs red based on the values of the logic wires)
				for (int i = 0; i < DenseLogicGate.NUM_BITS; i++) {
					kbac.SetSymbolVisiblity(Light(i, BitOn(inVal, i) ? 0 : 1), true);
					kbac.SetSymbolVisiblity(Light(4 + i, BitOn(curOut, i) ? 0 : 1), true);
				}*/
			} else {
				// set symbol tints for the wiring bits on the edges of the remapping to off tinting
				// don't need to worry about symbol visibility here b/c the "off" animation is completely separate from the "on" animation
				for (int i = 0; i < DenseLogicGate.NUM_BITS; i++) {
					kbac.SetSymbolTint(IN_LINE[i], COLOR_DISABLED);
					kbac.SetSymbolTint(OUT_LINE[i], COLOR_DISABLED);
				}
			}
		}
	}
}
