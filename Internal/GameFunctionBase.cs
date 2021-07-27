using System;
using System.Runtime.InteropServices;

using Dalamud.Game;

namespace TinyCmds.Internal {
	internal abstract class GameFunctionBase<T> where T : Delegate {
		public IntPtr Address { get; private set; }
		private T? function;
		public bool Valid => this.function is not null || this.Address != IntPtr.Zero;
		public T? Delegate {
			get {
				if (this.function is not null)
					return this.function;
				if (this.Address != IntPtr.Zero) {
					this.function = Marshal.GetDelegateForFunctionPointer<T>(this.Address);
					return this.function;
				}
				Logger.Error($"{this.GetType().Name} invocation FAILED: no pointer available");
				return null;
			}
		}
		internal GameFunctionBase(SigScanner scanner, string sig, int offset = 0) {
			this.Address = scanner.ScanText(sig);
			if (this.Address != IntPtr.Zero) {
				this.Address += offset;
				ulong totalOffset = (ulong) this.Address.ToInt64() - (ulong) scanner.Module.BaseAddress.ToInt64();
				Logger.Debug($"{this.GetType().Name} loaded; address = 0x{this.Address.ToInt64():X16}, base memory offset = 0x{totalOffset:X16}");
			}
			else {
				Logger.Warning($"{this.GetType().Name} FAILED, could not find address from signature: ${sig.ToUpper()}");
			}
		}
		public dynamic? Invoke(params dynamic[] parameters)
			=> this.Delegate?.DynamicInvoke(parameters);
	}
}
