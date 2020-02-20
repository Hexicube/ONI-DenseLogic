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

namespace ONI_DenseLogic
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DenseMultiplexerBase : KMonoBehaviour, IRender200ms
    {
        public static readonly HashedString INPUTID = new HashedString("DenseGate_IN");
        public static readonly HashedString OUTPUTID = new HashedString("DenseGate_OUT");
        public static readonly HashedString CONTROLID1 = new HashedString("DenseMuxGate_CTRL1");
        public static readonly HashedString CONTROLID2 = new HashedString("DenseMuxGate_CTRL2");

        private static readonly EventSystem.IntraObjectHandler<DenseMultiplexerBase>
            OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DenseMultiplexerBase>(
            (component, data) => component.OnLogicValueChanged(data));

#pragma warning disable IDE0044 // Add readonly modifier
        [MyCmpReq]
        private KBatchedAnimController kbac;
#pragma warning restore IDE0044

        [Serialize]
        private int inVal;
        private int curOut;
        [Serialize]
        private int ctrlVal1, ctrlVal2;

        [Serialize]
        public MultiplexerType muxType;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
        }

        public void OnLogicValueChanged(object data)
        {
            var logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID == INPUTID)
                inVal = logicValueChanged.newValue;
            else if (logicValueChanged.portID == CONTROLID1)
                ctrlVal1 = logicValueChanged.newValue;
            else if (logicValueChanged.portID == CONTROLID2)
                ctrlVal2 = logicValueChanged.newValue;
            else
                return;
            UpdateLogicCircuit();
        }

        private int GetBitValue(int pos)
        {
            return inVal & 1 << pos;
        }

        private int SetBitValue(int pos, bool on)
        {
            return on ? 1 << pos : 0;
        }

        private void UpdateLogicCircuit()
        {
            if (muxType == MultiplexerType.MUX)
            {
                curOut = GetBitValue(ctrlVal1 + 2 * ctrlVal2) > 0 ? 1 : 0;
            }
            else if (muxType == MultiplexerType.DEMUX)
            {
                curOut = SetBitValue(ctrlVal1 + 2 * ctrlVal2, inVal > 0);
            }
            else
            {
                // should never occur
                Debug.Log("[DenseMultiplexerBase] WARN: Unknown multiplexer type " + muxType);
                curOut = 0;
            }
            GetComponent<LogicPorts>().SendSignal(OUTPUTID, curOut);
            UpdateVisuals();
        }

        public void Render200ms(float dt)
        {
            // hexi/test/peter: Do we have to do this here? Can we render only on state change?
            UpdateVisuals();
        }

        private int GetRibbonValue(int wire)
        {
            if (wire == 0)
            {
                return 0;
            }
            else if (wire == 0b1111)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        private int GetSingleValue(int wire)
        {
            if (wire == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public void UpdateVisuals()
        {
            int num0 = 0;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            if (muxType == MultiplexerType.MUX)
            {
                num0 = GetRibbonValue(inVal);
                num1 = GetSingleValue(ctrlVal1);
                num2 = GetSingleValue(ctrlVal2);
                num3 = GetSingleValue(curOut);
            }
            else if (muxType == MultiplexerType.DEMUX)
            {
                num0 = GetRibbonValue(curOut);
                num1 = GetSingleValue(ctrlVal1);
                num2 = GetSingleValue(ctrlVal2);
                num3 = GetSingleValue(inVal);
            }
            else
            {
                // should never occur
                Debug.Log("[DenseMultiplexerBase] WARN: Unknown multiplexer type " + muxType);
            }
            int state = num0 + 3 * num1 + 6 * num2 + 12 * num3;
            kbac.Play("on_" + state, KAnim.PlayMode.Once, 1f, 0.0f);
        }

        public enum MultiplexerType
        {
            MUX, DEMUX
        }
    }
}
