namespace VoxeltoUnity {
	using Moenen;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;


	// Sprite Part
	public partial class VoxelEditorWindow {




		#region --- VAR ---
		

		// Short
		private bool IsSpriting {
			get {
				return SpriteEditingRoot && SpriteEditingRoot.gameObject.activeSelf;
			}
		}



		// Data



		#endregion



		#region --- TSK ---




		private void CreateSprite (bool _8bit) {
			if (!Data) { return; }
			string path = Util.FixPath(EditorUtility.SaveFilePanel("Select Export Path", "Assets", Util.GetNameWithoutExtension(VoxelFilePath) + (_8bit ? "_8bit" : "_2D"), "png"));
			if (!string.IsNullOrEmpty(path)) {
				path = Util.FixedRelativePath(path);
				if (!string.IsNullOrEmpty(path)) {
					var result = _8bit ?
						EditorSpriteCore.Create8bitSprite(Data, CurrentModelIndex) :
						EditorSpriteCore.Create2DSprite(Data, CurrentModelIndex);
					if (result.Texture) {
						Util.ByteToFile(result.Texture.EncodeToPNG(), path);
						VoxelPostprocessor.AddSprite(path, new VoxelPostprocessor.SpriteConfig() {
							width = result.Width,
							height = result.Height,
							Pivots = result.Pivots,
							spriteRects = result.Rects,
							Names = result.NameFixes,
						});
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
						EditorApplication.delayCall += VoxelPostprocessor.ClearAsset;

					}

				} else {
					Util.Dialog("Warning", "Export path must in Assets folder.", "OK");
				}
			}
		}




		private void CreateScreenShot () {
			string path = Util.FixPath(EditorUtility.SaveFilePanel("Select Export Path", "Assets", Util.GetNameWithoutExtension(VoxelFilePath) + "_screenShot", "png"));
			if (!string.IsNullOrEmpty(path)) {
				path = Util.FixedRelativePath(path);
				if (!string.IsNullOrEmpty(path)) {
					bool oldShow = ShowBackgroundBox;
					CubeTF.gameObject.SetActive(false);
					SetBoxBackgroundActive(false);
					var texture = Util.RenderTextureToTexture2D(Camera);
					if (texture) {
						texture = Util.TrimTexture(texture, 0.01f, 12);
						Util.ByteToFile(texture.EncodeToPNG(), path);
						VoxelPostprocessor.AddScreenshot(path);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						EditorApplication.delayCall += VoxelPostprocessor.ClearAsset;
					}
					CubeTF.gameObject.SetActive(true);
					SetBoxBackgroundActive(oldShow);
				} else {
					Util.Dialog("Warning", "Export path must in Assets folder.", "OK");
				}
			}
		}



		#endregion



		#region --- LGC ---


		// Sprite Editing
		private void SpriteEditingGUI () {
			if (IsSpriting) {




			}
		}



		#endregion



	}
}