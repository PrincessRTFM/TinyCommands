using System;
using System.Diagnostics;
using System.Threading;

using Dalamud.Plugin;

namespace TinyCmds.Internal {
	internal static class Logger {
		private static string MessagePrefix {
			get {
				string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
				Thread runner = Thread.CurrentThread;
				string threadDesc = $"{runner.Name ?? "unknown thread"}#{runner.ManagedThreadId}";
				return $"[{timestamp}/{threadDesc}]";
			}
		}
		[Conditional("DEBUG")]
		internal static void Debug(string tmpl, params object[] args)
			=> PluginLog.Debug($"{MessagePrefix} {string.Format(tmpl, args)}");
		[Conditional("DEBUG")]
		internal static void Verbose(string tmpl, params object[] args)
			=> PluginLog.Verbose($"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Info(string tmpl, params object[] args)
			=> PluginLog.Information($"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Warning(string tmpl, params object[] args)
			=> PluginLog.Warning($"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Error(string tmpl, params object[] args)
			=> PluginLog.Error($"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Fatal(string tmpl, params object[] args)
			=> PluginLog.Fatal($"{MessagePrefix} {string.Format(tmpl, args)}");
		[Conditional("DEBUG")]
		internal static void Debug(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Debug(ex, $"{MessagePrefix} {string.Format(tmpl, args)}");
		[Conditional("DEBUG")]
		internal static void Verbose(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Verbose(ex, $"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Info(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Information(ex, $"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Warning(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Warning(ex, $"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Error(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Error(ex, $"{MessagePrefix} {string.Format(tmpl, args)}");
		internal static void Fatal(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Fatal(ex, $"{MessagePrefix} {string.Format(tmpl, args)}");
	}
}
