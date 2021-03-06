using NewBuildSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSaving : MonoBehaviour
{
	[Serializable]
	public class Quicksaves
	{
		public List<GameSaving.GameSave> quicksaves;

		public int QuicksavesCount
		{
			get
			{
				return this.quicksaves.Count;
			}
		}

		public Quicksaves()
		{
			this.quicksaves = new List<GameSaving.GameSave>();
		}

		public static void AddQuicksave(GameSaving.GameSave newQuicksave)
		{
			GameSaving.Quicksaves quicksaves = GameSaving.Quicksaves.LoadQuicksaves();
			quicksaves.quicksaves.Add(newQuicksave);
			GameSaving.Quicksaves.SaveQuicksaves(quicksaves);
		}

		public static void RemoveQuicksaveAt(int index)
		{
			GameSaving.Quicksaves quicksaves = GameSaving.Quicksaves.LoadQuicksaves();
			if (index > -1 && index < quicksaves.QuicksavesCount)
			{
				quicksaves.quicksaves.RemoveAt(index);
				GameSaving.Quicksaves.SaveQuicksaves(quicksaves);
			}
		}

		private static void SaveQuicksaves(GameSaving.Quicksaves newQuicksaves)
		{
			Ref.SaveJsonString(JsonUtility.ToJson(newQuicksaves), Saving.SaveKey.GameQuicksaves);
		}

		public static GameSaving.Quicksaves LoadQuicksaves()
		{
			string text = Ref.LoadJsonString(Saving.SaveKey.GameQuicksaves);
			return (!(text != string.Empty)) ? new GameSaving.Quicksaves() : JsonUtility.FromJson<GameSaving.Quicksaves>(text);
		}
	}

	[Serializable]
	public class GameSave
	{
		public string saveName;

		public double globalTime;

		public int timeWarpPhase;

		public double startedTimewapTime;

		public float camDistance;

		public bool mapView;

		public Double3 mapPosition;

		public string mapFollowingAdress;

		public int mainVesselId;

		public int selectedVesselId;

		public List<GameSaving.VesselSave> vessels;

		public Double3 positionOffset;

		public Double3 velocityOffset;

		public GameSave(string saveName, int mainVesselId, int selectedVesselId, List<GameSaving.VesselSave> vessels, Double3 positionOffset, Double3 velocityOffset, double globalTime, int timeWarpPhase, double startedTimewapTime, float camDistance, bool mapView, Double3 mapPosition, string mapFollowingAdress)
		{
			this.saveName = saveName;
			this.globalTime = globalTime;
			this.timeWarpPhase = timeWarpPhase;
			this.startedTimewapTime = startedTimewapTime;
			this.camDistance = camDistance;
			this.mapView = mapView;
			this.mapPosition = mapPosition;
			this.mapFollowingAdress = mapFollowingAdress;
			this.mainVesselId = mainVesselId;
			this.selectedVesselId = selectedVesselId;
			this.vessels = vessels;
			this.positionOffset = positionOffset;
			this.velocityOffset = velocityOffset;
		}

		public static GameSaving.GameSave LoadPersistant()
		{
			string text = Ref.LoadJsonString(Saving.SaveKey.PersistantGameSave);
			return (!(text != string.Empty)) ? null : JsonUtility.FromJson<GameSaving.GameSave>(text);
		}

		public static void DeletePersistant()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void UpdatePersistantSave()
		{
			GameSaving.GameSave gameSaveData = GameSaving.GetGameSaveData("Persistant Game Save");
			Ref.SaveJsonString(JsonUtility.ToJson(gameSaveData), Saving.SaveKey.PersistantGameSave);
		}
	}

	[Serializable]
	public class VesselSave
	{
		public string adress;

		public Double3 globalPosition;

		public Double3 globalVelocity;

		public float rotation;

		public float angularVelocity;

		public Vessel.State state;

		public Vessel.Throttle throttle;

		public Part.Save[] parts;

		public Part.Joint.Save[] joints;

		public List<string> vesselArchivments;

		public VesselSave(Vessel.State state, string adress, Double3 globalPosition, Double3 globalVelocity, float rotation, float angularVelocity, Vessel.Throttle throttle, Part.Save[] parts, Part.Joint.Save[] joints, List<string> vesselArchivments)
		{
			this.globalPosition = globalPosition;
			this.globalVelocity = globalVelocity;
			this.rotation = rotation;
			this.angularVelocity = angularVelocity;
			this.adress = adress;
			this.state = state;
			this.throttle = throttle;
			this.parts = parts;
			this.joints = joints;
			this.vesselArchivments = vesselArchivments;
		}
	}

	public static GameSaving.GameSave GetGameSaveData(string saveName)
	{
		return new GameSaving.GameSave(saveName, GameSaving.GetVesselListIndex(Ref.mainVessel), GameSaving.GetVesselListIndex(Ref.selectedVessel), GameSaving.GetVesselSaveData(Ref.controller.vessels), Ref.positionOffset, Ref.velocityOffset, Ref.controller.globalTime, Ref.controller.timewarpPhase, Ref.controller.startedTimewarpTime, Ref.controller.cameraDistanceGame, Ref.mapView, Ref.map.mapPosition, Ref.map.following.bodyName);
	}

	private static List<GameSaving.VesselSave> GetVesselSaveData(List<Vessel> vesselsToSave)
	{
		List<GameSaving.VesselSave> list = new List<GameSaving.VesselSave>();
		for (int i = 0; i < vesselsToSave.Count; i++)
		{
			list.Add(new GameSaving.VesselSave(vesselsToSave[i].state, vesselsToSave[i].GetVesselPlanet.bodyName, vesselsToSave[i].GetGlobalPosition, vesselsToSave[i].GetGlobalVelocity, vesselsToSave[i].partsManager.rb2d.rotation, vesselsToSave[i].partsManager.rb2d.angularVelocity, vesselsToSave[i].throttle, GameSaving.GetPartsSaveData(vesselsToSave[i].partsManager.parts), GameSaving.GetPartsJointsSave(vesselsToSave[i].partsManager.parts), vesselsToSave[i].vesselAchievements));
		}
		return list;
	}

	private static Part.Save[] GetPartsSaveData(List<Part> partsToSave)
	{
		int count = partsToSave.Count;
		Part.Save[] array = new Part.Save[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new Part.Save(partsToSave[i], partsToSave[i].orientation, partsToSave);
		}
		return array;
	}

	private static Part.Joint.Save[] GetPartsJointsSave(List<Part> parts)
	{
		List<Part.Joint.Save> list = new List<Part.Joint.Save>();
		foreach (Part current in parts)
		{
			foreach (Part.Joint current2 in current.joints)
			{
				if (current2.fromPart == current)
				{
					list.Add(new Part.Joint.Save(Part.GetPartListIndex(current2.fromPart, parts), Part.GetPartListIndex(current2.toPart, parts), current2.anchor, current2.fromSurfaceIndex, current2.toSurfaceIndex, current2.fuelFlow));
				}
			}
		}
		return list.ToArray();
	}

	public static void LoadGame(GameSaving.GameSave loadedData)
	{
		GameSaving.ClearScene();
		Ref.planetManager.SwitchLocation(Ref.GetPlanetByName(loadedData.vessels[loadedData.mainVesselId].adress), loadedData.vessels[loadedData.mainVesselId].globalPosition, false, true);
		Ref.velocityOffset = loadedData.velocityOffset;
		Ref.controller.globalTime = loadedData.globalTime;
		Ref.controller.timewarpPhase = loadedData.timeWarpPhase;
		Ref.controller.startedTimewarpTime = loadedData.startedTimewapTime;
		Ref.timeWarping = (Ref.controller.timewarpPhase != 0);
		Ref.controller.SetCameraDistance(loadedData.camDistance);
		foreach (GameSaving.VesselSave current in loadedData.vessels)
		{
			GameSaving.LoadVessel(current);
		}
		Ref.planetManager.FullyLoadTerrain(Ref.GetPlanetByName(loadedData.vessels[loadedData.mainVesselId].adress));
		Ref.mainVessel = Ref.controller.vessels[loadedData.mainVesselId];
		Ref.selectedVessel = ((loadedData.selectedVesselId == -1) ? null : Ref.controller.vessels[loadedData.selectedVesselId]);
		Ref.map.UpdateVesselsMapIcons();
		Ref.map.following = Ref.GetPlanetByName(loadedData.mapFollowingAdress);
		Ref.map.UpdateMapPosition(loadedData.mapPosition);
		Ref.map.UpdateMapZoom(-loadedData.mapPosition.z);
		Ref.map.ToggleMap();
		if (Ref.mapView != loadedData.mapView)
		{
			Ref.map.ToggleMap();
		}
		Ref.mainVessel.SetThrottle(Ref.mainVessel.throttle);
		Ref.map.UpdateVesselOrbitLines(new List<Orbit>(), true);
		Ref.controller.RepositionFuelIcons();
		Ref.controller.warpedTimeCounterUI.text = string.Empty;
		if (Ref.map.transferWindow.target != null)
		{
			Ref.map.SelectCelestilaBodyTarget(Ref.map.transferWindow.target);
		}
		Ref.planetManager.UpdateAtmosphereFade();
		Ref.mainVesselHeight = loadedData.vessels[loadedData.mainVesselId].globalPosition.magnitude2d - Ref.GetPlanetByName(loadedData.vessels[loadedData.mainVesselId].adress).radius;
		Ref.mainVesselAngleToPlanet = (float)Math.Atan2(loadedData.vessels[loadedData.mainVesselId].globalPosition.y, loadedData.vessels[loadedData.mainVesselId].globalPosition.x) * 57.29578f;
		if (Ref.mainVesselHeight < Ref.GetPlanetByName(loadedData.vessels[loadedData.mainVesselId].adress).cameraSwitchHeightM)
		{
			Ref.controller.camTargetAngle = Ref.mainVesselAngleToPlanet - 90f;
		}
		else
		{
			Ref.controller.camTargetAngle = 0f;
		}
		Ref.cam.transform.eulerAngles = new Vector3(0f, 0f, Ref.controller.camTargetAngle);
		Ref.controller.camAngularVelocity = 0f;
	}

	public static void LoadForLaunch(GameSaving.GameSave loadedData, Double3 launchPadPosition)
	{
		for (int i = 0; i < loadedData.vessels.Count; i++)
		{
			if (loadedData.vessels[i].adress == Ref.controller.startAdress && Math.Abs(loadedData.vessels[i].globalPosition.x - launchPadPosition.x) < 10.0 && Math.Abs(loadedData.vessels[i].globalPosition.y - launchPadPosition.y) < 40.0)
			{
				loadedData.vessels.RemoveAt(i);
				i--;
			}
			else
			{
				Vessel.State state = loadedData.vessels[i].state;
				if (state != Vessel.State.RealTime)
				{
					if (state != Vessel.State.OnRails)
					{
						if (state == Vessel.State.Stationary)
						{
							loadedData.vessels[i].state = Vessel.State.StationaryUnloaded;
						}
					}
					else
					{
						loadedData.vessels[i].state = Vessel.State.OnRailsUnloaded;
					}
				}
				else if ((loadedData.vessels[i].globalPosition - launchPadPosition).magnitude2d > 1000.0 || loadedData.vessels[i].adress != Ref.controller.startAdress)
				{
					loadedData.vessels[i].state = ((Math.Abs(loadedData.vessels[i].globalVelocity.x) <= 1.0 && Math.Abs(loadedData.vessels[i].globalVelocity.y) <= 1.0) ? Vessel.State.StationaryUnloaded : Vessel.State.OnRailsUnloaded);
				}
			}
		}
		Ref.planetManager.SwitchLocation(Ref.GetPlanetByName(Ref.controller.startAdress), launchPadPosition, false, true);
		Ref.planetManager.UpdatePositionOffset(new Double3(0.0, 315000.0));
		Ref.velocityOffset = Double3.zero;
		Ref.controller.globalTime = loadedData.globalTime;
		Ref.controller.timewarpPhase = 0;
		Ref.timeWarping = false;
		foreach (GameSaving.VesselSave current in loadedData.vessels)
		{
			GameSaving.LoadVessel(current);
		}
		Ref.map.following = Ref.GetPlanetByName(Ref.controller.startAdress);
		Ref.map.UpdateMapPosition(new Double3(0.0, launchPadPosition.y / 10000.0));
		Ref.map.UpdateMapZoom(launchPadPosition.y / 10000.0 / 20.0);
		Ref.planetManager.UpdateAtmosphereFade();
		Ref.controller.warpedTimeCounterUI.text = string.Empty;
	}

	private static Vessel LoadVessel(GameSaving.VesselSave vesselToLoad)
	{
		Part[] array = GameSaving.LoadParts(vesselToLoad);
		Vessel vessel = Vessel.CreateVessel(array[0].GetComponent<Part>(), Vector2.zero, vesselToLoad.angularVelocity, vesselToLoad.throttle, vesselToLoad.vesselArchivments, Ref.GetPlanetByName(vesselToLoad.adress).mapRefs.holder);
		vessel.partsManager.UpdateCenterOfMass();
		vessel.partsManager.ReCenter();
		vessel.transform.position = (vesselToLoad.globalPosition - Ref.positionOffset).toVector2;
		vessel.transform.eulerAngles = new Vector3(0f, 0f, vesselToLoad.rotation);
		if (vesselToLoad.state == Vessel.State.RealTime)
		{
			vessel.partsManager.rb2d.velocity = (vesselToLoad.globalVelocity - Ref.velocityOffset).toVector2;
		}
		else if (vesselToLoad.state == Vessel.State.OnRails || vesselToLoad.state == Vessel.State.OnRailsUnloaded)
		{
			vessel.orbits = Orbit.CalculateOrbits(vesselToLoad.globalPosition, vesselToLoad.globalVelocity, Ref.GetPlanetByName(vesselToLoad.adress));
			vessel.partsManager.rb2d.bodyType = RigidbodyType2D.Static;
			vessel.state = Vessel.State.OnRails;
			if (vessel.state == Vessel.State.OnRailsUnloaded)
			{
				vessel.SetVesselState(Vessel.ToState.ToUnloaded);
			}
			if (double.IsNaN(vessel.orbits[0].meanMotion))
			{
				MonoBehaviour.print("Cannot orbit NaN, went stationary instead");
				vessel.orbits.Clear();
				vessel.stationaryData.posToPlane = vesselToLoad.globalPosition;
				vessel.stationaryData.planet = Ref.GetPlanetByName(vesselToLoad.adress);
				vessel.state = Vessel.State.Stationary;
				vessel.SetVesselState(Vessel.ToState.ToUnloaded);
				vessel.mapIcon.localPosition = (vessel.stationaryData.posToPlane / 10000.0).toVector3;
			}
		}
		else
		{
			vessel.stationaryData.posToPlane = vesselToLoad.globalPosition;
			vessel.stationaryData.planet = Ref.GetPlanetByName(vesselToLoad.adress);
			vessel.state = Vessel.State.Stationary;
			if (vesselToLoad.state == Vessel.State.StationaryUnloaded)
			{
				vessel.SetVesselState(Vessel.ToState.ToUnloaded);
			}
			vessel.mapIcon.localPosition = (vessel.stationaryData.posToPlane / 10000.0).toVector3;
		}
		GameSaving.LoadPartsData(vesselToLoad, array);
		vessel.mapIcon.rotation = vessel.partsManager.parts[0].transform.rotation;
		return vessel;
	}

	private static Part[] LoadParts(GameSaving.VesselSave vesselToLoad)
	{
		int num = vesselToLoad.parts.Length;
		PartData[] array = new PartData[num];
		Part[] array2 = new Part[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Ref.controller.partDatabase.GetPartByName(vesselToLoad.parts[i].partName);
			array2[i] = UnityEngine.Object.Instantiate<Transform>(array[i].prefab, Vector3.zero, Quaternion.identity).GetComponent<Part>();
			array2[i].orientation = vesselToLoad.parts[i].orientation;
		}
		for (int j = 0; j < vesselToLoad.joints.Length; j++)
		{
			Part.Joint.Save save = vesselToLoad.joints[j];
			if (save.fromPartId != -1 && save.toPartId != -1)
			{
				new Part.Joint(save.anchor, array2[save.fromPartId], array2[save.toPartId], save.fromSurfaceIndex, save.toSurfaceIndex, save.fuelFlow);
			}
		}
		for (int k = 0; k < num; k++)
		{
			array2[k].UpdateConnected();
		}
		return array2;
	}

	private static void LoadPartsData(GameSaving.VesselSave vesselToLoad, Part[] loadedParts)
	{
		for (int i = 0; i < vesselToLoad.parts.Length; i++)
		{
			Part part = loadedParts[i];
			Part.Save save = vesselToLoad.parts[i];
			Module[] components = part.GetComponents<Module>();
			Array.Reverse(components);
			Array.Reverse(save.moduleSaves);
			for (int j = 0; j < part.modules.Length; j++)
			{
				components[j].Load(save.moduleSaves[j]);
			}
		}
	}

	private static void ClearScene()
	{
		while (Ref.controller.vessels.Count > 0)
		{
			Ref.controller.vessels[0].DestroyVessel();
		}
		Ref.controller.vessels.Clear();
	}

	private static int GetVesselListIndex(Vessel vessel)
	{
		for (int i = 0; i < Ref.controller.vessels.Count; i++)
		{
			if (Ref.controller.vessels[i] == vessel)
			{
				return i;
			}
		}
		return -1;
	}
}
