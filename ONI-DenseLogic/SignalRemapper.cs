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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ONI_DenseLogic {
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class SignalRemapper : KMonoBehaviour, ISaveLoadable {
		public static readonly HashedString INPUTID = new HashedString("SignalRemapper_IN");
		public static readonly HashedString OUTPUTID = new HashedString("SignalRemapper_OUT");

		public const int BITS = 4;
		public const int NO_BIT = -1;

		[Serialize]
		[SerializeField]
		private List<int> bits;

		internal SignalRemapper() {
			bits = null;
		}

		public int GetBitMapping(int bit) {
			int mapping = 0;
			if (bits != null && bit < bits.Count)
				mapping = bits[bit].InRange(NO_BIT, BITS - 1);
			return mapping;
		}

		protected override void OnPrefabInit() {
			base.OnPrefabInit();
			if (bits == null)
				bits = new List<int>(BITS);
			if (bits.Count <= BITS) {
				// Default config: all -1 (none)
				bits.Clear();
				for (int i = 0; i < BITS; i++)
					bits.Add(NO_BIT);
			}
		}

		public void SetBitMapping(int bit, int mapping) {
			if (bits != null && bit < bits.Count)
				bits[bit] = mapping.InRange(NO_BIT, BITS - 1);
		}
	}
}
