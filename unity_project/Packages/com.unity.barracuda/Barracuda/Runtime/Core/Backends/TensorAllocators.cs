using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // ToList()

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace Unity.Barracuda {


/// <summary>
/// Caching `Tensor` allocator
/// </summary>
public class TensorCachingAllocator : UniqueResourceId, ITensorAllocator, IAllocatorStatistics
{
    public string name { get; set; }

    struct Entry : ITensorDataStatistics
    {
        public int size;
        public ITensorData tensorData;
        public bool free;

        //ITensorDataStatistics
        public int maxCapacity => tensorData.maxCapacity;
#if ENABLE_BARRACUDA_STATS
        public int uniqueId => tensorData.uniqueId;
        public bool inUse => !free;
        public bool isGPUMem => tensorData.isGPUMem;
#endif //ENABLE_BARRACUDA_STATS
    }
    // Sorted by size array of ITensorData
    private List<Entry> m_AllocatedBuffers = new List<Entry>();
    private Dictionary<Tensor, ITensorData> m_BusyTensors = new Dictionary<Tensor, ITensorData>();
    private Dictionary<ITensorData, int> m_SharedBuffers = new Dictionary<ITensorData, int>();

    private Action<ITensorData> disposeAllocatedBufferDelegate;
    private Action<ITensorData> adoptFreeBufferDelegate;

    // Stores only hollow tensor objects, tensor data is stored by m_AllocatedBuffers
    private List<Tensor> m_AllocatedTensors = new List<Tensor>();
    private int m_NumAllocatedBufferSinceCleanup = 0;

    /// <summary>
    /// Create `TensorCachingAllocator`
    /// </summary>
    public TensorCachingAllocator()
    {
        name = "Caching Allocator";
        disposeAllocatedBufferDelegate = DisposeAllocatedBuffer;
        adoptFreeBufferDelegate = AdoptFreeBuffer;
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~TensorCachingAllocator()
    {
        Dispose();
    }

    internal Tensor AllocTensorInternal(TensorShape shape, ITensorData buffer = null)
    {
        Tensor res = null;

        lock (m_AllocatedTensors)
        {
            if (m_AllocatedTensors.Count > 0)
            {
                res = m_AllocatedTensors.Last();
                res.Init(shape, buffer, this);
                m_AllocatedTensors.RemoveAt(m_AllocatedTensors.Count - 1);
            }
            else
            {
                res = new Tensor(shape, buffer, this);
            }
        }

        return res;
    }

    internal void AddRef(ITensorData buffer)
    {
        if (buffer == null)
            return;

        var sharedBufferCount = 0;
        m_SharedBuffers.TryGetValue(buffer, out sharedBufferCount);
        m_SharedBuffers[buffer] = sharedBufferCount + 1;
    }

    internal void DecRef(ITensorData buffer, Action<ITensorData> onLastRef = null)
    {
        if (buffer == null)
            return;

        Assert.IsTrue(m_SharedBuffers.ContainsKey(buffer));
        Assert.IsTrue(m_SharedBuffers[buffer] > 0);
        if (--m_SharedBuffers[buffer] > 0)
            return;

        m_SharedBuffers.Remove(buffer);

        if (onLastRef != null)
            onLastRef(buffer);
    }

    internal void AdoptFreeBuffer(ITensorData buffer)
    {
        // insert into the sorted array
        var size = buffer.maxCapacity;
        var newEntry = new Entry { size = size, tensorData = buffer, free = true };
        bool found = false;
        for (int i = 0; !found && i < m_AllocatedBuffers.Count; ++i)
        {
            var entry = m_AllocatedBuffers[i];
            if (buffer == entry.tensorData)
            {
                Assert.IsTrue(!entry.free);
                entry.free = true;
                m_AllocatedBuffers[i] = entry;
                Assert.IsTrue(m_AllocatedBuffers[i].free);
                found = true;
            }
            if (size < entry.size)
            {
                m_AllocatedBuffers.Insert(i, newEntry);
                Assert.IsTrue(m_AllocatedBuffers[i].size < m_AllocatedBuffers[i + 1].size);
                found = true;
            }
        }

        if (!found)
            m_AllocatedBuffers.Add(newEntry);
    }

    internal void DisposeAllocatedBuffer(ITensorData buffer)
    {
        for (int i = m_AllocatedBuffers.Count - 1; i >= 0; i--)
            if (m_AllocatedBuffers[i].tensorData == buffer)
                m_AllocatedBuffers.RemoveAt(i);
        buffer.Dispose();
    }

    /// <inheritdoc/>
    public virtual Tensor Alloc(TensorShape shape, AllocScope scope)
    {
        Profiler.BeginSample("Barracuda.SizeAllocator.Alloc");
        var name = "untitled";

        for (int i = 0; i < m_AllocatedBuffers.Count; ++i)
        {
            var entry = m_AllocatedBuffers[i];
            if (entry.size >= shape.length && entry.free)
            {
                entry.free = false;
                m_AllocatedBuffers[i] = entry;

                ITensorData buffer = entry.tensorData;
                buffer?.Reserve(shape.length);

                var tensor = AllocTensorInternal(shape, buffer);
                tensor.name = name;

                m_BusyTensors.Add(tensor, tensor.tensorOnDevice);
                AddRef(tensor.tensorOnDevice);

                Profiler.EndSample();
                return tensor;
            }
        }

        ++m_NumAllocatedBufferSinceCleanup;

        var newTensor = AllocTensorInternal(shape);
        newTensor.name = name;
        m_BusyTensors.Add(newTensor, newTensor.tensorOnDevice);
        AddRef(newTensor.tensorOnDevice);

        Profiler.EndSample();
        return newTensor;
    }

    /// <inheritdoc/>
    public virtual Tensor Alloc(TensorShape shape, ITensorData buffer, AllocScope scope)
    {
        Profiler.BeginSample("Barracuda.SizeAllocator.Alloc");
        var name = "untitled";

        var tensor = AllocTensorInternal(shape, buffer);
        tensor.name = name;
        m_BusyTensors.Add(tensor, tensor.tensorOnDevice);
        AddRef(tensor.tensorOnDevice);

        Profiler.EndSample();
        return tensor;
    }

    /// <inheritdoc/>
    public virtual void PostLayerCleanup()
    {
        //This allocator does not have support for allocation scope,
        //all tensors live until Reset() is called.

        //however allocation of new buffer are tracked for debug warning purpose
        //reset here to help catch context of those allocation (potential leaks)
        m_NumAllocatedBufferSinceCleanup = 0;
    }

    /// <inheritdoc/>
    public virtual void Release(Tensor tensor, bool calledFromTensorDispose)
    {
        Profiler.BeginSample("Barracuda.SizeAllocator.Release");
        Assert.AreEqual(tensor.allocator, this);

        var detachedBuffer = tensor.Invalidate(); // calls MoveToDevice(newBuffer=null,disposeDetachedBufferHint=false)

        if (calledFromTensorDispose)
        {
            lock (m_AllocatedTensors)
            {
                m_AllocatedTensors.Add(tensor);
                tensor.name = "";
            }
        }

        if (!m_BusyTensors.ContainsKey(tensor))
        {
            if (detachedBuffer == null)
                return;

            foreach (var entry in m_AllocatedBuffers)
                if (entry.tensorData == detachedBuffer && entry.free)
                    return;

            // some operations can create new Tensor and reassign ITensorData to it
            foreach (var busyEntry in m_BusyTensors)
                if (busyEntry.Value == detachedBuffer)
                    return; // we have original ITensorData in m_BusyTensors, nothing to realease
        }

        Assert.IsTrue(m_BusyTensors.ContainsKey(tensor));
        m_BusyTensors.Remove(tensor);


        Profiler.EndSample();
    }

    /// <inheritdoc/>
    public virtual void MoveToDevice(Tensor tensor, ITensorData newBuffer, ITensorData oldBuffer, bool disposeDetachedBufferHint)
    {
        if (newBuffer == oldBuffer)
            return;

        Assert.AreEqual(tensor.allocator, this);
        Assert.IsTrue(m_BusyTensors.ContainsKey(tensor));
        m_BusyTensors[tensor] = newBuffer;

        AddRef(newBuffer);

        if (disposeDetachedBufferHint)
            DecRef(oldBuffer, disposeAllocatedBufferDelegate);
        else
            DecRef(oldBuffer, adoptFreeBufferDelegate);
    }

    /// <inheritdoc/>
    public virtual void Reset(bool keepCachedMemory)
    {
        Profiler.BeginSample("Barracuda.SizeAllocator.Reset");

        if (!keepCachedMemory)
            Dispose();

        foreach(var tensor in m_BusyTensors.Keys.ToList())
            Release(tensor, false);

        Assert.AreEqual(m_BusyTensors.Count, 0);
        Assert.AreEqual(m_SharedBuffers.Count, 0);

        foreach(var buf in m_AllocatedBuffers)
            Assert.IsTrue(buf.free);

        Profiler.EndSample();
    }

    /// <inheritdoc/>
    public virtual void WaiveOwnership(Tensor tensor)
    {
        Assert.AreEqual(tensor.allocator, this);
        Assert.IsTrue(m_BusyTensors.ContainsKey(tensor));
        m_BusyTensors.Remove(tensor);

        var buffer = tensor.tensorOnDevice;
        if (buffer == null)
            return;

        Profiler.BeginSample("Barracuda.SizeAllocator.WaiveOwnership");

        int sharedCount = 0;
        m_SharedBuffers.TryGetValue(buffer, out sharedCount);
        if (sharedCount > 1)
        {
            var patchBusyTensors = new List<Tensor>();
            foreach (var busyEntry in m_BusyTensors)
                if (busyEntry.Value == buffer)
                    patchBusyTensors.Add(busyEntry.Key);

            Assert.AreEqual(sharedCount - 1, patchBusyTensors.Count);

            foreach (var busyTensor in patchBusyTensors)
            {
                Assert.AreEqual(m_BusyTensors[busyTensor], buffer);

                var oldBuffer = busyTensor.DetachFromDevice(false);
                var newBuffer = busyTensor.tensorOnDevice;
                Assert.IsTrue(oldBuffer == buffer);
                Assert.IsTrue(newBuffer != buffer);
                m_BusyTensors[busyTensor] = newBuffer;
                AddRef(newBuffer);
            }
        }

        // Assert no references to tensor are left owned by allocator
        Assert.IsTrue(m_SharedBuffers[buffer] == 1);
        m_SharedBuffers.Remove(buffer);

        int countInAllocatedBuffers = 0;
        for (int i = 0; i < m_AllocatedBuffers.Count; i++)
        {
            Entry entry = m_AllocatedBuffers[i];
            if (entry.tensorData == buffer)
            {
                Assert.IsFalse(entry.free);
                m_AllocatedBuffers.RemoveAt(i);
                countInAllocatedBuffers++;
            }
        }
        // This entry should have only been in the allocated buffers once at most
        Assert.IsTrue(countInAllocatedBuffers <= 1);

        foreach(var busyEntry in m_BusyTensors)
        {
            Assert.IsTrue(busyEntry.Key != tensor);
            Assert.IsTrue(busyEntry.Value != buffer);
        }

        Profiler.EndSample();
    }

    /// <summary>
    /// Dispose all allocated buffers
    /// </summary>
    public virtual void Dispose()
    {
        foreach(var tensor in m_BusyTensors.Keys.ToList())
            Release(tensor, false);
        foreach (var entry in m_AllocatedBuffers)
            entry.tensorData?.Dispose();

        m_BusyTensors.Clear();
        m_AllocatedBuffers.Clear();
        m_AllocatedTensors.Clear();
        m_SharedBuffers.Clear();
    }

    /// <summary>
    /// Return the number of buffer allocated since last call to LastLayerCleanup()
    /// </summary>
    internal int NumAllocatedBufferSinceCleanup
    {
        get { return m_NumAllocatedBufferSinceCleanup; }
    }

    /// <summary>
    /// Return true if the allocator is ready to be asked for a new ping pong buffer
    /// </summary>
    internal bool IsPingPongReady
    {
        get { return NumAllocatedBuffer == 2 && NumFreeBuffer >= 1; }
    }

    private int NumAllocatedBuffer
    {
        get { return m_AllocatedBuffers.Count; }
    }

    private int NumFreeBuffer
    {
        get { return m_AllocatedBuffers.Count(e => e.free); }
    }

#if ENABLE_BARRACUDA_STATS
    /// <inheritdoc/>
    public long usedBytes
    { get {
        long bytes = 0;

        Dictionary<int, int> usedSizePerTensorDataId = new Dictionary<int, int>();
        foreach (var tensorAnDataPair in m_BusyTensors)
        {
            var tensor = tensorAnDataPair.Key;
            var tensorData = tensorAnDataPair.Value;
            Assert.IsTrue(tensor.shape.length <= tensorData.maxCapacity);
            if (usedSizePerTensorDataId.ContainsKey(tensorData.uniqueId))
                Assert.AreEqual(usedSizePerTensorDataId[tensorData.uniqueId], tensor.shape.length);
            else
                usedSizePerTensorDataId[tensorData.uniqueId] = tensor.shape.length;
        }

        foreach (var usedSizeForTensorData in usedSizePerTensorDataId.Values)
        {
            bytes += usedSizeForTensorData  * sizeof(float);
        }

        return bytes;
    } }

    /// <inheritdoc/>
    public long busyBytes
    { get {
        long bytes = 0;
        //Dictionary to account for shallow copies of Tensors.
        Dictionary<int, ITensorData> tensorDatas = new Dictionary<int, ITensorData>();
        foreach (var tensor in m_BusyTensors.Keys)
        {
            if (tensor.tensorOnDevice != null)
                tensorDatas[tensor.tensorOnDevice.uniqueId] = tensor.tensorOnDevice;
        }
        foreach (var tensorData in tensorDatas)
            bytes += tensorData.Value.maxCapacity * sizeof(float);

        return bytes;
    } }

    /// <inheritdoc/>
    public long freeBytes
    { get {
        long bytes = 0;
        foreach(var entry in m_AllocatedBuffers)
            if (entry.free)
                bytes += entry.size * sizeof(float);
        return bytes;
    } }

    /// <inheritdoc/>
    public long totalBytes
    { get {
        return busyBytes + freeBytes;
    } }

    /// <inheritdoc/>
    public IEnumerable<ITensorStatistics> GetTensorsStatistics()
    {
        foreach (var busyTensor in m_BusyTensors)
        {
            yield return busyTensor.Key;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ITensorDataStatistics> GetTensorDatasStatistics()
    {
        Dictionary<int, ITensorDataStatistics> tensorDataStats = new Dictionary<int, ITensorDataStatistics>();
        foreach (var allocatedBuffer in m_AllocatedBuffers)
        {
            tensorDataStats[allocatedBuffer.uniqueId] = allocatedBuffer;
        }
        foreach (var sharedBuffer in m_SharedBuffers)
        {
            tensorDataStats[sharedBuffer.Key.uniqueId] = sharedBuffer.Key;
        }
        return tensorDataStats.Values;
    }

    /// <summary>
    /// Summary
    /// </summary>
    /// <returns>summary</returns>
    public override string ToString()
    {
        return "Total allocated: " + totalBytes + " busy: " + busyBytes;
    }
#endif //ENABLE_BARRACUDA_STATS
}

} // namespace Unity.Barracuda
