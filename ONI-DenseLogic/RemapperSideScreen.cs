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

using PeterHan.PLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ONI_DenseLogic {
	internal sealed class RemapperSideScreen : SideScreenContent {
		private readonly IList<BitOption> bitNames;

		private readonly GameObject[] bitSelects;

		private SignalRemapper target;

		internal RemapperSideScreen() {
			bitNames = new List<BitOption>();
			bitSelects = new GameObject[SignalRemapper.BITS];
			target = null;
		}

		public override void ClearTarget() {
			target = null;
		}

		public override string GetTitle() {
			return DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.TITLE;
		}

		public override bool IsValidForTarget(GameObject target) {
			return target.GetComponentSafe<SignalRemapper>() != null;
		}

		private void LoadSignalMap() {
			if (target != null)
				for (int i = 0; i < SignalRemapper.BITS; i++) {
					var bs = bitSelects[i];
					// relies on NO_BIT being -1
					int bit = target.GetBitMapping(i) + 1;
					if (bs != null)
						PComboBox<BitOption>.SetSelectedItem(bs, bitNames[bit], false);
				}
		}

		protected override void OnPrefabInit() {
			RectOffset lblMargin = new RectOffset(0, 10, 0, 0), rowMargin = new RectOffset(0,
				0, 5, 5);
			var ss = new PGridPanel("SignalRemap") {
				Margin = new RectOffset(4, 4, 4, 4), FlexSize = Vector2.right
			}.AddColumn(new GridColumnSpec(flex: 0.5f)).
				AddColumn(new GridColumnSpec(flex: 0.5f));
			var tooltips = new string[] {
				DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_1,
				DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_2,
				DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_3,
				DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_4,
			};
			// Can be safely shared
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_NONE);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_1);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_2);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_3);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_4);
			for (int i = 0; i < SignalRemapper.BITS; i++) {
				// Important! This assignment is required to ensure that the closure created
				// for OnRealize binds the right value of i
				int index = i;
				ss.AddRow(new GridRowSpec()).AddChild(new PLabel("Label") {
					TextAlignment = TextAnchor.MiddleRight, ToolTip = tooltips[index],
					Text = string.Format(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.
					OUTPUT, bitNames[index + 1].GetProperName()),
					TextStyle = PUITuning.Fonts.TextDarkStyle
				}, new GridComponentSpec(index, 0) {
					Alignment = TextAnchor.MiddleRight, Margin = lblMargin
				});
				var cb = new PComboBox<BitOption>("Select") {
					Content = bitNames, InitialItem = bitNames[0], ToolTip = tooltips[index],
					OnOptionSelected = (_, chosen) => SetSignalMap(index, chosen),
					TextStyle = PUITuning.Fonts.TextLightStyle, DynamicSize = true
				};
				cb.OnRealize += (obj) => bitSelects[index] = obj;
				ss.AddChild(cb, new GridComponentSpec(index, 1) {
					Alignment = TextAnchor.MiddleLeft, Margin = rowMargin
				});
			}
			ContentContainer = ss.Build();
			base.OnPrefabInit();
			ContentContainer.SetParent(gameObject);
			LoadSignalMap();
		}

		private void SetSignalMap(int index, BitOption chosen) {
			var bs = bitSelects[index];
			if (bs != null && target != null && chosen != null) {
				// relies on NO_BIT being -1
				target.SetBitMapping(index, bitNames.IndexOf(chosen).InRange(0,
					SignalRemapper.BITS) - 1);
				PComboBox<BitOption>.SetSelectedItem(bs, chosen, false);
			}
		}

		public override void SetTarget(GameObject target) {
			this.target = target.GetComponentSafe<SignalRemapper>();
			LoadSignalMap();
		}

		private sealed class BitOption : IListableOption {
			public static implicit operator BitOption(LocString name) => new BitOption(name);

			private readonly string name;

			public BitOption(string name) {
				this.name = name ?? throw new ArgumentNullException("name");
			}

			public string GetProperName() {
				return name;
			}
		}
	}
}
