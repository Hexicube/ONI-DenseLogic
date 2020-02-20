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

namespace ONI_DenseLogic
{
    /// <summary>
    /// Strings used in Dense Logic.
    /// </summary>
    public static class DenseLogicStrings
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class DENSEGATE
                {
                    public static LocString NAME = STRINGS.UI.FormatAsLink("Dense Logic Gate",
                        DenseLogicGateConfig.ID);
                    public static LocString DESC = "Performs logic.";
                    public static LocString EFFECT = "Performs logic.";
                }

                public static class DENSEMULTIPLEXER
                {
                    public static LocString NAME = STRINGS.UI.FormatAsLink("Dense Multiplexer",
                        DenseMultiplexerConfig.ID);
                    public static LocString DESC = $"4-bit multiplexer with {STRINGS.UI.FormatAsLink("Ribbon Cable", LogicRibbonConfig.ID)} as input.";
                    public static LocString EFFECT = $"Controls which bit of the input {STRINGS.UI.FormatAsLink("Ribbon Cable", LogicRibbonConfig.ID)} is sent to the output using two control inputs.";
                }

                public static class DENSEDEMULTIPLEXER
                {
                    public static LocString NAME = STRINGS.UI.FormatAsLink("Dense DeMultiplexer",
                        DenseDeMultiplexerConfig.ID);
                    public static LocString DESC = $"4-bit multiplexer with {STRINGS.UI.FormatAsLink("Ribbon Cable", LogicRibbonConfig.ID)} as output.";
                    public static LocString EFFECT = $"Controls which bit of the output {STRINGS.UI.FormatAsLink("Ribbon Cable", LogicRibbonConfig.ID)} the input is sent to using two control inputs.";
                }
            }
        }
    }
}
