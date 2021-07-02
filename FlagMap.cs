using System.Collections.Generic;

namespace TinyCmds {
	public class FlagMap: Dictionary<string, bool> {
		public new bool this[string key] {
			get {
				if (string.IsNullOrEmpty(key))
					return false;
				bool found = this.TryGetValue(key, out bool val);
				return found && val;
			}
			set {
				if (!string.IsNullOrEmpty(key)) {
					this.Remove(key);
					this.Add(key, value);
				}
			}
		}

		public void SetAll(IEnumerable<string> keys) {
			foreach (string key in keys) {
				this[key] = true;
			}
		}
		public void Set(params string[] keys) {
			this.SetAll(keys);
		}

		public void ToggleAll(IEnumerable<string> keys) {
			foreach (string key in keys) {
				this[key] = !this[key];
			}
		}
		public void Toggle(params string[] keys) {
			this.ToggleAll(keys);
		}

		public bool Get(string key) {
			return this[key];
		}
	}
}
