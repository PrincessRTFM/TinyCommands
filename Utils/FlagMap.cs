using System.Collections.Generic;
using System.Linq;

namespace TinyCmds.Utils {
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

		public void SetAll(IEnumerable<char> keys) {
			this.SetAll(keys.Select(c => c.ToString()));
		}
		public void Set(params char[] keys) {
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

		public void ToggleAll(IEnumerable<char> keys) {
			this.ToggleAll(keys.Select(c => c.ToString()));
		}
		public void Toggle(params char[] keys) {
			this.ToggleAll(keys);
		}

		public bool Get(string key) {
			return this[key];
		}
		public bool Get(char key) {
			return this.Get(key.ToString());
		}
	}
}
