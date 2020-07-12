namespace TurnTheGameOn.ObjectDistanceToggle
{
    using UnityEngine;
    using UnityEngine.Jobs;
    using Unity.Collections;
    using Unity.Mathematics;
    using Unity.Jobs;
    using System.Collections.Generic;

    public class ObjectDistanceToggleManager : MonoBehaviour
    {
        public static ObjectDistanceToggleManager Instance;
        public bool debugProcessTime;
        public Transform originTransform;
        private float3 playerPosition;
        private int indexes;
        private List<LODLevel> currentLODList = new List<LODLevel>();
        private List<ObjectDistanceToggle> ObjectDistanceToggleList = new List<ObjectDistanceToggle>();
        private List<GameObject> gameObjectList = new List<GameObject>();
        private TransformAccessArray transformAccessArray;
        private NativeArray<float> lod0DistanceArray;
        private NativeArray<LODLevel> LODLevelArray;
        private NativeArray<float> distanceToPlayerArray;
        private LODObjectJob m_LODJob;
        private JobHandle jobHandle;
        private bool isActive;
        private bool isQuitting;
        private float startTime;

        void Awake()
        {
            Instance = this;
            if (originTransform == null)
            {
                Debug.LogWarning("Origin Transform is not assigned, disabling ObjectDistanceToggleManager");
                enabled = false;
            }
        }

        void Update()
        {
            if (indexes > 0)
            {
                if (debugProcessTime)
                {
                    startTime = Time.realtimeSinceStartup;
                }
                playerPosition = originTransform.position;
                m_LODJob = new LODObjectJob
                {
                    playerPosition = playerPosition,
                    distanceToPlayerArray = distanceToPlayerArray,
                    lod0DistanceArray = lod0DistanceArray,
                    LODLevelArray = LODLevelArray,
                };
                jobHandle = m_LODJob.Schedule(transformAccessArray);
                jobHandle.Complete();
                for (int i = 0; i < indexes; i++)
                {
                    if (LODLevelArray[i] == LODLevel.CULLED)
                    {
                        SetLODCULLED(i);
                    }
                    else if (LODLevelArray[i] == LODLevel.LOD0)
                    {
                        SetLOD0(i);
                    }
                }
                if (debugProcessTime)
                {
                    Debug.Log((("StreetLightManager Update " + (Time.realtimeSinceStartup - startTime) * 1000f)) + "ms");
                }
            }
        }

        void SetLOD0(int index)
        {
            if (currentLODList[index] != LODLevel.LOD0)
            {
                currentLODList[index] = LODLevel.LOD0;
                gameObjectList[index].SetActive(true);
            }
        }

        void SetLODCULLED(int index)
        {
            if (currentLODList[index] != LODLevel.CULLED)
            {
                currentLODList[index] = LODLevel.CULLED;
                gameObjectList[index].SetActive(false);
            }
        }

        public int RegisterObject(ObjectDistanceToggle _newEntry)
        {
            isActive = true;
            int index = currentLODList.Count;
            ObjectDistanceToggleList.Add(_newEntry);
            gameObjectList.Add(_newEntry.gameObject);
            currentLODList.Add(LODLevel.NOTSET);
            /// setup temp array
            TransformAccessArray tempArray = new TransformAccessArray(indexes);
            NativeArray<float> templod0DistanceArray = new NativeArray<float>(indexes + 1, Allocator.Persistent);
            for (int i = 0; i < indexes; i++)
            {
                tempArray.Add(transformAccessArray[i]);
                templod0DistanceArray[i] = lod0DistanceArray[i];
            }
            tempArray.Add(_newEntry.transform);
            templod0DistanceArray[indexes] = _newEntry.lod0;
            indexes = index + 1;
            /// copy temp aray into new array
            if (indexes > 1)
            {
                lod0DistanceArray.Dispose();
                distanceToPlayerArray.Dispose();
                transformAccessArray.Dispose();
                LODLevelArray.Dispose();
            }
            transformAccessArray = new TransformAccessArray(indexes);
            lod0DistanceArray = new NativeArray<float>(indexes, Allocator.Persistent);
            distanceToPlayerArray = new NativeArray<float>(indexes, Allocator.Persistent);
            LODLevelArray = new NativeArray<LODLevel>(indexes, Allocator.Persistent);
            for (int i = 0; i < indexes; i++)
            {
                transformAccessArray.Add(tempArray[i]);
                lod0DistanceArray[i] = templod0DistanceArray[i];
            }
            /// dispose temp array
            tempArray.Dispose();
            templod0DistanceArray.Dispose();
            return index;
        }

        public void RemoveObject(int index)
        {
            if (isQuitting == false)
            {
                /// Convert native arrays to lists
                List<Transform> transformList = new List<Transform>();
                List<float> distanceToPlayerList = new List<float>();
                List<float> lod0DistanceList = new List<float>();
                List<LODLevel> lodLevelList = new List<LODLevel>();
                for (int i = 0; i < indexes; i++)
                {
                    transformList.Add(transformAccessArray[i]);
                    distanceToPlayerList.Add(distanceToPlayerArray[i]);
                    lod0DistanceList.Add(lod0DistanceArray[i]);
                    lodLevelList.Add(LODLevelArray[i]);
                }
                /// Remove index from all lists
                currentLODList.RemoveAt(index);
                ObjectDistanceToggleList.RemoveAt(index);
                gameObjectList.RemoveAt(index);
                transformList.RemoveAt(index);
                distanceToPlayerList.RemoveAt(index);
                lod0DistanceList.RemoveAt(index);
                lodLevelList.RemoveAt(index);
                /// Dispose old arrays
                transformAccessArray.Dispose();
                lod0DistanceArray.Dispose();
                distanceToPlayerArray.Dispose();
                LODLevelArray.Dispose();
                /// Create new arrays
                transformAccessArray = new TransformAccessArray(transformList.ToArray());
                lod0DistanceArray = new NativeArray<float>(lod0DistanceList.ToArray(), Allocator.Persistent);
                distanceToPlayerArray = new NativeArray<float>(distanceToPlayerList.ToArray(), Allocator.Persistent);
                LODLevelArray = new NativeArray<LODLevel>(lodLevelList.ToArray(), Allocator.Persistent);
                /// Update indexes count
                indexes -= 1;
                /// Update assigned index of registered objects
                for (int i = 0; i < indexes; i++)
                {
                    ObjectDistanceToggleList[i].assignedIndex = i;
                }
            }
        }

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        private void OnDestroy()
        {
            if (isQuitting && isActive)
            {
                transformAccessArray.Dispose();
                lod0DistanceArray.Dispose();
                distanceToPlayerArray.Dispose();
                LODLevelArray.Dispose();
            }
        }

    }
}