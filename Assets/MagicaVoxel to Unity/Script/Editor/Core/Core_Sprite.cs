namespace VoxeltoUnity {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public static class EditorSpriteCore {





		#region --- SUB ---



		public enum Direction {
			Up = 0,
			Down = 1,
			Front = 2,
			Back = 3,
			Left = 4,
			Right = 5,
		}



		public enum FaceType {
			Up_0 = 0,
			Up_22 = 1,
			Up_45 = 2,
			Up_67 = 3,
			Left_0 = 4,
			Left_22 = 5,
			Left_45 = 6,
			Left_67 = 7,
			Front_0 = 8,
			Front_22 = 9,
			Front_45 = 10,
			Front_67 = 11,
		}



		public class Result {
			public int Width;
			public int Height;
			public string[] NameFixes;
			public Texture2D Texture;
			public Vector2[] Pivots;
			public Rect[] Rects;
		}



		public struct Voxel {

			public int ColorIndex {
				get;
				set;
			}

			public bool IsEmpty {
				get {
					return ColorIndex == 0;
				}
			}

			public bool IsVisible {
				get {
					return Visible != null && Visible.Length > 5 && (Visible[0] || Visible[1] || Visible[2] || Visible[3] || Visible[4] || Visible[5]);
				}
				set {
					Visible[0] = value;
					Visible[1] = value;
					Visible[2] = value;
					Visible[3] = value;
					Visible[4] = value;
					Visible[5] = value;
				}
			}

			public bool VisibleLeft {
				get {
					return Visible[(int)Direction.Left];
				}
				set {
					Visible[(int)Direction.Left] = value;
				}
			}
			public bool VisibleRight {
				get {
					return Visible[(int)Direction.Right];
				}
				set {
					Visible[(int)Direction.Right] = value;
				}
			}
			public bool VisibleUp {
				get {
					return Visible[(int)Direction.Up];
				}
				set {
					Visible[(int)Direction.Up] = value;
				}
			}
			public bool VisibleDown {
				get {
					return Visible[(int)Direction.Down];
				}
				set {
					Visible[(int)Direction.Down] = value;
				}
			}
			public bool VisibleFront {
				get {
					return Visible[(int)Direction.Front];
				}
				set {
					Visible[(int)Direction.Front] = value;
				}
			}
			public bool VisibleBack {
				get {
					return Visible[(int)Direction.Back];
				}
				set {
					Visible[(int)Direction.Back] = value;
				}
			}
			public bool[] Visible {
				get;
				private set;
			}

			public void Init () {
				ColorIndex = 0;
				Visible = new bool[6] { false, false, false, false, false, false };
			}

		}



		private struct VoxelFace {
			public Vector3 Position;
			public Vector3 Normal;
			public Color Color;
			public FaceType Type;

			public static FaceType GetFaceType (int spriteAngleIndex, Direction dir, float normalX) {
				int type = dir == Direction.Up ? 0 : normalX < 0f ? 4 : 8;
				type +=
					spriteAngleIndex >= 0 && spriteAngleIndex <= 3 ? 2 :
					(spriteAngleIndex >= 4 && spriteAngleIndex <= 7 ? 0 :
					(spriteAngleIndex >= 8 && spriteAngleIndex <= 11 ? 1 : 3));
				if (type == (int)FaceType.Left_0) {
					type = (int)FaceType.Front_0;
				}
				return (FaceType)type;
			}

		}


		private struct Pixel {
			public Color Color;
			public int X;
			public int Y;
		}


		private class FaceSorter : IComparer<VoxelFace> {
			public int Compare (VoxelFace x, VoxelFace y) {
				return y.Position.z.CompareTo(x.Position.z);
			}
		}




		#endregion







		#region --- VAR ---



		public static readonly float[] SPRITE_ANGLE = new float[] {
			45f, 135f, 225f, 315f,
			0f, 90f, 180f, 270f
		};
		public static readonly float[] SPRITE_ANGLE_SIMPLE = new float[] {
			45f, 135f, 225f, 315f,
			0f, 90.001f, 180f, 270.001f
		};
		private static readonly string[] SPRITE_2D_NAMES = new string[] { "F", "L", "B", "R", "U", "D" };
		private static readonly Vector3[] VOX_CENTER_OFFSET = new Vector3[] {
			Vector3.up,
			Vector3.down,
			Vector3.back ,
			Vector3.forward,
			Vector3.left,
			Vector3.right,
		};
		private const int MAX_ROOM_NUM = 80 * 80 * 80;
		private static readonly float CAMERA_ANGLE = 35f;



		#endregion



		// API
		public static Result Create8bitSprite (VoxelData voxelData, int modelIndex) {

			// Voxels
			Voxel[,,] voxels = GetVoxels(voxelData, modelIndex);

			// Colorss
			int[] widths;
			int[] heights;
			Vector2[] pivots;
			Color[][] colorss = GetColorss(
				voxels, voxelData.Palette.ToArray(),
				Vector3.one * 0.5f, 8, 3f,
				out widths, out heights, out pivots
			);

			// Result
			Result result = PackTextures(colorss, widths, heights, pivots);
			result.NameFixes = new string[result.Rects.Length];
			for (int i = 0; i < result.Rects.Length; i++) {
				result.NameFixes[i] = SPRITE_ANGLE[i].ToString("0");
			}

			return result;
		}



		public static Result Create2DSprite (VoxelData voxelData, int modelIndex) {

			// Voxels
			Voxel[,,] voxels = GetVoxels(voxelData, modelIndex);

			// Colorss
			int[] widths;
			int[] heights;
			Color[][] colorss = Get2DColorss(voxels, voxelData.Palette.ToArray(), 6, Mathf.Lerp(0f, 1f, 3f / 5f), out widths, out heights);

			// Result
			Result result = Pack2DTextures(colorss, widths, heights, Vector2.one * 0.5f);
			result.NameFixes = new string[result.Rects.Length];
			for (int i = 0; i < result.Rects.Length; i++) {
				result.NameFixes[i] = SPRITE_2D_NAMES[i];
			}
			return result;
		}









		// LGC
		private static Voxel[,,] GetVoxels (VoxelData voxelData, int modelIndex) {
			var size = voxelData.GetModelSize(modelIndex);
			int sizeX = (int)size.x;
			int sizeY = (int)size.z;
			int sizeZ = (int)size.y;
			Voxel[,,] voxels = new Voxel[sizeX, sizeY, sizeZ];
			for (int i = 0; i < sizeX; i++) {
				for (int j = 0; j < sizeY; j++) {
					for (int k = 0; k < sizeZ; k++) {
						voxels[i, j, k].Init();
						voxels[i, j, k].ColorIndex = voxelData.Voxels[modelIndex][i, k, j];
					}
				}
			}
			for (int i = 0; i < sizeX; i++) {
				for (int j = 0; j < sizeY; j++) {
					for (int k = 0; k < sizeZ; k++) {
						if (voxels[i, j, k].IsEmpty) {
							voxels[i, j, k].IsVisible = true;
							continue;
						}
						voxels[i, j, k].VisibleLeft = i > 0 ? voxels[i - 1, j, k].IsEmpty : true;
						voxels[i, j, k].VisibleRight = i < sizeX - 1 ? voxels[i + 1, j, k].IsEmpty : true;
						voxels[i, j, k].VisibleFront = j > 0 ? voxels[i, j - 1, k].IsEmpty : true;
						voxels[i, j, k].VisibleBack = j < sizeY - 1 ? voxels[i, j + 1, k].IsEmpty : true;
						voxels[i, j, k].VisibleDown = k > 0 ? voxels[i, j, k - 1].IsEmpty : true;
						voxels[i, j, k].VisibleUp = k < sizeZ - 1 ? voxels[i, j, k + 1].IsEmpty : true;
					}
				}
			}
			return voxels;
		}



		private static Color[][] GetColorss (
			Voxel[,,] voxels, Color[] palette,
			Vector3 mainPivot, int spriteNum, float lightIntensity,
			out int[] widths, out int[] heights, out Vector2[] pivots
		) {

			int voxelSizeX = voxels.GetLength(0);
			int voxelSizeY = voxels.GetLength(1);
			int voxelSizeZ = voxels.GetLength(2);
			widths = new int[spriteNum];
			heights = new int[spriteNum];
			pivots = new Vector2[spriteNum];

			if (voxelSizeX * voxelSizeY * voxelSizeZ > MAX_ROOM_NUM) {
				if (!Util.Dialog(
					"Warning",
					"Model Is Too Large !\nIt may takes very long time to create this sprite.\nAre you sure to do that?",
					"Just Go ahead!",
					"Cancel"
				)) {
					return null;
				}
			}

			Color[][] colorss = new Color[spriteNum][];
			Vector3 pivotOffset = new Vector3(
				voxelSizeX * mainPivot.x,
				voxelSizeZ * mainPivot.y,
				voxelSizeY * mainPivot.z
			);


			for (int index = 0; index < spriteNum; index++) {

				float angleY = SPRITE_ANGLE_SIMPLE[index];
				Quaternion cameraRot = Quaternion.Inverse(Quaternion.Euler(CAMERA_ANGLE, angleY, 0f));
				Vector3 minPos;
				Vector3 maxPos;


				// Get faces
				List<VoxelFace> faces = GetFaces(
					voxels, palette,
					new Vector3(voxelSizeX, voxelSizeY, voxelSizeZ), mainPivot, cameraRot, pivotOffset,
					index, lightIntensity,
					out minPos, out maxPos
				);


				// Get Pivot01
				Vector3 pivot = new Vector3(
					Mathf.LerpUnclamped(0f, voxelSizeX, mainPivot.x),
					Mathf.LerpUnclamped(0f, voxelSizeZ, mainPivot.y),
					Mathf.LerpUnclamped(0f, voxelSizeY, mainPivot.z)
				);

				pivot = cameraRot * new Vector3(
					pivot.x - voxelSizeX * mainPivot.x,
					pivot.y - voxelSizeZ * mainPivot.y,
					pivot.z - voxelSizeY * mainPivot.z
				) + pivotOffset;

				pivot = new Vector3(
					(pivot.x - minPos.x) / (maxPos.x - minPos.x),
					(pivot.y - minPos.y) / (maxPos.y - minPos.y),
					(pivot.z - minPos.z) / (maxPos.z - minPos.z)
				);


				// Get Pixels

				int minPixelX;
				int minPixelY;
				int maxPixelX;
				int maxPixelY;

				// Get Pixels
				List<Pixel> pixels = GetPixels(
					faces,
					out minPixelX, out minPixelY, out maxPixelX, out maxPixelY
				);

				// W and H
				int width = maxPixelX - minPixelX + 1 + 2;
				int height = maxPixelY - minPixelY + 1 + 2;

				// Get Colorss
				colorss[index] = new Color[width * height];
				int len = pixels.Count;
				for (int i = 0; i < len; i++) {
					int id = (pixels[i].Y - minPixelY + 1) * width + (pixels[i].X - minPixelX + 1);
					colorss[index][id] = pixels[i].Color;
				}

				// Cheat
				{
					List<int> cheatPixels = new List<int>();
					List<Color> cheatColors = new List<Color>();
					for (int x = 0; x < width; x++) {
						for (int y = 0; y < height; y++) {
							Color c = CheckPixelsAround_Cheat(colorss[index], x, y, width, height);
							if (c != Color.clear) {
								cheatPixels.Add(y * width + x);
								cheatColors.Add(c);
							}
						}
					}
					int cheatCount = cheatPixels.Count;
					for (int i = 0; i < cheatCount; i++) {
						colorss[index][cheatPixels[i]] = cheatColors[i];
					}
				}

				// Final
				widths[index] = width;
				heights[index] = height;
				pivots[index] = pivot;

			}
			return colorss;
		}



		private static Color[][] Get2DColorss (Voxel[,,] voxels, Color[] palette, int num, float light01, out int[] widths, out int[] heights) {
			widths = new int[num];
			heights = new int[num];
			Color[][] colorss = new Color[num][];
			int[] FACE_DIR_ID = new int[6] { 2, 4, 3, 5, 0, 1 };
			for (int i = 0; i < num; i++) {
				int width = voxels.GetLength(i == 1 || i == 3 ? 2 : 0);
				int height = voxels.GetLength(i == 0 || i == 2 ? 2 : 1);
				int depth = voxels.GetLength(i == 1 || i == 3 ? 0 : i == 0 || i == 2 ? 1 : 2);
				int faceDirIndex = FACE_DIR_ID[i];
				colorss[i] = new Color[(width + 1) * (height + 1)];
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						Color c = Color.clear;
						for (int z = 0; z < depth; z++) {
							Voxel v =
								i == 0 ? voxels[x, z, y] :
								i == 1 ? voxels[z, height - y - 1, x] :
								i == 2 ? voxels[width - x - 1, depth - z - 1, y] :
								i == 3 ? voxels[depth - z - 1, y, x] :
								i == 4 ? voxels[x, y, depth - z - 1] :
										 voxels[x, height - y - 1, z];
							if (v.ColorIndex > 0 && v.Visible[faceDirIndex]) {
								c = Color.Lerp(
									palette[v.ColorIndex - 1],
									Color.black,
									light01 * z / depth
								);
								break;
							}
						}
						colorss[i][i == 1 || i == 3 ? x * (height + 1) + y : y * (width + 1) + x] = c;
					}
				}
				widths[i] = (i == 1 || i == 3 ? height : width) + 1;
				heights[i] = (i == 1 || i == 3 ? width : height) + 1;
			}
			return colorss;
		}



		private static List<VoxelFace> GetFaces (
			Voxel[,,] voxels, Color[] palette,
			Vector3 voxelSize, Vector3 mainPivot, Quaternion cameraRot, Vector3 pivotOffset,
			int cameraAngleIndex, float lightIntensity,
			out Vector3 minPos, out Vector3 maxPos
		) {
			lightIntensity = Mathf.Lerp(0f, 0.7f, lightIntensity * 0.2f);
			minPos = Vector3.one * float.MaxValue;
			maxPos = Vector3.one * float.MinValue;
			List<VoxelFace> faces = new List<VoxelFace>();
			for (int x = 0; x < voxelSize.x; x++) {
				for (int y = 0; y < voxelSize.y; y++) {
					for (int z = 0; z < voxelSize.z; z++) {
						Voxel vox = voxels[x, y, z];
						Color color = palette[vox.ColorIndex <= 0 ? 0 : vox.ColorIndex - 1];
						for (int i = 0; i < 6; i++) {
							if (!vox.IsEmpty && vox.Visible[i]) {
								Vector3 pos = cameraRot * (new Vector3(
									x - voxelSize.x * mainPivot.x,
									z - voxelSize.z * mainPivot.y,
									y - voxelSize.y * mainPivot.z
								) + VOX_CENTER_OFFSET[i] * 0.5f) + pivotOffset;
								minPos = Vector3.Min(minPos, pos);
								maxPos = Vector3.Max(maxPos, pos);
								Vector3 worldNormal = cameraRot * VOX_CENTER_OFFSET[i];
								// Normal Check
								if (Vector3.Angle(worldNormal, Vector3.back) >= 90f) {
									continue;
								}
								VoxelFace face = new VoxelFace() {
									Position = pos,
									Normal = worldNormal,
									Type = VoxelFace.GetFaceType(cameraAngleIndex, (Direction)i, worldNormal.x),
								};
								face.Color = Color.Lerp(
									color,
									(int)face.Type > 3 ? Color.black : color,
									(int)face.Type > 3 ? (int)face.Type > 7 ? lightIntensity * 0.5f : lightIntensity : 1f
								);
								faces.Add(face);
							}
						}
					}
				}
			}
			faces.Sort(new FaceSorter());
			return faces;
		}




		private static Color CheckPixelsAround_Cheat (Color[] colors, int x, int y, int w, int h) {
			// Self
			if (colors[y * w + x] != Color.clear) {
				return Color.clear;
			}

			Color c, color = Color.clear;

			// U
			if (y < h - 1) {
				c = colors[(y + 1) * w + x];
				if (c == Color.clear) {
					return Color.clear;
				} else {
					color = c;
				}
			}

			// D
			if (y > 0) {
				c = colors[(y - 1) * w + x];
				if (c == Color.clear) {
					return Color.clear;
				} else {
					color = c;
				}
			}

			// L
			if (x < w - 1) {
				c = colors[y * w + x + 1];
				if (c == Color.clear) {
					return Color.clear;
				} else {
					color = c;
				}
			}

			// R
			if (x > 0) {
				c = colors[y * w + x - 1];
				if (c == Color.clear) {
					return Color.clear;
				} else {
					color = c;
				}
			}

			return color;
		}



		#region --- Colorss ---



		private static List<Pixel> GetPixels (
			List<VoxelFace> faces,
			out int minPixelX, out int minPixelY, out int maxPixelX, out int maxPixelY
		) {
			minPixelX = int.MaxValue;
			minPixelY = int.MaxValue;
			maxPixelX = 0;
			maxPixelY = 0;
			List<Pixel> pixels = new List<Pixel>();
			int count = faces.Count;
			for (int index = 0; index < count; index++) {
				VoxelFace face = faces[index];
				float pixelSize = 1f;
				Vector2 pos = new Vector2(
					(face.Position.x * pixelSize),
					(face.Position.y * pixelSize)
				);
				pos.x = Mathf.Floor((pos.x / pixelSize) * pixelSize);
				pos.y = Mathf.Floor((pos.y / pixelSize) * pixelSize);

				minPixelX = Mathf.Min(minPixelX, (int)pos.x);
				minPixelY = Mathf.Min(minPixelY, (int)pos.y);
				maxPixelX = Mathf.Max((int)pos.x, maxPixelX);
				maxPixelY = Mathf.Max((int)pos.y, maxPixelY);
				pixels.Add(new Pixel() { Color = face.Color, X = (int)pos.x, Y = (int)pos.y });
			}

			return pixels;
		}






		#endregion



		private static Result PackTextures (Color[][] colorss, int[] widths, int[] heights, Vector2[] pivots) {

			int tCount = colorss.Length;
			int gapSize = 1;
			Result resultInfo = new Result() {
				Pivots = pivots,
			};

			// Single Size
			int singleWidth = 0;
			int singleHeight = 0;
			for (int i = 0; i < tCount; i++) {
				singleWidth = Mathf.Max(singleWidth, widths[i]);
				singleHeight = Mathf.Max(singleHeight, heights[i]);
			}

			// Size All
			int aimCountX = tCount > 4 ? 4 : tCount;
			int aimCountY = ((tCount - 1) / 4) + 1;
			int aimWidth = aimCountX * singleWidth + gapSize * (aimCountX + 1);
			int aimHeight = aimCountY * singleHeight + gapSize * (aimCountY + 1);

			resultInfo.Width = aimWidth;
			resultInfo.Height = aimHeight;
			resultInfo.Texture = new Texture2D(aimWidth, aimHeight, TextureFormat.ARGB32, false);
			resultInfo.Texture.SetPixels(new Color[aimWidth * aimHeight]);

			Rect[] spriteRects = new Rect[tCount];
			for (int i = 0; i < tCount; i++) {
				int width = widths[i];
				int height = heights[i];
				int globalOffsetX = (i % 4) * singleWidth + ((i % 4) + 1) * gapSize;
				int globalOffsetY = (i / 4) * singleHeight + ((i / 4) + 1) * gapSize;
				int offsetX = globalOffsetX + (singleWidth - width) / 2;
				int offsetY = globalOffsetY + (singleHeight - height) / 2;
				// Rect
				spriteRects[i] = new Rect(globalOffsetX, globalOffsetY, singleWidth, singleHeight);
				// Pivot
				resultInfo.Pivots[i] = new Vector2(
					(resultInfo.Pivots[i].x * (float)width + (float)(singleWidth - width) / 2f) / (float)singleWidth,
					(resultInfo.Pivots[i].y * (float)height + (float)(singleHeight - height) / 2f) / (float)singleHeight
				);
				resultInfo.Texture.SetPixels(offsetX, offsetY, width, height, colorss[i]);
			}
			resultInfo.Texture.Apply();
			resultInfo.Rects = spriteRects;

			return resultInfo;
		}



		private static Result Pack2DTextures (Color[][] colorss, int[] widths, int[] heights, Vector2 pivot) {

			int tCount = colorss.Length;
			Result resultInfo = new Result() {
				Pivots = new Vector2[6] { pivot, pivot, pivot, pivot, Vector2.one * 0.5f, Vector2.one * 0.5f },
			};

			Texture2D[] textures = new Texture2D[colorss.Length];
			for (int i = 0; i < textures.Length; i++) {
				textures[i] = new Texture2D(widths[i], heights[i], TextureFormat.ARGB32, false);
				textures[i].SetPixels(colorss[i]);
				textures[i].Apply();
			}

			var packingList = new List<PackingData>();
			for (int i = 0; i < textures.Length; i++) {
				var t = textures[i];
				packingList.Add(new PackingData(t.width, t.height, t.GetPixels(), false));
			}

			Rect[] rects = RectPacking.PackTextures(out resultInfo.Texture, packingList, false, true);
			resultInfo.Width = resultInfo.Texture.width;
			resultInfo.Height = resultInfo.Texture.height;
			for (int i = 0; i < rects.Length; i++) {
				rects[i].x *= resultInfo.Width;
				rects[i].y *= resultInfo.Height;
				rects[i].width *= resultInfo.Width;
				rects[i].height *= resultInfo.Height;
				rects[i].width = rects[i].width - 1;
				rects[i].height = rects[i].height - 1;
			}
			resultInfo.Rects = rects;
			return resultInfo;
		}





	}

}