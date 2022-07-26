namespace TinyCmds.Internal;

using System;
using System.Runtime.InteropServices;

internal abstract class GameFunctionBase<T> where T : Delegate {
	private readonly IntPtr addr = IntPtr.Zero;
	public IntPtr Address => this.addr;
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
			Logger.error($"{this.GetType().Name} invocation FAILED: no pointer available");
			return null;
		}
	}
	internal GameFunctionBase(string sig, int offset = 0) {
		if (Plugin.scanner.TryScanText(sig, out this.addr)) {
			this.addr += offset;
			ulong totalOffset = (ulong)this.Address.ToInt64() - (ulong)Plugin.scanner.Module.BaseAddress.ToInt64();
			Logger.info($"{this.GetType().Name} loaded; address = 0x{this.Address.ToInt64():X16}, base memory offset = 0x{totalOffset:X16}");
		}
		else {
			Logger.warning($"{this.GetType().Name} FAILED, could not find address from signature: ${sig.ToUpper()}");
		}
	}
	internal GameFunctionBase(IntPtr address, int offset = 0) {
		this.addr = address + offset;
		ulong totalOffset = (ulong)this.Address.ToInt64() - (ulong)Plugin.scanner.Module.BaseAddress.ToInt64();
		Logger.info($"{this.GetType().Name} loaded; address = 0x{this.Address.ToInt64():X16}, base memory offset = 0x{totalOffset:X16}");
	}

	public dynamic? Invoke(params dynamic[] parameters)
		=> this.Delegate?.DynamicInvoke(parameters);
}
