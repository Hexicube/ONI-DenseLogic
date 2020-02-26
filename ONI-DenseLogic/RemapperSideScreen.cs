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
			tooltips = new string[4];
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
			var borderMargin = new RectOffset(1, 1, 0, 1);
			var border = new PPanel() {
				Margin = borderMargin,
				Direction = PanelDirection.Vertical,
				FlexSize = new Vector2(1.0f, 1.0f),
				BackColor = new Color(0.0f, 0.0f, 0.0f),
				Alignment = TextAnchor.UpperCenter
			};
			var margin = new RectOffset(4, 4, 4, 4);
			var ss = new PPanel() {
				Margin = margin,
				Direction = PanelDirection.Vertical,
				FlexSize = new Vector2(1.0f, 1.0f),
				BackColor = new Color(1.0f, 1.0f, 1.0f),
				Alignment = TextAnchor.UpperCenter
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
			for (int i = 0; i < SignalRemapper.BITS; i++) {
				ss.AddChild(new RemapperRow(i, this).Row);
			}
			ss.AddChild(new PPanel() {
				Margin = margin,
				Direction = PanelDirection.Horizontal,
				FlexSize = new Vector2(1.0f, 1.0f),
				Alignment = TextAnchor.UpperCenter,
			}.AddChild(new PSpacer() { FlexSize = new Vector2(1.0f, 0.0f) })
			.AddChild(new PButton() {
				Color = PUITuning.Colors.ButtonBlueStyle,
				Margin = new RectOffset(8, 8, 3, 3),
				TextStyle = PUITuning.Fonts.TextLightStyle,
				ToolTip = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.IDENTITY,
				Text = DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.IDENTITY,
				OnClick = obj => {
					if (target != null) {
						SetSignalMap(0, bitNames[1]);
						SetSignalMap(1, bitNames[2]);
						SetSignalMap(2, bitNames[3]);
						SetSignalMap(3, bitNames[4]);
					}
				}
			}).AddChild(new PSpacer() { FlexSize = new Vector2(0.5f, 0.0f) })
			.AddChild(new PButton() {
				Color = PUITuning.Colors.ButtonBlueStyle,
				Margin = new RectOffset(8, 8, 3, 3),
				TextStyle = PUITuning.Fonts.TextLightStyle,
				ToolTip = DenseLogicStrings.UI.TOOLTIPS.SIGNALREMAPPER.CLEAR,
				Text = DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.CLEAR,
				OnClick = obj => {
					if (target != null) {
						SetSignalMap(0, bitNames[0]);
						SetSignalMap(1, bitNames[0]);
						SetSignalMap(2, bitNames[0]);
						SetSignalMap(3, bitNames[0]);
					}
				}
			}).AddChild(new PSpacer() { FlexSize = new Vector2(1.0f, 0.0f) }));
			border.AddChild(ss);
			ContentContainer = border.Build();
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

		private sealed class RemapperRow {
			private RemapperSideScreen parent;

			/// <summary>
			/// The panel containing this row
			/// </summary>
			public PPanel Row { get; }

			public RemapperRow(int index, RemapperSideScreen parent) {
				// TODO This could probably be done as a relative layout eventually, but
				// until PRelativePanel is added this is the best we can do
				this.parent = parent;
				Row = new PPanel("RemapperRow") {
					Margin = new RectOffset(4, 4, 4, 4),
					FlexSize = new Vector2(1.0f, 0.0f),
				};
				var RowBackground = new PPanel("RemapperRowBackground") {
					FlexSize = new Vector2(1.0f, 0.0f)
				};
				RowBackground.OnRealize += gameObject => {
					var img = gameObject.AddComponent<KImage>();
					img.sprite = PUITuning.Images.GetSpriteByName("BitSelectorSideScreenRow");
					img.type = Image.Type.Sliced;
					img.color = new Color(1.0f, 1.0f, 1.0f);
				};
				Row.AddChild(RowBackground);
				var RowInternal = new PPanel("RemapperRowInternal") {
					Margin = new RectOffset(4, 4, 4, 4),
					FlexSize = new Vector2(1.0f, 0.0f),
				};
				RowInternal.OnRealize += gameObject => {
					var img = gameObject.AddComponent<KImage>();
					img.sprite = PUITuning.Images.GetSpriteByName("overview_highlight_outline_sharp");
					img.type = Image.Type.Sliced;
					img.color = new Color(0.898f, 0.898f, 0.898f);
				};
				RowBackground.AddChild(RowInternal);
				var RowInternalGrid = new PGridPanel("RemapperRowInternalGrid") {
					Margin = new RectOffset(4, 4, 4, 4),
					FlexSize = new Vector2(1.0f, 0.0f),
				};
				RowInternalGrid.AddRow(new GridRowSpec());
				RowInternalGrid
					.AddColumn(new GridColumnSpec(flex: 0.33f)).AddColumn(new GridColumnSpec())
					.AddColumn(new GridColumnSpec(flex: 0.33f)).AddColumn(new GridColumnSpec())
					.AddColumn(new GridColumnSpec(flex: 0.33f));
				RowInternal.AddChild(RowInternalGrid);
				RowInternalGrid.AddChild(new PSpacer(), new GridComponentSpec(0, 0));
				RowInternalGrid.AddChild(new PLabel("Label") {
					TextAlignment = TextAnchor.MiddleRight,
					ToolTip = parent.tooltips[index],
					Text = string.Format(DenseLogicStrings.UI.UISIDESCREENS.SIGNALREMAPPER.
					OUTPUT, parent.bitNames[index + 1].GetProperName()),
					TextStyle = PUITuning.Fonts.TextDarkStyle
				}, new GridComponentSpec(0, 1));
				RowInternalGrid.AddChild(new PSpacer(), new GridComponentSpec(0, 2));
				var cb = new PComboBox<BitOption>("Select") {
					Content = parent.bitNames,
					InitialItem = parent.bitNames[0],
					ToolTip = parent.tooltips[index],
					OnOptionSelected = (_, chosen) => parent.SetSignalMap(index, chosen),
					TextStyle = PUITuning.Fonts.TextLightStyle,
					DynamicSize = true
				};
				cb.OnRealize += (obj) => parent.bitSelects[index] = obj;
				RowInternalGrid.AddChild(cb, new GridComponentSpec(0, 3));
				RowInternalGrid.AddChild(new PSpacer(), new GridComponentSpec(0, 4));
			}
		}
	}
}
