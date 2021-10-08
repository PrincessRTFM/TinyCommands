using System;
using System.Diagnostics;
using System.Threading;

using Dalamud.Logging;

namespace TinyCmds.Internal {
	internal static class Logger {
		private static string msgPrefix {
			get {
				string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
				Thread runner = Thread.CurrentThread;
				string threadDesc = $"{runner.Name ?? "unknown thread"}#{runner.ManagedThreadId}";
				return $"[{timestamp}/{threadDesc}]";
			}
		}
		[Conditional("DEBUG")]
		internal static void debug(string tmpl, params object[] args)
			=> PluginLog.Debug($"{msgPrefix} {string.Format(tmpl, args)}");
		[Conditional("DEBUG")]
		internal static void verbose(string tmpl, params object[] args)
			=> PluginLog.Verbose($"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void info(string tmpl, params object[] args)
			=> PluginLog.Information($"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void warning(string tmpl, params object[] args)
			=> PluginLog.Warning($"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void error(string tmpl, params object[] args)
			=> PluginLog.Error($"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void fatal(string tmpl, params object[] args)
			=> PluginLog.Fatal($"{msgPrefix} {string.Format(tmpl, args)}");
		[Conditional("DEBUG")]
		internal static void debug(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Debug(ex, $"{msgPrefix} {string.Format(tmpl, args)}");
		[Conditional("DEBUG")]
		internal static void verbose(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Verbose(ex, $"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void info(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Information(ex, $"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void warning(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Warning(ex, $"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void error(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Error(ex, $"{msgPrefix} {string.Format(tmpl, args)}");
		internal static void fatal(Exception ex, string tmpl, params object[] args)
			=> PluginLog.Fatal(ex, $"{msgPrefix} {string.Format(tmpl, args)}");
	}
}
