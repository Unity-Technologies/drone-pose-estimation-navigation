// This is auto-generated -- do not modify directly
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Burst.Intrinsics.X86.Avx;
using static Unity.Burst.Intrinsics.X86.Fma;

namespace Unity.Barracuda {
public partial class BurstCPUOps
{

    #region Resources declaration for mode: _Float
    internal interface IJobResourceDeclarationO_Float
    {
        ReadWriteMemResource O { get; set; }
    }

    internal interface IJobResourceDeclarationXO_Float
    {
        ReadOnlyMemResource  X { get; set; }
        ReadWriteMemResource O { get; set; }
    }

    internal interface IJobResourceDeclarationXBO_Float
    {
        ReadOnlyMemResource  X { get; set; }
        ReadOnlyMemResource  B { get; set; }
        ReadWriteMemResource O { get; set; }
    }

    internal interface IJobResourceDeclarationXSBO_Float
    {
        ReadOnlyMemResource  X { get; set; }
        ReadOnlyMemResource  S { get; set; }
        ReadOnlyMemResource  B { get; set; }
        ReadWriteMemResource O { get; set; }
    }
    #endregion

    #region Jobs declaration for mode: _Float

    internal struct VectorBroadcastScaleBiasJobHelper
    {
        [ReadOnly] public int inOutChannels;
        [ReadOnly] public float alpha;
        
        public void ScheduleXSBO(Tensor X, Tensor S, Tensor B, Tensor O, int arrayLength, int innerBatchCount)
        {
            var pinX = Pin(X);
            var pinS = Pin(S);
            var pinB = Pin(B);
            var pinO = Pin(O, uploadCache: false);
            bool AHalf = pinX.array.Type == BarracudaArray.DataType.Half;
            bool WHalf = pinS.array.Type == BarracudaArray.DataType.Half;
            bool BHalf = pinB.array.Type == BarracudaArray.DataType.Half;
            bool OHalf = pinO.array.Type == BarracudaArray.DataType.Half;
            UnityEngine.Assertions.Assert.AreEqual(AHalf, OHalf);
            UnityEngine.Assertions.Assert.AreEqual(WHalf, BHalf);
            if (AHalf && WHalf)
            {
                var job = new VectorBroadcastScaleBiasJob_Half();
                job.data = this;
                job.ScheduleXSBO_Half(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && WHalf)
            {
                var job = new VectorBroadcastScaleBiasJob_WAsHalf();
                job.data = this;
                job.ScheduleXSBO_WAsHalf(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && !WHalf)
            {
                var job = new VectorBroadcastScaleBiasJob_Float();
                job.data = this;
                job.ScheduleXSBO_Float(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else //if (AHalf && !WHalf)
            {
                UnityEngine.Assertions.Assert.IsTrue(false, "VectorBroadcastScaleBiasJob does not support activation as half while weights are floats.");
            }
        }
    }
    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct VectorBroadcastScaleBiasJob_Float : IJobParallelFor, IJobResourceDeclarationXSBO_Float
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResource S { get; set; }
        public ReadOnlyMemResource B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public VectorBroadcastScaleBiasJobHelper data;

        const int unrollSize = 32;
        public void Execute(int i)
        {
            float* src   = X.ptr + i * data.inOutChannels;
            float* dst   = O.ptr + i * data.inOutChannels;
            float* gamma = S.ptr;
            float* beta  = B.ptr;

            int j = 0;
            for (; j < data.inOutChannels - unrollSize + 1; j += unrollSize) // unroll of inOutChannels loop
                for (int q = 0; q < unrollSize; q++, src++, dst++, gamma++, beta++)
                    *dst = (float)((*src) * (*gamma) + (*beta) * data.alpha);
            for (; j < data.inOutChannels; j++, src++, dst++, gamma++, beta++) // remainder of inOutChannels loop
                *dst = (float)((*src) * (*gamma) + (*beta) * data.alpha);
        }
    }

    internal struct ReduceMaxJobHelper
    {
        [ReadOnly] public int offsetReduce;
        [ReadOnly] public int reduceDim;
        
        public void ScheduleXO(BurstTensorData pinX, FencedMemoryAlloc pinO, int arrayLength, int innerBatchCount)
        {
            bool AHalf = pinX.array.Type == BarracudaArray.DataType.Half;
            bool OHalf = pinO.type == BarracudaArray.DataType.Half;
            UnityEngine.Assertions.Assert.AreEqual(AHalf, OHalf);
            if (AHalf)
            {
                var job = new ReduceMaxJob_Half();
                job.data = this;
                job.ScheduleXO_Half(pinX, pinO, arrayLength, innerBatchCount);
            }
            else
            {
                var job = new ReduceMaxJob_Float();
                job.data = this;
                job.ScheduleXO_Float(pinX, pinO, arrayLength, innerBatchCount);
            }
        }
    }
    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct ReduceMaxJob_Float : IJobParallelFor, IJobResourceDeclarationXO_Float
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadWriteMemResource O { get; set; }
        public ReduceMaxJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = i / data.offsetReduce;

            float maxV = float.MinValue;
            for (int z = 0; z < data.reduceDim; ++z)
            {
                float v = X.ptr[y * data.offsetReduce * data.reduceDim + z * data.offsetReduce + x];
                maxV = math.max(maxV, v);
            }
            O.ptr[y * data.offsetReduce + x] = (float)maxV;
        }
    }

    internal struct ExpBiasReduceJobHelper
    {
        [ReadOnly] public int offsetReduce;
        [ReadOnly] public int reduceDim;
        
        public void ScheduleXBO(BurstTensorData pinX, FencedMemoryAlloc pinB, FencedMemoryAlloc pinO, int arrayLength, int innerBatchCount)
        {
            bool AHalf = pinX.array.Type == BarracudaArray.DataType.Half;
            bool WHalf = pinB.type == BarracudaArray.DataType.Half;
            bool OHalf = pinO.type == BarracudaArray.DataType.Half;
            UnityEngine.Assertions.Assert.AreEqual(AHalf, OHalf);
            if (AHalf && WHalf)
            {
                var job = new ExpBiasReduceJob_Half();
                job.data = this;
                job.ScheduleXBO_Half(pinX, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && WHalf)
            {
                var job = new ExpBiasReduceJob_WAsHalf();
                job.data = this;
                job.ScheduleXBO_WAsHalf(pinX, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && !WHalf)
            {
                var job = new ExpBiasReduceJob_Float();
                job.data = this;
                job.ScheduleXBO_Float(pinX, pinB, pinO, arrayLength, innerBatchCount);
            }
            else //if (AHalf && !WHalf)
            {
                UnityEngine.Assertions.Assert.IsTrue(false, "ExpBiasReduceJob does not support activation as half while weights are floats.");
            }
        }
    }
    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct ExpBiasReduceJob_Float : IJobParallelFor, IJobResourceDeclarationXBO_Float
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResource B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public ExpBiasReduceJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = i / data.offsetReduce;

            float accum = 0.0f;
            for (int z = 0; z < data.reduceDim; ++z)
            {
                float v = X.ptr[y * data.offsetReduce * data.reduceDim + z * data.offsetReduce + x];
                float b = B.ptr[y * data.offsetReduce + x];
                accum += math.exp(v - b);
            }
            O.ptr[y * data.offsetReduce + x] = (float)accum;
        }
    }

    internal struct SoftmaxEndJobHelper
    {
        [ReadOnly] public int offsetReduce;
        [ReadOnly] public int reduceDim;
        
        public void ScheduleXSBO(BurstTensorData pinX, FencedMemoryAlloc pinS, FencedMemoryAlloc pinB, BurstTensorData pinO, int arrayLength, int innerBatchCount)
        {
            bool AHalf = pinX.array.Type == BarracudaArray.DataType.Half;
            bool WHalf = pinS.type == BarracudaArray.DataType.Half;
            bool BHalf = pinB.type == BarracudaArray.DataType.Half;
            bool OHalf = pinO.array.Type == BarracudaArray.DataType.Half;
            UnityEngine.Assertions.Assert.AreEqual(AHalf, OHalf);
            UnityEngine.Assertions.Assert.AreEqual(WHalf, BHalf);
            if (AHalf && WHalf)
            {
                var job = new SoftmaxEndJob_Half();
                job.data = this;
                job.ScheduleXSBO_Half(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && WHalf)
            {
                var job = new SoftmaxEndJob_WAsHalf();
                job.data = this;
                job.ScheduleXSBO_WAsHalf(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && !WHalf)
            {
                var job = new SoftmaxEndJob_Float();
                job.data = this;
                job.ScheduleXSBO_Float(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else //if (AHalf && !WHalf)
            {
                UnityEngine.Assertions.Assert.IsTrue(false, "SoftmaxEndJob does not support activation as half while weights are floats.");
            }
        }
    }
    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard)]
    unsafe struct SoftmaxEndJob_Float : IJobParallelFor, IJobResourceDeclarationXSBO_Float
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResource S { get; set; }
        public ReadOnlyMemResource B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public SoftmaxEndJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = ((i / data.offsetReduce) % data.reduceDim);
            int z = ((i / data.offsetReduce) / data.reduceDim);

            O.ptr[i] = (float)(math.exp(X.ptr[i] - B.ptr[z * data.offsetReduce + x]) / S.ptr[z * data.offsetReduce + x]);
        }
    }

    internal struct ReluJobHelper
    {
        
        public void ScheduleXO(Tensor X, Tensor O, int arrayLength, int innerBatchCount)
        {
            var pinX = Pin(X);
            var pinO = Pin(O, uploadCache: false);
            bool AHalf = pinX.array.Type == BarracudaArray.DataType.Half;
            bool OHalf = pinO.array.Type == BarracudaArray.DataType.Half;
            UnityEngine.Assertions.Assert.AreEqual(AHalf, OHalf);
            if (AHalf)
            {
                var job = new ReluJob_Half();
                job.data = this;
                job.ScheduleXO_Half(pinX, pinO, arrayLength, innerBatchCount);
            }
            else
            {
                var job = new ReluJob_Float();
                job.data = this;
                job.ScheduleXO_Float(pinX, pinO, arrayLength, innerBatchCount);
            }
        }
    }
    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard)]
    unsafe struct ReluJob_Float : IJobParallelFor, IJobResourceDeclarationXO_Float
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadWriteMemResource O { get; set; }
        public ReluJobHelper data;

        public void Execute(int i)
        {
            float v = X.ptr[i];
            // NOTE: burst-1.2.3 has troubles with Math.Min/Max generating poorly vectorized and branch code
            // Instead Math.Abs based code is used instead. (Math.Abs just flips 1 bit)
            O.ptr[i] = (float)(0.5f * (v + math.abs(v)));
        }
    }

    internal struct Dense3JobHelper
    {
        public int AM, AN;
        public int BM, BN;
        public int SM, SN;
        public int dispatchThreadX, dispatchThreadY, dispatchThreadZ;
        
        public void ScheduleXSBO(Tensor X, Tensor S, Tensor B, Tensor O, int arrayLength, int innerBatchCount)
        {
            var pinX = Pin(X);
            var pinS = Pin(S);
            var pinB = Pin(B);
            var pinO = Pin(O, uploadCache: false);
            bool AHalf = pinX.array.Type == BarracudaArray.DataType.Half;
            bool WHalf = pinS.array.Type == BarracudaArray.DataType.Half;
            bool BHalf = pinB.array.Type == BarracudaArray.DataType.Half;
            bool OHalf = pinO.array.Type == BarracudaArray.DataType.Half;
            UnityEngine.Assertions.Assert.AreEqual(AHalf, OHalf);
            UnityEngine.Assertions.Assert.AreEqual(WHalf, BHalf);
            if (AHalf && WHalf)
            {
                var job = new Dense3Job_Half();
                job.data = this;
                job.ScheduleXSBO_Half(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && WHalf)
            {
                var job = new Dense3Job_WAsHalf();
                job.data = this;
                job.ScheduleXSBO_WAsHalf(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else if (!AHalf && !WHalf)
            {
                var job = new Dense3Job_Float();
                job.data = this;
                job.ScheduleXSBO_Float(pinX, pinS, pinB, pinO, arrayLength, innerBatchCount);
            }
            else //if (AHalf && !WHalf)
            {
                UnityEngine.Assertions.Assert.IsTrue(false, "Dense3Job does not support activation as half while weights are floats.");
            }
        }
    }
    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct Dense3Job_Float : IJobParallelFor, IJobResourceDeclarationXSBO_Float
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResource S { get; set; }
        public ReadOnlyMemResource B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public Dense3JobHelper data;

        public const int blockSize = 16;
        public void Execute(int threadID)
        {
            float* A = this.X.ptr;
            float* B = this.S.ptr;
            float* C = this.B.ptr;
            float* S = this.O.ptr;
            int AM = data.AM;
            int BM = data.BM;
            int SM = data.SM;
            int AN = data.AN;
            int BN = data.BN;
            int SN = data.SN;

            int dispatchThreadXY = data.dispatchThreadX * data.dispatchThreadY;

            int batch = (threadID / dispatchThreadXY);
            int i = (threadID % dispatchThreadXY) % data.dispatchThreadX;
            int j = (threadID % dispatchThreadXY) / data.dispatchThreadX;

            int batchOffSetA = (batch * AM * AN);
            int batchOffSetS = (batch * SM * SN);

            int rowA = i * blockSize;
            int colB = j * blockSize;

            unsafe
            {
                float* blockTempA = null;
                float* blockTempB = null;
                float* blockTempS = null;

                float* blockS = S + rowA + SM * colB + batchOffSetS;
                int strideS = SM;

                if (rowA + blockSize > SM || colB + blockSize > SN) // copy remainder of C into zero-padded block
                {
                    blockTempS = AllocBlock(blockSize, blockSize);
                    strideS = blockSize;
                    blockS = blockTempS;
                }
                for (int y = 0; y < blockSize; y++)
                    for (int x = 0; x < blockSize; x++)
                        blockS[x + strideS * y] = (float)((colB + y) < BN ? C[colB + y] : 0.0f);

                for (int l = 0; l < AN; l += blockSize) // inner-loop
                {
                    float* blockA = A + rowA + AM * l + batchOffSetA;
                    float* blockB = B + l * BN + colB;
                    int strideA = AM;
                    int strideB = BN;

                    if (rowA + blockSize > AM || l + blockSize > AN) // copy remainder of A into zero-padded block
                    {
                        if (blockTempA == null)
                            blockTempA = AllocBlock(blockSize, blockSize);
                        strideA = blockSize;

                        for (int y = 0; y < blockSize; y++)
                            for (int x = 0; x < blockSize; x++)
                                blockTempA[x + blockSize * y] = (float)(((rowA + x) < AM && (l + y < AN)) ? blockA[x + AM * y] : 0.0f);

                        blockA = blockTempA;
                    }

                    if (colB + blockSize > BN || l + blockSize > BM) // copy remainder of B into zero-padded block
                    {
                        if (blockTempB == null)
                            blockTempB = AllocBlock(blockSize, blockSize);
                        strideB = blockSize;

                        for (int y = 0; y < blockSize; y++)
                            for (int x = 0; x < blockSize; x++)
                                blockTempB[x + blockSize * y] = (float)(((colB + x) < BN && (l + y < BM)) ? blockB[x + BN * y] : 0.0f);

                        blockB = blockTempB;
                    }

                    MultiplyBlockUnrollHx16(blockA, strideA, blockB, strideB, blockS, strideS);
                }

                if (blockS == blockTempS) // copy back
                {
                    for (int y = 0; y < blockSize; y++)
                        for (int x = 0; x < blockSize; x++)
                        {
                            if (((rowA + x) < SM) && ((colB + y) < SN))
                                S[(rowA + x) + SM * (colB + y) + batchOffSetS] = blockTempS[x + blockSize * y];
                        }
                }

                FreeBlock(blockTempA);
                FreeBlock(blockTempB);
                FreeBlock(blockTempS);
            }
        }

        static void MultiplyBlockUnrollHx16(float* Ap, int Astride, float* Bp, int Bstride, float* Sp, int Sstride)
        {
            for (int i = 0; i < blockSize; i++)
            {
                float sum0 = *(Sp + i + Sstride * 0);
                float sum1 = *(Sp + i + Sstride * 1);
                float sum2 = *(Sp + i + Sstride * 2);
                float sum3 = *(Sp + i + Sstride * 3);
                float sum4 = *(Sp + i + Sstride * 4);
                float sum5 = *(Sp + i + Sstride * 5);
                float sum6 = *(Sp + i + Sstride * 6);
                float sum7 = *(Sp + i + Sstride * 7);
                float sum8 = *(Sp + i + Sstride * 8);
                float sum9 = *(Sp + i + Sstride * 9);
                float sumA = *(Sp + i + Sstride * 10);
                float sumB = *(Sp + i + Sstride * 11);
                float sumC = *(Sp + i + Sstride * 12);
                float sumD = *(Sp + i + Sstride * 13);
                float sumE = *(Sp + i + Sstride * 14);
                float sumF = *(Sp + i + Sstride * 15);

                for (int l = 0; l < blockSize; l++)
                {
                    float A = *(Ap + i + Astride * l);

                    float B0 = *(Bp + l * Bstride + 0);
                    float B1 = *(Bp + l * Bstride + 1);
                    float B2 = *(Bp + l * Bstride + 2);
                    float B3 = *(Bp + l * Bstride + 3);
                    float B4 = *(Bp + l * Bstride + 4);
                    float B5 = *(Bp + l * Bstride + 5);
                    float B6 = *(Bp + l * Bstride + 6);
                    float B7 = *(Bp + l * Bstride + 7);
                    float B8 = *(Bp + l * Bstride + 8);
                    float B9 = *(Bp + l * Bstride + 9);
                    float BA = *(Bp + l * Bstride + 10);
                    float BB = *(Bp + l * Bstride + 11);
                    float BC = *(Bp + l * Bstride + 12);
                    float BD = *(Bp + l * Bstride + 13);
                    float BE = *(Bp + l * Bstride + 14);
                    float BF = *(Bp + l * Bstride + 15);


                    sum0 += A * B0;
                    sum1 += A * B1;
                    sum2 += A * B2;
                    sum3 += A * B3;
                    sum4 += A * B4;
                    sum5 += A * B5;
                    sum6 += A * B6;
                    sum7 += A * B7;
                    sum8 += A * B8;
                    sum9 += A * B9;
                    sumA += A * BA;
                    sumB += A * BB;
                    sumC += A * BC;
                    sumD += A * BD;
                    sumE += A * BE;
                    sumF += A * BF;
                }

                *(Sp + i + Sstride * 0 ) = (float)(sum0);
                *(Sp + i + Sstride * 1 ) = (float)(sum1);
                *(Sp + i + Sstride * 2 ) = (float)(sum2);
                *(Sp + i + Sstride * 3 ) = (float)(sum3);
                *(Sp + i + Sstride * 4 ) = (float)(sum4);
                *(Sp + i + Sstride * 5 ) = (float)(sum5);
                *(Sp + i + Sstride * 6 ) = (float)(sum6);
                *(Sp + i + Sstride * 7 ) = (float)(sum7);
                *(Sp + i + Sstride * 8 ) = (float)(sum8);
                *(Sp + i + Sstride * 9 ) = (float)(sum9);
                *(Sp + i + Sstride * 10) = (float)(sumA);
                *(Sp + i + Sstride * 11) = (float)(sumB);
                *(Sp + i + Sstride * 12) = (float)(sumC);
                *(Sp + i + Sstride * 13) = (float)(sumD);
                *(Sp + i + Sstride * 14) = (float)(sumE);
                *(Sp + i + Sstride * 15) = (float)(sumF);
            }
        }
    }

    #endregion

    #region Resources declaration for mode: _WAsHalf
    internal interface IJobResourceDeclarationO_WAsHalf
    {
        ReadWriteMemResource O { get; set; }
    }

    internal interface IJobResourceDeclarationXO_WAsHalf
    {
        ReadOnlyMemResource  X { get; set; }
        ReadWriteMemResource O { get; set; }
    }

    internal interface IJobResourceDeclarationXBO_WAsHalf
    {
        ReadOnlyMemResource  X { get; set; }
        ReadOnlyMemResourceHalf  B { get; set; }
        ReadWriteMemResource O { get; set; }
    }

    internal interface IJobResourceDeclarationXSBO_WAsHalf
    {
        ReadOnlyMemResource  X { get; set; }
        ReadOnlyMemResourceHalf  S { get; set; }
        ReadOnlyMemResourceHalf  B { get; set; }
        ReadWriteMemResource O { get; set; }
    }
    #endregion

    #region Jobs declaration for mode: _WAsHalf

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct VectorBroadcastScaleBiasJob_WAsHalf : IJobParallelFor, IJobResourceDeclarationXSBO_WAsHalf
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResourceHalf S { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public VectorBroadcastScaleBiasJobHelper data;

        const int unrollSize = 32;
        public void Execute(int i)
        {
            float* src   = X.ptr + i * data.inOutChannels;
            float* dst   = O.ptr + i * data.inOutChannels;
            half* gamma = S.ptr;
            half* beta  = B.ptr;

            int j = 0;
            for (; j < data.inOutChannels - unrollSize + 1; j += unrollSize) // unroll of inOutChannels loop
                for (int q = 0; q < unrollSize; q++, src++, dst++, gamma++, beta++)
                    *dst = (float)((*src) * (*gamma) + (*beta) * data.alpha);
            for (; j < data.inOutChannels; j++, src++, dst++, gamma++, beta++) // remainder of inOutChannels loop
                *dst = (float)((*src) * (*gamma) + (*beta) * data.alpha);
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct ReduceMaxJob_WAsHalf : IJobParallelFor, IJobResourceDeclarationXO_WAsHalf
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadWriteMemResource O { get; set; }
        public ReduceMaxJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = i / data.offsetReduce;

            float maxV = float.MinValue;
            for (int z = 0; z < data.reduceDim; ++z)
            {
                float v = X.ptr[y * data.offsetReduce * data.reduceDim + z * data.offsetReduce + x];
                maxV = math.max(maxV, v);
            }
            O.ptr[y * data.offsetReduce + x] = (float)maxV;
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct ExpBiasReduceJob_WAsHalf : IJobParallelFor, IJobResourceDeclarationXBO_WAsHalf
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public ExpBiasReduceJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = i / data.offsetReduce;

            float accum = 0.0f;
            for (int z = 0; z < data.reduceDim; ++z)
            {
                float v = X.ptr[y * data.offsetReduce * data.reduceDim + z * data.offsetReduce + x];
                float b = B.ptr[y * data.offsetReduce + x];
                accum += math.exp(v - b);
            }
            O.ptr[y * data.offsetReduce + x] = (float)accum;
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard)]
    unsafe struct SoftmaxEndJob_WAsHalf : IJobParallelFor, IJobResourceDeclarationXSBO_WAsHalf
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResourceHalf S { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public SoftmaxEndJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = ((i / data.offsetReduce) % data.reduceDim);
            int z = ((i / data.offsetReduce) / data.reduceDim);

            O.ptr[i] = (float)(math.exp(X.ptr[i] - B.ptr[z * data.offsetReduce + x]) / S.ptr[z * data.offsetReduce + x]);
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard)]
    unsafe struct ReluJob_WAsHalf : IJobParallelFor, IJobResourceDeclarationXO_WAsHalf
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadWriteMemResource O { get; set; }
        public ReluJobHelper data;

        public void Execute(int i)
        {
            float v = X.ptr[i];
            // NOTE: burst-1.2.3 has troubles with Math.Min/Max generating poorly vectorized and branch code
            // Instead Math.Abs based code is used instead. (Math.Abs just flips 1 bit)
            O.ptr[i] = (float)(0.5f * (v + math.abs(v)));
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct Dense3Job_WAsHalf : IJobParallelFor, IJobResourceDeclarationXSBO_WAsHalf
    {
        public ReadOnlyMemResource X { get; set; }
        public ReadOnlyMemResourceHalf S { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResource O { get; set; }
        public Dense3JobHelper data;

        public const int blockSize = 16;
        public void Execute(int threadID)
        {
            float* A = this.X.ptr;
            half* B = this.S.ptr;
            half* C = this.B.ptr;
            float* S = this.O.ptr;
            int AM = data.AM;
            int BM = data.BM;
            int SM = data.SM;
            int AN = data.AN;
            int BN = data.BN;
            int SN = data.SN;

            int dispatchThreadXY = data.dispatchThreadX * data.dispatchThreadY;

            int batch = (threadID / dispatchThreadXY);
            int i = (threadID % dispatchThreadXY) % data.dispatchThreadX;
            int j = (threadID % dispatchThreadXY) / data.dispatchThreadX;

            int batchOffSetA = (batch * AM * AN);
            int batchOffSetS = (batch * SM * SN);

            int rowA = i * blockSize;
            int colB = j * blockSize;

            unsafe
            {
                float* blockTempA = null;
                half* blockTempB = null;
                float* blockTempS = null;

                float* blockS = S + rowA + SM * colB + batchOffSetS;
                int strideS = SM;

                if (rowA + blockSize > SM || colB + blockSize > SN) // copy remainder of C into zero-padded block
                {
                    blockTempS = AllocBlock(blockSize, blockSize);
                    strideS = blockSize;
                    blockS = blockTempS;
                }
                for (int y = 0; y < blockSize; y++)
                    for (int x = 0; x < blockSize; x++)
                        blockS[x + strideS * y] = (float)((colB + y) < BN ? C[colB + y] : 0.0f);

                for (int l = 0; l < AN; l += blockSize) // inner-loop
                {
                    float* blockA = A + rowA + AM * l + batchOffSetA;
                    half* blockB = B + l * BN + colB;
                    int strideA = AM;
                    int strideB = BN;

                    if (rowA + blockSize > AM || l + blockSize > AN) // copy remainder of A into zero-padded block
                    {
                        if (blockTempA == null)
                            blockTempA = AllocBlock(blockSize, blockSize);
                        strideA = blockSize;

                        for (int y = 0; y < blockSize; y++)
                            for (int x = 0; x < blockSize; x++)
                                blockTempA[x + blockSize * y] = (float)(((rowA + x) < AM && (l + y < AN)) ? blockA[x + AM * y] : 0.0f);

                        blockA = blockTempA;
                    }

                    if (colB + blockSize > BN || l + blockSize > BM) // copy remainder of B into zero-padded block
                    {
                        if (blockTempB == null)
                            blockTempB = AllocBlockHalf(blockSize, blockSize);
                        strideB = blockSize;

                        for (int y = 0; y < blockSize; y++)
                            for (int x = 0; x < blockSize; x++)
                                blockTempB[x + blockSize * y] = (half)(((colB + x) < BN && (l + y < BM)) ? blockB[x + BN * y] : 0.0f);

                        blockB = blockTempB;
                    }

                    MultiplyBlockUnrollHx16(blockA, strideA, blockB, strideB, blockS, strideS);
                }

                if (blockS == blockTempS) // copy back
                {
                    for (int y = 0; y < blockSize; y++)
                        for (int x = 0; x < blockSize; x++)
                        {
                            if (((rowA + x) < SM) && ((colB + y) < SN))
                                S[(rowA + x) + SM * (colB + y) + batchOffSetS] = blockTempS[x + blockSize * y];
                        }
                }

                FreeBlock(blockTempA);
                FreeBlock(blockTempB);
                FreeBlock(blockTempS);
            }
        }

        static void MultiplyBlockUnrollHx16(float* Ap, int Astride, half* Bp, int Bstride, float* Sp, int Sstride)
        {
            for (int i = 0; i < blockSize; i++)
            {
                float sum0 = *(Sp + i + Sstride * 0);
                float sum1 = *(Sp + i + Sstride * 1);
                float sum2 = *(Sp + i + Sstride * 2);
                float sum3 = *(Sp + i + Sstride * 3);
                float sum4 = *(Sp + i + Sstride * 4);
                float sum5 = *(Sp + i + Sstride * 5);
                float sum6 = *(Sp + i + Sstride * 6);
                float sum7 = *(Sp + i + Sstride * 7);
                float sum8 = *(Sp + i + Sstride * 8);
                float sum9 = *(Sp + i + Sstride * 9);
                float sumA = *(Sp + i + Sstride * 10);
                float sumB = *(Sp + i + Sstride * 11);
                float sumC = *(Sp + i + Sstride * 12);
                float sumD = *(Sp + i + Sstride * 13);
                float sumE = *(Sp + i + Sstride * 14);
                float sumF = *(Sp + i + Sstride * 15);

                for (int l = 0; l < blockSize; l++)
                {
                    float A = *(Ap + i + Astride * l);

                    float B0 = *(Bp + l * Bstride + 0);
                    float B1 = *(Bp + l * Bstride + 1);
                    float B2 = *(Bp + l * Bstride + 2);
                    float B3 = *(Bp + l * Bstride + 3);
                    float B4 = *(Bp + l * Bstride + 4);
                    float B5 = *(Bp + l * Bstride + 5);
                    float B6 = *(Bp + l * Bstride + 6);
                    float B7 = *(Bp + l * Bstride + 7);
                    float B8 = *(Bp + l * Bstride + 8);
                    float B9 = *(Bp + l * Bstride + 9);
                    float BA = *(Bp + l * Bstride + 10);
                    float BB = *(Bp + l * Bstride + 11);
                    float BC = *(Bp + l * Bstride + 12);
                    float BD = *(Bp + l * Bstride + 13);
                    float BE = *(Bp + l * Bstride + 14);
                    float BF = *(Bp + l * Bstride + 15);


                    sum0 += A * B0;
                    sum1 += A * B1;
                    sum2 += A * B2;
                    sum3 += A * B3;
                    sum4 += A * B4;
                    sum5 += A * B5;
                    sum6 += A * B6;
                    sum7 += A * B7;
                    sum8 += A * B8;
                    sum9 += A * B9;
                    sumA += A * BA;
                    sumB += A * BB;
                    sumC += A * BC;
                    sumD += A * BD;
                    sumE += A * BE;
                    sumF += A * BF;
                }

                *(Sp + i + Sstride * 0 ) = (float)(sum0);
                *(Sp + i + Sstride * 1 ) = (float)(sum1);
                *(Sp + i + Sstride * 2 ) = (float)(sum2);
                *(Sp + i + Sstride * 3 ) = (float)(sum3);
                *(Sp + i + Sstride * 4 ) = (float)(sum4);
                *(Sp + i + Sstride * 5 ) = (float)(sum5);
                *(Sp + i + Sstride * 6 ) = (float)(sum6);
                *(Sp + i + Sstride * 7 ) = (float)(sum7);
                *(Sp + i + Sstride * 8 ) = (float)(sum8);
                *(Sp + i + Sstride * 9 ) = (float)(sum9);
                *(Sp + i + Sstride * 10) = (float)(sumA);
                *(Sp + i + Sstride * 11) = (float)(sumB);
                *(Sp + i + Sstride * 12) = (float)(sumC);
                *(Sp + i + Sstride * 13) = (float)(sumD);
                *(Sp + i + Sstride * 14) = (float)(sumE);
                *(Sp + i + Sstride * 15) = (float)(sumF);
            }
        }
    }

    #endregion

    #region Resources declaration for mode: _Half
    internal interface IJobResourceDeclarationO_Half
    {
        ReadWriteMemResourceHalf O { get; set; }
    }

    internal interface IJobResourceDeclarationXO_Half
    {
        ReadOnlyMemResourceHalf  X { get; set; }
        ReadWriteMemResourceHalf O { get; set; }
    }

    internal interface IJobResourceDeclarationXBO_Half
    {
        ReadOnlyMemResourceHalf  X { get; set; }
        ReadOnlyMemResourceHalf  B { get; set; }
        ReadWriteMemResourceHalf O { get; set; }
    }

    internal interface IJobResourceDeclarationXSBO_Half
    {
        ReadOnlyMemResourceHalf  X { get; set; }
        ReadOnlyMemResourceHalf  S { get; set; }
        ReadOnlyMemResourceHalf  B { get; set; }
        ReadWriteMemResourceHalf O { get; set; }
    }
    #endregion

    #region Jobs declaration for mode: _Half

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct VectorBroadcastScaleBiasJob_Half : IJobParallelFor, IJobResourceDeclarationXSBO_Half
    {
        public ReadOnlyMemResourceHalf X { get; set; }
        public ReadOnlyMemResourceHalf S { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResourceHalf O { get; set; }
        public VectorBroadcastScaleBiasJobHelper data;

        const int unrollSize = 32;
        public void Execute(int i)
        {
            half* src   = X.ptr + i * data.inOutChannels;
            half* dst   = O.ptr + i * data.inOutChannels;
            half* gamma = S.ptr;
            half* beta  = B.ptr;

            int j = 0;
            for (; j < data.inOutChannels - unrollSize + 1; j += unrollSize) // unroll of inOutChannels loop
                for (int q = 0; q < unrollSize; q++, src++, dst++, gamma++, beta++)
                    *dst = (half)((*src) * (*gamma) + (*beta) * data.alpha);
            for (; j < data.inOutChannels; j++, src++, dst++, gamma++, beta++) // remainder of inOutChannels loop
                *dst = (half)((*src) * (*gamma) + (*beta) * data.alpha);
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct ReduceMaxJob_Half : IJobParallelFor, IJobResourceDeclarationXO_Half
    {
        public ReadOnlyMemResourceHalf X { get; set; }
        public ReadWriteMemResourceHalf O { get; set; }
        public ReduceMaxJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = i / data.offsetReduce;

            float maxV = float.MinValue;
            for (int z = 0; z < data.reduceDim; ++z)
            {
                float v = X.ptr[y * data.offsetReduce * data.reduceDim + z * data.offsetReduce + x];
                maxV = math.max(maxV, v);
            }
            O.ptr[y * data.offsetReduce + x] = (half)maxV;
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct ExpBiasReduceJob_Half : IJobParallelFor, IJobResourceDeclarationXBO_Half
    {
        public ReadOnlyMemResourceHalf X { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResourceHalf O { get; set; }
        public ExpBiasReduceJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = i / data.offsetReduce;

            float accum = 0.0f;
            for (int z = 0; z < data.reduceDim; ++z)
            {
                float v = X.ptr[y * data.offsetReduce * data.reduceDim + z * data.offsetReduce + x];
                float b = B.ptr[y * data.offsetReduce + x];
                accum += math.exp(v - b);
            }
            O.ptr[y * data.offsetReduce + x] = (half)accum;
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard)]
    unsafe struct SoftmaxEndJob_Half : IJobParallelFor, IJobResourceDeclarationXSBO_Half
    {
        public ReadOnlyMemResourceHalf X { get; set; }
        public ReadOnlyMemResourceHalf S { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResourceHalf O { get; set; }
        public SoftmaxEndJobHelper data;

        public void Execute(int i)
        {
            int x = i % data.offsetReduce;
            int y = ((i / data.offsetReduce) % data.reduceDim);
            int z = ((i / data.offsetReduce) / data.reduceDim);

            O.ptr[i] = (half)(math.exp(X.ptr[i] - B.ptr[z * data.offsetReduce + x]) / S.ptr[z * data.offsetReduce + x]);
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard)]
    unsafe struct ReluJob_Half : IJobParallelFor, IJobResourceDeclarationXO_Half
    {
        public ReadOnlyMemResourceHalf X { get; set; }
        public ReadWriteMemResourceHalf O { get; set; }
        public ReluJobHelper data;

        public void Execute(int i)
        {
            float v = X.ptr[i];
            // NOTE: burst-1.2.3 has troubles with Math.Min/Max generating poorly vectorized and branch code
            // Instead Math.Abs based code is used instead. (Math.Abs just flips 1 bit)
            O.ptr[i] = (half)(0.5f * (v + math.abs(v)));
        }
    }

    [BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    unsafe struct Dense3Job_Half : IJobParallelFor, IJobResourceDeclarationXSBO_Half
    {
        public ReadOnlyMemResourceHalf X { get; set; }
        public ReadOnlyMemResourceHalf S { get; set; }
        public ReadOnlyMemResourceHalf B { get; set; }
        public ReadWriteMemResourceHalf O { get; set; }
        public Dense3JobHelper data;

        public const int blockSize = 16;
        public void Execute(int threadID)
        {
            half* A = this.X.ptr;
            half* B = this.S.ptr;
            half* C = this.B.ptr;
            half* S = this.O.ptr;
            int AM = data.AM;
            int BM = data.BM;
            int SM = data.SM;
            int AN = data.AN;
            int BN = data.BN;
            int SN = data.SN;

            int dispatchThreadXY = data.dispatchThreadX * data.dispatchThreadY;

            int batch = (threadID / dispatchThreadXY);
            int i = (threadID % dispatchThreadXY) % data.dispatchThreadX;
            int j = (threadID % dispatchThreadXY) / data.dispatchThreadX;

            int batchOffSetA = (batch * AM * AN);
            int batchOffSetS = (batch * SM * SN);

            int rowA = i * blockSize;
            int colB = j * blockSize;

            unsafe
            {
                half* blockTempA = null;
                half* blockTempB = null;
                half* blockTempS = null;

                half* blockS = S + rowA + SM * colB + batchOffSetS;
                int strideS = SM;

                if (rowA + blockSize > SM || colB + blockSize > SN) // copy remainder of C into zero-padded block
                {
                    blockTempS = AllocBlockHalf(blockSize, blockSize);
                    strideS = blockSize;
                    blockS = blockTempS;
                }
                for (int y = 0; y < blockSize; y++)
                    for (int x = 0; x < blockSize; x++)
                        blockS[x + strideS * y] = (half)((colB + y) < BN ? C[colB + y] : 0.0f);

                for (int l = 0; l < AN; l += blockSize) // inner-loop
                {
                    half* blockA = A + rowA + AM * l + batchOffSetA;
                    half* blockB = B + l * BN + colB;
                    int strideA = AM;
                    int strideB = BN;

                    if (rowA + blockSize > AM || l + blockSize > AN) // copy remainder of A into zero-padded block
                    {
                        if (blockTempA == null)
                            blockTempA = AllocBlockHalf(blockSize, blockSize);
                        strideA = blockSize;

                        for (int y = 0; y < blockSize; y++)
                            for (int x = 0; x < blockSize; x++)
                                blockTempA[x + blockSize * y] = (half)(((rowA + x) < AM && (l + y < AN)) ? blockA[x + AM * y] : 0.0f);

                        blockA = blockTempA;
                    }

                    if (colB + blockSize > BN || l + blockSize > BM) // copy remainder of B into zero-padded block
                    {
                        if (blockTempB == null)
                            blockTempB = AllocBlockHalf(blockSize, blockSize);
                        strideB = blockSize;

                        for (int y = 0; y < blockSize; y++)
                            for (int x = 0; x < blockSize; x++)
                                blockTempB[x + blockSize * y] = (half)(((colB + x) < BN && (l + y < BM)) ? blockB[x + BN * y] : 0.0f);

                        blockB = blockTempB;
                    }

                    MultiplyBlockUnrollHx16(blockA, strideA, blockB, strideB, blockS, strideS);
                }

                if (blockS == blockTempS) // copy back
                {
                    for (int y = 0; y < blockSize; y++)
                        for (int x = 0; x < blockSize; x++)
                        {
                            if (((rowA + x) < SM) && ((colB + y) < SN))
                                S[(rowA + x) + SM * (colB + y) + batchOffSetS] = blockTempS[x + blockSize * y];
                        }
                }

                FreeBlock(blockTempA);
                FreeBlock(blockTempB);
                FreeBlock(blockTempS);
            }
        }

        static void MultiplyBlockUnrollHx16(half* Ap, int Astride, half* Bp, int Bstride, half* Sp, int Sstride)
        {
            for (int i = 0; i < blockSize; i++)
            {
                float sum0 = *(Sp + i + Sstride * 0);
                float sum1 = *(Sp + i + Sstride * 1);
                float sum2 = *(Sp + i + Sstride * 2);
                float sum3 = *(Sp + i + Sstride * 3);
                float sum4 = *(Sp + i + Sstride * 4);
                float sum5 = *(Sp + i + Sstride * 5);
                float sum6 = *(Sp + i + Sstride * 6);
                float sum7 = *(Sp + i + Sstride * 7);
                float sum8 = *(Sp + i + Sstride * 8);
                float sum9 = *(Sp + i + Sstride * 9);
                float sumA = *(Sp + i + Sstride * 10);
                float sumB = *(Sp + i + Sstride * 11);
                float sumC = *(Sp + i + Sstride * 12);
                float sumD = *(Sp + i + Sstride * 13);
                float sumE = *(Sp + i + Sstride * 14);
                float sumF = *(Sp + i + Sstride * 15);

                for (int l = 0; l < blockSize; l++)
                {
                    float A = *(Ap + i + Astride * l);

                    float B0 = *(Bp + l * Bstride + 0);
                    float B1 = *(Bp + l * Bstride + 1);
                    float B2 = *(Bp + l * Bstride + 2);
                    float B3 = *(Bp + l * Bstride + 3);
                    float B4 = *(Bp + l * Bstride + 4);
                    float B5 = *(Bp + l * Bstride + 5);
                    float B6 = *(Bp + l * Bstride + 6);
                    float B7 = *(Bp + l * Bstride + 7);
                    float B8 = *(Bp + l * Bstride + 8);
                    float B9 = *(Bp + l * Bstride + 9);
                    float BA = *(Bp + l * Bstride + 10);
                    float BB = *(Bp + l * Bstride + 11);
                    float BC = *(Bp + l * Bstride + 12);
                    float BD = *(Bp + l * Bstride + 13);
                    float BE = *(Bp + l * Bstride + 14);
                    float BF = *(Bp + l * Bstride + 15);


                    sum0 += A * B0;
                    sum1 += A * B1;
                    sum2 += A * B2;
                    sum3 += A * B3;
                    sum4 += A * B4;
                    sum5 += A * B5;
                    sum6 += A * B6;
                    sum7 += A * B7;
                    sum8 += A * B8;
                    sum9 += A * B9;
                    sumA += A * BA;
                    sumB += A * BB;
                    sumC += A * BC;
                    sumD += A * BD;
                    sumE += A * BE;
                    sumF += A * BF;
                }

                *(Sp + i + Sstride * 0 ) = (half)(sum0);
                *(Sp + i + Sstride * 1 ) = (half)(sum1);
                *(Sp + i + Sstride * 2 ) = (half)(sum2);
                *(Sp + i + Sstride * 3 ) = (half)(sum3);
                *(Sp + i + Sstride * 4 ) = (half)(sum4);
                *(Sp + i + Sstride * 5 ) = (half)(sum5);
                *(Sp + i + Sstride * 6 ) = (half)(sum6);
                *(Sp + i + Sstride * 7 ) = (half)(sum7);
                *(Sp + i + Sstride * 8 ) = (half)(sum8);
                *(Sp + i + Sstride * 9 ) = (half)(sum9);
                *(Sp + i + Sstride * 10) = (half)(sumA);
                *(Sp + i + Sstride * 11) = (half)(sumB);
                *(Sp + i + Sstride * 12) = (half)(sumC);
                *(Sp + i + Sstride * 13) = (half)(sumD);
                *(Sp + i + Sstride * 14) = (half)(sumE);
                *(Sp + i + Sstride * 15) = (half)(sumF);
            }
        }
    }

    #endregion
}
}
