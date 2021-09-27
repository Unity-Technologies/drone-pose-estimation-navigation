// This is auto-generated -- do not modify directly
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Barracuda {
internal static partial class BurstSchedulingHelper
{

    #region Private scheduling helpers with pointer aliasing verification for mode: _Float

    private static unsafe JobHandle ScheduleXSBOInternal_Float<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        float* ptrS,
        float* ptrB,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Float
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.S = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrS};
        jobDataInternalCopy.B = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrB};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXBOInternal_Float<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        float* ptrB,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Float
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.B = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrB};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXOInternal_Float<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXOInternal_Float<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        float* ptrO)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleOInternal_Float<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationO_Float
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleOInternal_Float<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrO)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationO_Float
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(fenceBeforeJobStart);
    }

    #endregion

    #region Immediate scheduling helper for mode: _Float

    internal static unsafe JobHandle ScheduleXSBO_Float<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinS,
        BurstTensorData pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXSBO(pinX, pinS, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrS = pinS.array.AddressAt(pinS.offset);
            float* ptrB = pinB.array.AddressAt(pinB.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);

            jobFence = ScheduleXSBOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrS, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXSBOFences(pinX, pinS, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXSBO_Float<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinS,
        FencedMemoryAlloc pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXSBO(pinX, pinS, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            var ptrS = pinS.floatdata;
            var ptrB = pinB.floatdata;
            jobFence = ScheduleXSBOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrS, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXSBOFences(pinX, pinS, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_Float<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrB = pinB.array.AddressAt(pinB.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXBOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_Float<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinB,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            var ptrB = pinB.floatdata;
            var ptrO = pinO.floatdata;
            jobFence = ScheduleXBOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_Float<T>(this T jobData,
        FencedMemoryAlloc pinX,
        FencedMemoryAlloc pinB,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        var ptrX = pinX.floatdata;
        var ptrB = pinB.floatdata;
        var ptrO = pinO.floatdata;
        JobHandle jobFence = ScheduleXBOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleO_Float<T>(this T jobData,
        BurstTensorData pinO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationO_Float
    {
        var fenceBeforeJobStart = pinO.reuse;

        JobHandle jobFence;
        {
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleOInternal_Float(jobData, fenceBeforeJobStart, ptrO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            pinO.fence = jobFence;
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleO_Float<T>(this T jobData,
        BurstTensorData pinO,
        int offsetO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationO_Float
    {
        var fenceBeforeJobStart = pinO.reuse;

        JobHandle jobFence;
        {
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleOInternal_Float(jobData, fenceBeforeJobStart, ptrO+offsetO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            pinO.fence = jobFence;
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Float<T>(this T jobData,
        BurstTensorData pinX,
        int offsetX,
        BurstTensorData pinO,
        int offsetO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_Float(jobData, fenceBeforeJobStart, ptrX+offsetX, ptrO+offsetO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Float<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Float<T>(this T jobData,
        FencedMemoryAlloc pinX,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        var ptrX = pinX.floatdata;
        var ptrO = pinO.floatdata;
        JobHandle jobFence = ScheduleXOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Float<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Float<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            var ptrO = pinO.floatdata;
            jobFence = ScheduleXOInternal_Float(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static void ScheduleXO_Float<T>(this T jobData, Tensor X, Tensor O)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXO_Float(pinX, pinO);
    }

    internal static void ScheduleXO_Float<T>(this T jobData, Tensor X, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Float
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXO_Float(pinX, pinO, arrayLength, innerloopBatchCount);
    }

    internal static void ScheduleXBO_Float<T>(this T jobData, Tensor X, Tensor B, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Float
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinB = BurstCPUOps.Pin(B);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXBO_Float(pinX, pinB, pinO, arrayLength, innerloopBatchCount);
    }

    internal static void ScheduleXSBO_Float<T>(this T jobData, Tensor X, Tensor S, Tensor B, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Float
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinS = BurstCPUOps.Pin(S);
        var pinB = BurstCPUOps.Pin(B);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXSBO_Float(pinX, pinS, pinB, pinO, arrayLength, innerloopBatchCount);
    }

    #endregion

    #region Private scheduling helpers with pointer aliasing verification for mode: _WAsHalf

    private static unsafe JobHandle ScheduleXSBOInternal_WAsHalf<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        half* ptrS,
        half* ptrB,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_WAsHalf
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.S = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrS};
        jobDataInternalCopy.B = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrB};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXBOInternal_WAsHalf<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        half* ptrB,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_WAsHalf
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.B = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrB};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXOInternal_WAsHalf<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXOInternal_WAsHalf<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrX,
        float* ptrO)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResource() {ptr = ptrX};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleOInternal_WAsHalf<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationO_WAsHalf
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleOInternal_WAsHalf<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        float* ptrO)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationO_WAsHalf
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResource() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(fenceBeforeJobStart);
    }

    #endregion

    #region Immediate scheduling helper for mode: _WAsHalf

    internal static unsafe JobHandle ScheduleXSBO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinS,
        BurstTensorData pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXSBO(pinX, pinS, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            half* ptrS = pinS.array.HalfAddressAt(pinS.offset);
            half* ptrB = pinB.array.HalfAddressAt(pinB.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);

            jobFence = ScheduleXSBOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrS, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXSBOFences(pinX, pinS, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXSBO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinS,
        FencedMemoryAlloc pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXSBO(pinX, pinS, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            var ptrS = pinS.halfdata;
            var ptrB = pinB.halfdata;
            jobFence = ScheduleXSBOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrS, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXSBOFences(pinX, pinS, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            half* ptrB = pinB.array.HalfAddressAt(pinB.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXBOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinB,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            var ptrB = pinB.halfdata;
            var ptrO = pinO.floatdata;
            jobFence = ScheduleXBOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_WAsHalf<T>(this T jobData,
        FencedMemoryAlloc pinX,
        FencedMemoryAlloc pinB,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        var ptrX = pinX.floatdata;
        var ptrB = pinB.halfdata;
        var ptrO = pinO.floatdata;
        JobHandle jobFence = ScheduleXBOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleO_WAsHalf<T>(this T jobData,
        BurstTensorData pinO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationO_WAsHalf
    {
        var fenceBeforeJobStart = pinO.reuse;

        JobHandle jobFence;
        {
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            pinO.fence = jobFence;
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleO_WAsHalf<T>(this T jobData,
        BurstTensorData pinO,
        int offsetO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationO_WAsHalf
    {
        var fenceBeforeJobStart = pinO.reuse;

        JobHandle jobFence;
        {
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrO+offsetO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            pinO.fence = jobFence;
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        int offsetX,
        BurstTensorData pinO,
        int offsetO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX+offsetX, ptrO+offsetO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_WAsHalf<T>(this T jobData,
        FencedMemoryAlloc pinX,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        var ptrX = pinX.floatdata;
        var ptrO = pinO.floatdata;
        JobHandle jobFence = ScheduleXOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            float* ptrO = pinO.array.AddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_WAsHalf<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            float* ptrX = pinX.array.AddressAt(pinX.offset);
            var ptrO = pinO.floatdata;
            jobFence = ScheduleXOInternal_WAsHalf(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static void ScheduleXO_WAsHalf<T>(this T jobData, Tensor X, Tensor O)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXO_WAsHalf(pinX, pinO);
    }

    internal static void ScheduleXO_WAsHalf<T>(this T jobData, Tensor X, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_WAsHalf
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXO_WAsHalf(pinX, pinO, arrayLength, innerloopBatchCount);
    }

    internal static void ScheduleXBO_WAsHalf<T>(this T jobData, Tensor X, Tensor B, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_WAsHalf
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinB = BurstCPUOps.Pin(B);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXBO_WAsHalf(pinX, pinB, pinO, arrayLength, innerloopBatchCount);
    }

    internal static void ScheduleXSBO_WAsHalf<T>(this T jobData, Tensor X, Tensor S, Tensor B, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_WAsHalf
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinS = BurstCPUOps.Pin(S);
        var pinB = BurstCPUOps.Pin(B);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXSBO_WAsHalf(pinX, pinS, pinB, pinO, arrayLength, innerloopBatchCount);
    }

    #endregion

    #region Private scheduling helpers with pointer aliasing verification for mode: _Half

    private static unsafe JobHandle ScheduleXSBOInternal_Half<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        half* ptrX,
        half* ptrS,
        half* ptrB,
        half* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Half
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrX};
        jobDataInternalCopy.S = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrS};
        jobDataInternalCopy.B = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrB};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResourceHalf() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXBOInternal_Half<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        half* ptrX,
        half* ptrB,
        half* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Half
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrX};
        jobDataInternalCopy.B = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrB};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResourceHalf() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXOInternal_Half<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        half* ptrX,
        half* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrX};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResourceHalf() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleXOInternal_Half<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        half* ptrX,
        half* ptrO)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.X = new BurstCPUOps.ReadOnlyMemResourceHalf() {ptr = ptrX};
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResourceHalf() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleOInternal_Half<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        half* ptrO,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationO_Half
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResourceHalf() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(arrayLength, innerloopBatchCount, fenceBeforeJobStart);
    }

    private static unsafe JobHandle ScheduleOInternal_Half<T>(T jobData,
        JobHandle fenceBeforeJobStart,
        half* ptrO)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationO_Half
    {
        T jobDataInternalCopy = jobData;
        jobDataInternalCopy.O = new BurstCPUOps.ReadWriteMemResourceHalf() {ptr = ptrO};
        return jobDataInternalCopy.Schedule(fenceBeforeJobStart);
    }

    #endregion

    #region Immediate scheduling helper for mode: _Half

    internal static unsafe JobHandle ScheduleXSBO_Half<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinS,
        BurstTensorData pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXSBO(pinX, pinS, pinB, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            half* ptrS = pinS.array.HalfAddressAt(pinS.offset);
            half* ptrB = pinB.array.HalfAddressAt(pinB.offset);
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);

            jobFence = ScheduleXSBOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrS, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXSBOFences(pinX, pinS, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXSBO_Half<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinS,
        FencedMemoryAlloc pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXSBO(pinX, pinS, pinB, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            var ptrS = pinS.halfdata;
            var ptrB = pinB.halfdata;
            jobFence = ScheduleXSBOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrS, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXSBOFences(pinX, pinS, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_Half<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinB,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            half* ptrB = pinB.array.HalfAddressAt(pinB.offset);
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            jobFence = ScheduleXBOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_Half<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinB,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            var ptrB = pinB.halfdata;
            var ptrO = pinO.halfdata;
            jobFence = ScheduleXBOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXBO_Half<T>(this T jobData,
        FencedMemoryAlloc pinX,
        FencedMemoryAlloc pinB,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXBO(pinX, pinB, pinO);

        var ptrX = pinX.halfdata;
        var ptrB = pinB.halfdata;
        var ptrO = pinO.halfdata;
        JobHandle jobFence = ScheduleXBOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrB, ptrO, arrayLength, innerloopBatchCount);

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXBOFences(pinX, pinB, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleO_Half<T>(this T jobData,
        BurstTensorData pinO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationO_Half
    {
        var fenceBeforeJobStart = pinO.reuse;

        JobHandle jobFence;
        {
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            jobFence = ScheduleOInternal_Half(jobData, fenceBeforeJobStart, ptrO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            pinO.fence = jobFence;
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleO_Half<T>(this T jobData,
        BurstTensorData pinO,
        int offsetO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationO_Half
    {
        var fenceBeforeJobStart = pinO.reuse;

        JobHandle jobFence;
        {
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            jobFence = ScheduleOInternal_Half(jobData, fenceBeforeJobStart, ptrO+offsetO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            pinO.fence = jobFence;
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Half<T>(this T jobData,
        BurstTensorData pinX,
        int offsetX,
        BurstTensorData pinO,
        int offsetO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_Half(jobData, fenceBeforeJobStart, ptrX+offsetX, ptrO+offsetO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Half<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Half<T>(this T jobData,
        FencedMemoryAlloc pinX,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        var ptrX = pinX.halfdata;
        var ptrO = pinO.halfdata;
        JobHandle jobFence = ScheduleXOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Half<T>(this T jobData,
        BurstTensorData pinX,
        BurstTensorData pinO,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            half* ptrO = pinO.array.HalfAddressAt(pinO.offset);
            jobFence = ScheduleXOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrO);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static unsafe JobHandle ScheduleXO_Half<T>(this T jobData,
        BurstTensorData pinX,
        FencedMemoryAlloc pinO,
        int arrayLength, int innerloopBatchCount,
        FencingHelperMode fencingMode=FencingHelperMode.UpdateResourcesFencesOnScheduling)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var fenceBeforeJobStart = GetFenceBeforeJobStartXO(pinX, pinO);

        JobHandle jobFence;
        {
            half* ptrX = pinX.array.HalfAddressAt(pinX.offset);
            var ptrO = pinO.halfdata;
            jobFence = ScheduleXOInternal_Half(jobData, fenceBeforeJobStart, ptrX, ptrO, arrayLength, innerloopBatchCount);
        }

        if (fencingMode==FencingHelperMode.UpdateResourcesFencesOnScheduling)
        {
            jobFence.SetXOFences(pinX, pinO);
        }

        return jobFence;
    }

    internal static void ScheduleXO_Half<T>(this T jobData, Tensor X, Tensor O)
        where T : struct, IJob, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXO_Half(pinX, pinO);
    }

    internal static void ScheduleXO_Half<T>(this T jobData, Tensor X, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXO_Half
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXO_Half(pinX, pinO, arrayLength, innerloopBatchCount);
    }

    internal static void ScheduleXBO_Half<T>(this T jobData, Tensor X, Tensor B, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXBO_Half
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinB = BurstCPUOps.Pin(B);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXBO_Half(pinX, pinB, pinO, arrayLength, innerloopBatchCount);
    }

    internal static void ScheduleXSBO_Half<T>(this T jobData, Tensor X, Tensor S, Tensor B, Tensor O,
        int arrayLength, int innerloopBatchCount)
        where T : struct, IJobParallelFor, BurstCPUOps.IJobResourceDeclarationXSBO_Half
    {
        var pinX = BurstCPUOps.Pin(X);
        var pinS = BurstCPUOps.Pin(S);
        var pinB = BurstCPUOps.Pin(B);
        var pinO = BurstCPUOps.Pin(O, uploadCache: false);
        jobData.ScheduleXSBO_Half(pinX, pinS, pinB, pinO, arrayLength, innerloopBatchCount);
    }

    #endregion
}
}
