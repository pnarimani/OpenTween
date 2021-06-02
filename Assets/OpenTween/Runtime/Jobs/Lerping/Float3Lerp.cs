﻿using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
#if UNITY_COLLECTIONS
using NativeListInt = Unity.Collections.NativeList<int>;

#else
using NativeListInt = OpenTween.Jobs.NativeList<int>;
#endif

namespace OpenTween.Jobs
{
    [BurstCompile]
    internal struct Float3Lerp : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] [ReadOnly]
        public NativeArray<TweenOptions<float3>> Options;

        [ReadOnly] [NativeDisableParallelForRestriction]
        public NativeListInt Indices;

        [NativeDisableParallelForRestriction] public NativeArray<TweenInternal<float3>> Array;

        public void Execute(int i)
        {
            int index = Indices[i];
            TweenInternal<float3> t = Array[index];
            if (Hint.Unlikely(!t.IsUpdatedInLastFrame))
                return;
            
            TweenOptions<float3> options = Options[index];
            var end = options.IsRelative ? options.Start + options.End : options.End;
            t.CurrentValue = options.IsFrom
                ? math.lerp(end, options.Start, t.LerpParameter)
                : math.lerp(options.Start, end, t.LerpParameter);
            Array[index] = t;
        }
    }
}