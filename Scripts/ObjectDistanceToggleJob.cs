namespace TurnTheGameOn.ObjectDistanceToggle
{
    using UnityEngine.Jobs;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Mathematics;

    [BurstCompile]
    public struct LODObjectJob : IJobParallelForTransform
    {
        public float3 playerPosition;
        public NativeArray<float> distanceToPlayerArray;
        public NativeArray<float> lod0DistanceArray;
        public NativeArray<LODLevel> LODLevelArray;

        public void Execute(int index, TransformAccess transformAccess)
        {
            distanceToPlayerArray[index] = math.distance(transformAccess.position, playerPosition);

            if (distanceToPlayerArray[index] > lod0DistanceArray[index])
            {
                LODLevelArray[index] = LODLevel.CULLED;
            }
            else
            {
                LODLevelArray[index] = LODLevel.LOD0;
            }
        }
    }
}