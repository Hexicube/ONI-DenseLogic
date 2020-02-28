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
using UnityEngine.UI;

namespace ONI_DenseLogic {
	internal sealed class RemapperSideScreen : SideScreenContent {
		private readonly string[] tooltips;
		private readonly IList<BitOption> bitNames;

		private readonly GameObject[] bitSelects;

		private SignalRemapper target;

		internal RemapperSideScreen() {
			tooltips = new string[SignalRemapper.BITS];
			bitNames = new List<BitOption>(1 + SignalRemapper.BITS);
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
			var margin = new RectOffset(4, 4, 4, 4);
			var border = new PPanel() {
				Margin = margin, Direction = PanelDirection.Vertical, FlexSize = Vector2.right,
				Alignment = TextAnchor.UpperCenter, Spacing = 4
			};
			tooltips[0] = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_1;
			tooltips[1] = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_2;
			tooltips[2] = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_3;
			tooltips[3] = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.BIT_4;
			// Can be safely shared
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_NONE);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_1);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_2);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_3);
			bitNames.Add(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.BIT_4);
			var rowBG = PUITuning.Images.GetSpriteByName("BitSelectorSideScreenRow");
			for (int i = 0; i < SignalRemapper.BITS; i++) {
				// This assignment is required for captures to get the right index
				int index = i;
				var row = new PPanel("Bit" + index) {
					BackImage = rowBG, BackColor = Color.white, ImageMode = Image.Type.Sliced,
					Alignment = TextAnchor.MiddleCenter, Direction = PanelDirection.Horizontal,
					Spacing = 10, Margin = margin, FlexSize = Vector2.right
				}.AddChild(new PLabel("BitLabel") {
					TextAlignment = TextAnchor.MiddleRight, ToolTip = tooltips[index],
					Text = string.Format(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.
					OUTPUT, bitNames[index + 1].GetProperName()), TextStyle = PUITuning.Fonts.
					TextDarkStyle
				});
				var cb = new PComboBox<BitOption>("BitSelect") {
					Content = bitNames, InitialItem = bitNames[0], ToolTip = tooltips[index],
					OnOptionSelected = (_, chosen) => SetSignalMap(index, chosen),
					TextStyle = PUITuning.Fonts.TextLightStyle, TextAlignment = TextAnchor.
					MiddleLeft
				};
				cb.OnRealize += (obj) => bitSelects[index] = obj;
				row.AddChild(cb);
				border.AddChild(row);
			}
			// Set default / Clear mapping
			border.AddChild(new PPanel("BottomRow") {
				Alignment = TextAnchor.MiddleCenter, Direction = PanelDirection.Horizontal,
				Spacing = 10, Margin = margin
			}.AddChild(new PButton() {
				Color = PUITuning.Colors.ButtonBlueStyle, Margin = new RectOffset(8, 8, 3, 3),
				TextStyle = PUITuning.Fonts.TextLightStyle, OnClick = SetDefaultMap,
				ToolTip = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.IDENTITY,
				Text = DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.IDENTITY
			}).AddChild(new PButton() {
				Color = PUITuning.Colors.ButtonBlueStyle, Margin = new RectOffset(8, 8, 3, 3),
				TextStyle = PUITuning.Fonts.TextLightStyle, OnClick = SetBlankMap,
				ToolTip = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.CLEAR,
				Text = DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.CLEAR
			}));
			ContentContainer = border.Build();
			base.OnPrefabInit();
			ContentContainer.SetParent(gameObject);
			LoadSignalMap();
		}

		private void SetBlankMap(GameObject _) {
			if (target != null)
				for (int i = 0; i < SignalRemapper.BITS; i++)
					SetSignalMap(i, bitNames[0]);
		}

		private void SetDefaultMap(GameObject _) {
			if (target != null)
				for (int i = 0; i < SignalRemapper.BITS; i++)
					SetSignalMap(i, bitNames[i + 1]);
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
