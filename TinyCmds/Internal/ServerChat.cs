using System;
using System.Runtime.InteropServices;
using System.Text;

using Dalamud.Game;

using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;

using FFXIVClientStructs.FFXIV.Client.UI;

namespace VariableVixen.TinyCmds.Internal;

internal class ServerChat {
	private static class Signatures {
		internal const string SendChat = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B F2 48 8B F9 45 84 C9";
		internal const string SanitiseString = "E8 ?? ?? ?? ?? EB 0A 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D AE";
	}


	private delegate void ProcessChatBoxDelegate(nint uiModule, nint message, nint unused, byte a4);
	private readonly unsafe delegate* unmanaged<Utf8String*, int, nint, void> sanitiseString = null!;

	private ProcessChatBoxDelegate? processChatBox { get; }

	internal unsafe ServerChat(ISigScanner scanner) {
		if (scanner.TryScanText(Signatures.SendChat, out nint processChatBoxPtr)) {
			this.processChatBox = Marshal.GetDelegateForFunctionPointer<ProcessChatBoxDelegate>(processChatBoxPtr);
			Plugin.Log?.Information("Found signature for chat sending");
		}
		else {
			Plugin.Log?.Error("Can't find signature for chat sending, functionality will be unavailable!");
		}

		if (scanner.TryScanText(Signatures.SanitiseString, out nint sanitisePtr)) {
			this.sanitiseString = (delegate* unmanaged<Utf8String*, int, nint, void>)sanitisePtr;
			Plugin.Log?.Information("Found signature for chat sanitisation");
		}
		else {
			Plugin.Log?.Error("Can't find signature for chat sanitisation, functionality will be unavailable!");
		}
	}

	public unsafe void SendMessageUnsafe(byte[] message) {
		if (this.processChatBox == null)
			throw new InvalidOperationException("Could not find signature for chat sending");

		UIModule* uiModule = Framework.Instance()->GetUIModule();
		if (uiModule is null)
			throw new InvalidOperationException("Could not access UIModule instance", new NullReferenceException("Framework instance returned null for UIModule"));

		using ChatPayload payload = new(message);
		nint mem1 = Marshal.AllocHGlobal(400);
		Marshal.StructureToPtr(payload, mem1, false);

		this.processChatBox((nint)uiModule, mem1, nint.Zero, 0);

		Marshal.FreeHGlobal(mem1);
	}

	public void SendMessage(string message) {
		byte[] bytes = Encoding.UTF8.GetBytes(message);
		if (bytes.Length == 0)
			throw new ArgumentException("message is empty", nameof(message));

		if (bytes.Length > 500)
			throw new ArgumentException("message is longer than 500 bytes", nameof(message));

		if (message.Length != this.SanitiseText(message).Length)
			throw new ArgumentException("message contained invalid characters", nameof(message));

		this.SendMessageUnsafe(bytes);
	}

	public unsafe string SanitiseText(string text) {
		if (this.sanitiseString == null)
			throw new InvalidOperationException("Could not find signature for chat sanitisation");

		Utf8String* uText = Utf8String.FromString(text);

		this.sanitiseString(uText, 0x27F, nint.Zero);
		string sanitised = uText->ToString();

		uText->Dtor();
		IMemorySpace.Free(uText);

		return sanitised;
	}



	[StructLayout(LayoutKind.Explicit)]
	private readonly struct ChatPayload: IDisposable {
		[FieldOffset(0)]
		private readonly nint textPtr;

		[FieldOffset(16)]
		private readonly ulong textLen;

		[FieldOffset(8)]
		private readonly ulong unk1;

		[FieldOffset(24)]
		private readonly ulong unk2;

		internal ChatPayload(byte[] stringBytes) {
			this.textPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
			Marshal.Copy(stringBytes, 0, this.textPtr, stringBytes.Length);
			Marshal.WriteByte(this.textPtr + stringBytes.Length, 0);

			this.textLen = (ulong)(stringBytes.Length + 1);

			this.unk1 = 64;
			this.unk2 = 0;
		}

		public void Dispose() => Marshal.FreeHGlobal(this.textPtr);
	}


}
