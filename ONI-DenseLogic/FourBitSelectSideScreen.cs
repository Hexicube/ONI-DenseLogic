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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using STRINGS;

namespace ONI_DenseLogic {
	/// <summary>
	/// Controls the logic behind the side screen for selecting the bit values
	/// of a building with configurable bits.
	/// </summary>
	internal sealed class FourBitSelectSideScreen : SideScreenContent {

		/// <summary>
		/// The number of bits that can be set/visualized.
		/// </summary>
		public const int NUM_BITS = 4;

		public readonly Vector2 MIN_PANEL_SIZE = new Vector2(300, 250);

		private Color activeColor = new Color(0.3411765f, 0.7254902f, 0.3686275f);
		private Color inactiveColor = new Color(0.9529412f, 0.2901961f, 0.2784314f);

		/// <summary>
		/// The bit toggles in the UI.
		/// </summary>
		private readonly Dictionary<int, BitSelectRow> toggles;

		/// <summary>
		/// The selected building with bits to modify/visualize.
		/// </summary>
		private IConfigurableFourBits target;

		internal FourBitSelectSideScreen() {
			toggles = new Dictionary<int, BitSelectRow>();
			target = null;
		}

		public override void ClearTarget() {
			target = null;
		}

		public override string GetTitle() {
			return "Set Provided Signal";
		}

		public override bool IsValidForTarget(GameObject target) {
			return target.GetComponent<IConfigurableFourBits>() != null;
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
			for (int i = 0; i < NUM_BITS; i++) {
				toggles[i] = new BitSelectRow(i);
				ss.AddChild(toggles[i].Row);
			}
			border.AddChild(ss);
			ContentContainer = border.Build();
			/*foreach (KeyValuePair<int, BitSelectRow> keyValuePair in toggles) {
				keyValuePair.Value.Toggle.onClick += () => {
					if (target != null) {
						target.SetBit(!target.GetBit(keyValuePair.Key), keyValuePair.Key);
					}
				};
			}*/
			ContentContainer.SetMinUISize(MIN_PANEL_SIZE);
			base.OnPrefabInit();
			ContentContainer.SetParent(gameObject);
			RefreshToggles();
		}

		public override void SetTarget(GameObject target) {
			if (target == null) {
				PUtil.LogError("Invalid gameObject received");
				return;
			}
			this.target = target.GetComponent<IConfigurableFourBits>();
			if (this.target == null) {
				PUtil.LogError("The gameObject received is not an IConfigurableFourBits");
				return;
			}
			RefreshToggles();
		}

		/// <summary>
		/// Updates the state of the bit toggles, based on the state in the target.
		/// </summary>
		private void RefreshToggles() {
			if (target == null) {
				return;
			}
			foreach (KeyValuePair<int, BitSelectRow> keyValuePair in toggles) {
				bool bitOn = target.GetBit(keyValuePair.Key);
				keyValuePair.Value.StateIcon.color = 
					bitOn ? activeColor : inactiveColor;
				keyValuePair.Value.StateText.SetText(
					bitOn ? UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_ACTIVE : UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_INACTIVE);
			}
		}

		private sealed class BitSelectRow {
			/// <summary>
			/// The panel containing this row
			/// </summary>
			public PPanel Row { get;  }

			public Image StateIcon;

			public LocText BitName;

			public LocText StateText;

			public BitSelectRow(int pos) {
				Row = new PPanel("BitSelectRow") {
					Margin = new RectOffset(4, 4, 4, 4),
					FlexSize = new Vector2(1.0f, 0.0f),
				};
				var RowInternal = new PPanel("BitSelectRowInternal") {
					Margin = new RectOffset(4, 4, 4, 4),
					FlexSize = new Vector2(1.0f, 0.0f),
					BackColor = new Color(0.898f, 0.898f, 0.898f)
				};
				Row.AddChild(RowInternal);
				var RowInternalGrid = new PGridPanel("BitSelectRowInternalGrid") {
					Margin = new RectOffset(4, 4, 4, 4),
					FlexSize = new Vector2(1.0f, 0.0f),
					BackColor = new Color(1.0f, 1.0f, 1.0f)
				};
				RowInternalGrid.AddRow(new GridRowSpec());
				RowInternalGrid.AddColumn(new GridColumnSpec()).AddColumn(new GridColumnSpec())
					.AddColumn(new GridColumnSpec(flex: 0.5f)).AddColumn(new GridColumnSpec()).AddColumn(new GridColumnSpec(flex: 1.0f));
				RowInternal.AddChild(RowInternalGrid);
				var stateIcon = new PLabel("StateIcon") {
					Margin = new RectOffset(6, 0, 6, 6),
					Sprite = PUITuning.Images.GetSpriteByName("web_box_shadow"),
					SpriteSize = new Vector2(32, 32),
					TextAlignment = TextAnchor.MiddleLeft
				};
				stateIcon.OnRealize += gameObject => {
					StateIcon = gameObject.GetComponentInChildren<Image>();
					StateIcon.type = Image.Type.Sliced;
				};
				RowInternalGrid.AddChild(stateIcon, new GridComponentSpec(0, 0));
				var bitName = new PLabel("BitName") {
					Margin = new RectOffset(24, 0, 0, 0),
					Text = string.Format(UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.BIT, pos + 1),
					TextStyle = PUITuning.Fonts.TextDarkStyle,
					TextAlignment = TextAnchor.MiddleLeft
				};
				bitName.OnRealize += gameObject => BitName = gameObject.GetComponentInChildren<LocText>();
				RowInternalGrid.AddChild(bitName, new GridComponentSpec(0, 1));
				RowInternalGrid.AddChild(new PSpacer(), new GridComponentSpec(0, 2));
				var stateText = new PLabel("StateText") {
					Text = UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_INACTIVE,
					TextStyle = PUITuning.Fonts.TextDarkStyle,
					TextAlignment = TextAnchor.MiddleRight
				};
				stateText.OnRealize += gameObject => StateText = gameObject.GetComponentInChildren<LocText>();
				RowInternalGrid.AddChild(stateText, new GridComponentSpec(0, 3));
				RowInternalGrid.AddChild(new PSpacer(), new GridComponentSpec(0, 4));
			}

		}
	}
}
