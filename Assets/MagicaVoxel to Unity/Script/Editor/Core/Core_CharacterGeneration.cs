namespace VoxeltoUnity {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class Core_CharacterGeneration {




		#region --- SUB ---



		[System.Serializable]
		public class CharacterObject {
			public string Name = "";
			public int SizeX = 14;
			public int SizeY = 42;
			public int SizeZ = 14;


			public void FixValues () {
				SizeX = Mathf.Clamp(SizeX, 8, 126);
				SizeY = Mathf.Clamp(SizeY, 8, 126);
				SizeZ = Mathf.Clamp(SizeZ, 8, 126);

			}

		}



		[System.Serializable]
		public class Preset {


			// Body
			public CharacterObject Hip = new CharacterObject() { Name = "Hip", };
			public CharacterObject Spine = new CharacterObject() { Name = "Spine", };
			public CharacterObject Chest = new CharacterObject() { Name = "Chest", };
			public CharacterObject Neck = new CharacterObject() { Name = "Neck", };
			public CharacterObject Head = new CharacterObject() { Name = "Head", };
			// Arm
			public CharacterObject ArmUL = new CharacterObject() { Name = "ArmUL", };
			public CharacterObject ArmUR = new CharacterObject() { Name = "ArmUR", };
			public CharacterObject ArmDL = new CharacterObject() { Name = "ArmDL", };
			public CharacterObject ArmDR = new CharacterObject() { Name = "ArmDR", };
			public CharacterObject HandL = new CharacterObject() { Name = "HandL", };
			public CharacterObject HandR = new CharacterObject() { Name = "HandR", };
			// Leg
			public CharacterObject LegUL = new CharacterObject() { Name = "LegUL", };
			public CharacterObject LegUR = new CharacterObject() { Name = "LegUR", };
			public CharacterObject LegDL = new CharacterObject() { Name = "LegDL", };
			public CharacterObject LegDR = new CharacterObject() { Name = "LegDR", };
			public CharacterObject FootL = new CharacterObject() { Name = "FootL", };
			public CharacterObject FootR = new CharacterObject() { Name = "FootR", };





			public void LoadFromJson (string json) {
				try {
					if (string.IsNullOrEmpty(json)) { return; }
					JsonUtility.FromJsonOverwrite(json, this);
				} catch { }
			}



			public string ToJson () {
				return JsonUtility.ToJson(this, true);
			}





			public void FixGenerationValues () {

				Hip.FixValues();
				Spine.FixValues();
				Chest.FixValues();
				Neck.FixValues();
				Head.FixValues();

				ArmUL.FixValues();
				ArmUR.FixValues();
				ArmDL.FixValues();
				ArmDR.FixValues();
				HandL.FixValues();
				HandR.FixValues();

				LegUL.FixValues();
				LegUR.FixValues();
				LegDL.FixValues();
				LegDR.FixValues();
				FootL.FixValues();
				FootR.FixValues();

			}



		}



		#endregion




		public static VoxelData Generate (Preset preset, System.Action<float, float> onProgress = null) {
			try {
				var data = VoxelData.CreateNewData();
				if (preset == null) { return data; }








				if (onProgress != null) {
					onProgress(1f, 2f);
				}
				return data;
			} catch (System.Exception ex) {
				if (onProgress != null) {
					onProgress(1f, 2f);
				}
				throw ex;
			}
		}



	}
}