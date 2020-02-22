﻿/*
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

using KSerialization;
using UnityEngine;

namespace ONI_DenseLogic {
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class DenseInput : KMonoBehaviour, ISaveLoadable, IConfigurableFourBits {
		public static readonly HashedString OUTPUTID = new HashedString("DenseInput_OUT");

		private static readonly EventSystem.IntraObjectHandler<DenseInput>
			OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DenseInput>(
			(component, data) => component.OnCopySettings(data));

#pragma warning disable IDE0044 // Add readonly modifier
		[MyCmpReq]
		private KBatchedAnimController kbac;
		[MyCmpAdd]
		private CopyBuildingSettings copyBuildingSettings;
#pragma warning restore IDE0044

		[SerializeField]
		[Serialize]
		private int curOut;

		internal DenseInput() {
			curOut = 0;
		}

		protected override void OnSpawn() {
			base.OnSpawn();
			Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
			UpdateLogicCircuit();
		}

		private void OnCopySettings(object data) {
			DenseInput component = ((GameObject)data).GetComponent<DenseInput>();
			if (component == null) return;
			curOut = component.curOut;
			UpdateLogicCircuit();
		}

		private void UpdateLogicCircuit() {
			GetComponent<LogicPorts>().SendSignal(OUTPUTID, curOut);
			UpdateVisuals();
		}

		public void UpdateVisuals() {
			kbac.Play("on_" + curOut.ToString(), KAnim.PlayMode.Once, 1f, 0.0f);
		}

		public void SetBit(bool value, int pos) {
			curOut &= ~(1 << pos);
			if (value) {
				curOut |= 1 << pos;
			}
		}

		public bool GetBit(int pos) {
			return (curOut & (1 << pos)) > 0;
		}
	}
}