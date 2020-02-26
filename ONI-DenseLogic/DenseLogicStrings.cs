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
					public static LocString NAME = UI.FormatAsLink("Dense Multigate",
						DenseLogicGateConfig.ID);
					public static LocString DESC = $"Banhi told us she didn't like the old logic gates. Banhi is weird.";
					public static LocString EFFECT = $"Switches between different logic functions easily in the UI.\n\nPerforms logic on each bit in {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} based on the specified mode.\n\nAND:\nOutputs {UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active)} when both Input A <b>AND</b> Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOR:\nOutputs {UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active)} when either Input A <b>OR</b> Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nXOR:\nOutputs {UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active)} when <b>EXACTLY ONE</b> of Input A and Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOutputs {UI.FormatAsAutomationState("Red Signals", UI.AutomationState.Standby)} if none of the above are true.";
					public static LocString PORTIN_ACTIVE = $"Reads {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)} signals from Automation Ribbons.";
					public static LocString PORTIN_INACTIVE = $"Reads {UI.FormatAsAutomationState("Red", UI.AutomationState.Standby)} signals from Automation Ribbons.";
					public static LocString PORTOUT_ACTIVE = $"Writes {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)} signals to Automation Ribbons.";
					public static LocString PORTOUT_INACTIVE = $"Writes {UI.FormatAsAutomationState("Red", UI.AutomationState.Standby)} signals to Automation Ribbons.";
				}

				public static class DENSELOGICTEAM_LOGICSEVENSEGMENT {
					public static LocString NAME = UI.FormatAsLink("Seven Segment Display",
						LogicSevenSegmentConfig.ID);
					public static LocString DESC = $"Giving the duplicants a way to show off their favorite numbers has never been easier.";
					public static LocString EFFECT = $"Displays the value of a {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} as a number 0 through 9.";
					public static LocString LOGIC_PORT = "Numerical Input";
					public static LocString INPUT_PORT_ACTIVE = $"Displays a non-zero number.";
					public static LocString INPUT_PORT_INACTIVE = $"Displays zero.";
					public static LocString OUTPUT_LOGIC_PORT = "Overflow";
					public static LocString OUTPUT_PORT_ACTIVE = $"The input is larger than 9.";
					public static LocString OUTPUT_PORT_INACTIVE = $"The input is less than or equal to 9.";
				}

				public static class DENSELOGICTEAM_LOGICNOR {
					public static LocString NAME = UI.FormatAsLink("NOR Gate",
						LogicGateNorConfig.ID);
					public static LocString DESC = $"This gate outputs a Red Signal if receiving one or more Green Signals.";
					public static LocString EFFECT = $"Outputs a {UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)} if at least one of Input A <b>OR</b> Input B is receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOutputs a {UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)} when neither Input A or Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.";
				}

				public static class DENSELOGICTEAM_LOGICNAND {
					public static LocString NAME = UI.FormatAsLink("NAND Gate",
						LogicGateNorConfig.ID);
					public static LocString DESC = $"This gate outputs a Red Signal if receiving two Green Signals.";
					public static LocString EFFECT = $"Outputs a {UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)} if both Input A <b>AND</b> Input B are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOutputs a {UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)} when even one Input is receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.";
				}

				public static class DENSELOGICTEAM_LOGICXNOR {
					public static LocString NAME = UI.FormatAsLink("XNOR Gate",
						LogicGateNorConfig.ID);
					public static LocString DESC = $"This gate outputs a Red Signal if exactly one of its Inputs is receiving a Green Signal.";
					public static LocString EFFECT = $"Outputs a {UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)} if exactly one of its Inputs is receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.\n\nOutputs a {UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)} if both or neither Inputs are receiving {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)}.";
				}

				public static class DENSELOGICTEAM_DENSEMULTIPLEXER {
					public static LocString NAME = UI.FormatAsLink("Dense Multiplexer",
						DenseMultiplexerConfig.ID);
					public static LocString DESC = $"4-bit multiplexer with {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} as input.";
					public static LocString EFFECT = $"Controls which bit of the input {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} is sent to the output using two control inputs.";
				}

				public static class DENSELOGICTEAM_DENSEDEMULTIPLEXER {
					public static LocString NAME = UI.FormatAsLink("Dense DeMultiplexer",
						DenseDeMultiplexerConfig.ID);
					public static LocString DESC = $"4-bit multiplexer with {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} as output.";
					public static LocString EFFECT = $"Controls which bit of the output {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} the input is sent to using two control inputs.";
				}

				public static class DENSELOGICTEAM_DENSEINPUT {
					public static LocString NAME = UI.FormatAsLink("Signal Multiswitch",
						DenseInputConfig.ID);
					public static LocString DESC = $"4-bit constant input with {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)} as output.";
					public static LocString EFFECT = $"Sends a configurable 4-bit signal on an {UI.FormatAsLink("Automation", "LOGIC")} grid.\n\n{STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME} must be used as the output wire to avoid overloading."; 
					public static LocString PORTOUT_ACTIVE = $"Writes {UI.FormatAsAutomationState("Green", UI.AutomationState.Active)} signals to some bits of the {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)}.";
					public static LocString PORTOUT_INACTIVE = $"Writes {UI.FormatAsAutomationState("Red", UI.AutomationState.Standby)} signals to some bits of the {UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LOGICRIBBON.NAME, LogicRibbonConfig.ID)}.";
				}
			}
		}
	}
}
