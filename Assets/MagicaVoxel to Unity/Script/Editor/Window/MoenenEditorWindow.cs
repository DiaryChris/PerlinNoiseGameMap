﻿namespace Moenen {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Moenen.Saving;

	public class MoenenEditorWindow : EditorWindow {





		#region --- SUB ---



		protected class Int3 {

			public int this[int axis] {
				get {
					return axis == 0 ? x : axis == 1 ? y : z;
				}
				set {
					if (axis == 0) {
						x = value;
					} else if (axis == 1) {
						y = value;
					} else {
						z = value;
					}
				}
			}

			public int x;
			public int y;
			public int z;
			public Int3 (int _x, int _y, int _z) {
				x = _x;
				y = _y;
				z = _z;
			}
			public static implicit operator Int3 (Vector3 v) {
				return new Int3(
					Mathf.RoundToInt(v.x),
					Mathf.RoundToInt(v.y),
					Mathf.RoundToInt(v.z)
				);
			}
			public static implicit operator bool (Int3 i) {
				return i != null;
			}
			public static Int3 Min (Int3 a, Int3 b) {
				return new Int3(
					Mathf.Min(a.x, b.x),
					Mathf.Min(a.y, b.y),
					Mathf.Min(a.z, b.z)
				);
			}
			public static Int3 Max (Int3 a, Int3 b) {
				return new Int3(
					Mathf.Max(a.x, b.x),
					Mathf.Max(a.y, b.y),
					Mathf.Max(a.z, b.z)
				);
			}
			public override string ToString () {
				return string.Format("({0}, {1}, {2})", x, y, z);
			}
		}



		#endregion




		protected static Rect GUIRect (float width, float height) {
			return GUILayoutUtility.GetRect(
				width, height,
				GUILayout.ExpandWidth(width == 0), GUILayout.ExpandHeight(height == 0)
			);
		}



		protected static void Link (int width, int height, string label, string link) {
			var buttonRect = GUIRect(width, height);
			if (GUI.Button(buttonRect, label, new GUIStyle(GUI.skin.label) {
				wordWrap = true,
				normal = new GUIStyleState() {
					textColor = new Color(86f / 256f, 156f / 256f, 214f / 256f),
					background = null,
					scaledBackgrounds = null,
				}
			})) {
				Application.OpenURL(link);
			}
			EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
		}



		protected static bool Button (int width, int height, string label, bool enabled = true, GUIStyle style = null) {
			bool oldE = GUI.enabled;
			GUI.enabled = enabled;
			if (style == null) { style = GUI.skin.button; }
			bool pressed = GUI.Button(GUIRect(width, height), label, style);
			GUI.enabled = oldE;
			return pressed;
		}



		protected static bool IntField (int width, int height, string label, ref int value, bool enabled = true) {
			bool oldE = GUI.enabled;
			GUI.enabled = enabled;
			bool changed = false;
			int valueInt = value;
			LayoutH(() => {
				GUI.Label(GUIRect(width, height), label);
				int newInt = EditorGUI.IntField(GUIRect(30, height), valueInt);
				if (newInt != valueInt) {
					valueInt = newInt;
					changed = true;
				}
			}, false, new GUIStyle() {
				fixedWidth = width + 30,
			});
			if (changed) {
				value = valueInt;
			}
			GUI.enabled = oldE;
			return changed;
		}



		protected static bool FloatField (int width, int height, string label, ref float value, bool enabled = true) {
			bool oldE = GUI.enabled;
			GUI.enabled = enabled;
			bool changed = false;
			float valueFloat = value;
			LayoutH(() => {
				GUI.Label(GUIRect(width, height), label);
				var newValue = EditorGUI.FloatField(GUIRect(30, height), valueFloat);
				if (newValue != valueFloat) {
					valueFloat = newValue;
					changed = true;
				}
			}, false, new GUIStyle() {
				fixedWidth = width + 30,
			});
			if (changed) {
				value = valueFloat;
			}
			GUI.enabled = oldE;
			return changed;
		}




		public static bool StringField (int width, int height, string label, ref string value, bool enabled = true) {
			bool oldE = GUI.enabled;
			GUI.enabled = enabled;
			bool changed = false;
			string valueStr = value;
			LayoutH(() => {
				GUI.Label(GUIRect(width, height), label);
				var newValue = EditorGUI.TextField(GUIRect(90, height), valueStr);
				if (newValue != valueStr) {
					valueStr = newValue;
					changed = true;
				}
			}, false, new GUIStyle() {
				fixedWidth = width + 90,
			});
			if (changed) {
				value = valueStr;
			}
			GUI.enabled = oldE;
			return changed;
		}



		protected static bool MinMaxField (int width, int labelWidth, int labelGap, int height, string label, string labelX, string labelY, ref Vector2 value, bool enabled = true) {
			bool oldE = GUI.enabled;
			GUI.enabled = enabled;
			bool changed = false;
			Vector2 valueVector2 = value;
			LayoutH(() => {
				GUI.Label(GUIRect(width, height), label);
				if (!string.IsNullOrEmpty(labelX)) {
					GUI.Label(GUIRect(labelWidth, height), labelX);
				}
				var newValueX = EditorGUI.FloatField(GUIRect(30, height), valueVector2.x);
				Space(labelGap);
				if (!string.IsNullOrEmpty(labelY)) {
					GUI.Label(GUIRect(labelWidth, height), labelY);
				}
				var newValueY = EditorGUI.FloatField(GUIRect(30, height), valueVector2.y);
				if (newValueX != valueVector2.x) {
					valueVector2.x = Mathf.Min(newValueX, valueVector2.y);
					changed = true;
				}
				if (newValueY != valueVector2.y) {
					valueVector2.y = Mathf.Max(newValueY, valueVector2.x);
					changed = true;
				}
			}, false, new GUIStyle() {
				fixedWidth = width + labelWidth + labelWidth + labelGap + 60,
			});
			if (changed) {
				value = valueVector2;
			}
			GUI.enabled = oldE;
			return changed;
		}



		protected static Color ColorField (int width, int height, string label, Color value, bool enabled = true) {
			bool oldE = GUI.enabled;
			GUI.enabled = enabled;
			LayoutH(() => {
				if (width > 0) {
					GUI.Label(GUIRect(width, height), label);
				}
#if UNITY_2017 || UNITY_5 || UNITY_4
				value = EditorGUI.ColorField(GUIRect(height, height), GUIContent.none, value, false, false, false, null);
#else
				value = EditorGUI.ColorField(GUIRect(height, height), GUIContent.none, value, false, false, false);
#endif
			}, false, new GUIStyle() {
				fixedWidth = width + height,
			});
			GUI.enabled = oldE;
			return value;
		}



		protected static void LayoutV (System.Action action, bool box = false, GUIStyle style = null) {
			if (box) {
				style = new GUIStyle(GUI.skin.box) {
					padding = new RectOffset(6, 6, 2, 2)
				};
			}
			if (style != null) {
				GUILayout.BeginVertical(style);
			} else {
				GUILayout.BeginVertical();
			}
			action();
			GUILayout.EndVertical();
		}



		protected static void LayoutH (System.Action action, bool box = false, GUIStyle style = null) {
			if (box) {
				style = new GUIStyle(GUI.skin.box) {
					padding = new RectOffset(6, 6, 2, 2)
				};
			}
			if (style != null) {
				GUILayout.BeginHorizontal(style);
			} else {
				GUILayout.BeginHorizontal();
			}
			action();
			GUILayout.EndHorizontal();
		}



		protected static void LayoutF (System.Action action, string label, EditorSavingBool open, bool box = false, GUIStyle style = null) {
			LayoutV(() => {
				open.Value = GUILayout.Toggle(
					open,
					label,
					GUI.skin.GetStyle("foldout"),
					GUILayout.ExpandWidth(true),
					GUILayout.Height(18)
				);
				if (open) {
					action();
				}
			}, box, style);
			Space(4);
		}



		protected static void AltLayoutF (System.Action action, string label, EditorSavingBool open, bool box = false, GUIStyle style = null) {
			LayoutV(() => {
				if (box) {
					style = GUI.skin.box;
				}
				Rect rect = GUIRect(0, 18);
				rect.x -= style == null ? 18 : style.padding.left;
				open.Value = GUI.Toggle(
					rect,
					open,
					label,
					GUI.skin.GetStyle("foldout")
				);
				if (open) {
					action();
				}
			}, box, style);
			Space(4);
		}



		protected static void LayoutF (System.Action action, string label, ref bool open, bool box = false, GUIStyle style = null) {
			bool _open = open;
			LayoutV(() => {
				_open = GUILayout.Toggle(
					_open,
					label,
					GUI.skin.GetStyle("foldout"),
					GUILayout.ExpandWidth(true),
					GUILayout.Height(18)
				);
				if (_open) {
					action();
				}
			}, box, style);
			Space(4);
			open = _open;
		}



		protected static void AltLayoutF (System.Action action, string label, ref bool open, bool box = false, GUIStyle style = null) {
			bool _open = open;
			LayoutV(() => {
				if (box) {
					style = GUI.skin.box;
				}
				Rect rect = GUIRect(0, 18);
				rect.x -= style == null ? 18 : style.padding.left;
				_open = GUI.Toggle(
					rect,
					_open,
					label,
					GUI.skin.GetStyle("foldout")
				);
				if (_open) {
					action();
				}
			}, box, style);
			Space(4);
			open = _open;
		}



		protected static void Space (float space = 4f) {
			GUILayout.Space(space);
		}



		protected static string GetDisplayString (string str, int maxLength) {
			return str.Length > maxLength ? str.Substring(0, maxLength - 3) + "..." : str;
		}



		protected static bool ColorfulButton (Rect rect, string label, Color color, GUIStyle style = null) {
			Color oldColor = GUI.color;
			GUI.color = color;
			bool pressed = style == null ? GUI.Button(rect, label) : GUI.Button(rect, label, style);
			GUI.color = oldColor;
			return pressed;
		}



		protected static void ColorBlock (Rect rect) {
			ColorBlock(rect, new Color(1, 1, 1, 0.1f));
		}



		protected static void ColorBlock (Rect rect, Color color) {
			var oldC = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(rect, Texture2D.whiteTexture);
			GUI.color = oldC;
		}


	}
}
