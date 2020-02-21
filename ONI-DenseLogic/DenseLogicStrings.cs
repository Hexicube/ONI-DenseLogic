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

using STRINGS;

namespace ONI_DenseLogic {
	/// <summary>
	/// Strings used in Dense Logic.
	/// </summary>
	public static class DenseLogicStrings {
		public static class BUILDINGS {
			public static class PREFABS {
				public static class DENSELOGICTEAM_DENSEGATE {
					public static LocString NAME = STRINGS.UI.FormatAsLink("Dense Logic Gate",
						DenseLogicGateConfig.ID);
					
					public static LocString DESC = $"Banhi told us she didn't like the old logic gates. Banhi is weird.";
					public static LocString EFFECT = $"Performs logic on each bit in {UI.FormatAsLink("Automation Ribbons", "LOGICRIBBON")} based on the specified mode.\n\nAND:\nOutputs {UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active)} when both Input A <b>AND</b> Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOR:\nOutputs {UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active)} when either Input A <b>OR</b> Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nXOR:\nOutputs {UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active)} when <b>EXACTLY ONE</b> of Input A and Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOutputs {UI.FormatAsAutomationState("Red Signals", UI.AutomationState.Standby)} if none of the above are true.";
					public static LocString PORTIN_ACTIVE = $"Reads {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)} signals from Automation Ribbons.";
					public static LocString PORTIN_INACTIVE = $"Reads {UI.FormatAsAutomationState("Red", UI.AutomationState.Standby)} signals from Automation Ribbons.";
					public static LocString PORTOUT_ACTIVE = $"Writes {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)} signals to Automation Ribbons.";
					public static LocString PORTOUT_INACTIVE = $"Writes {UI.FormatAsAutomationState("Red", UI.AutomationState.Standby)} signals to Automation Ribbons.";
				}
			}
		}
	}
}
