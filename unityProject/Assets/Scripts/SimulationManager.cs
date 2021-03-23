using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unclassified.Net;
using Unity.Simulation.UdpCluster;
using UnityEditor.Rendering;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    
    protected ClusterManager _cluster;
    
    protected ClusterNode[] _clusterNodes;
    private ClusterOptions _clusterOptions;
    public static readonly uint kClusterMessageRequestServerId = Utils.FourCC("DRRQ");
    public static readonly uint kClusterMessageAnnounceServerId = Utils.FourCC("DRAN");
    private AsyncTcpListener _listener;


    public void Initialize()
    {
        _listener = new AsyncTcpListener();
        StartClusterDiscovery();
        StartNetwork();
    }
    
    protected void StartClusterDiscovery()
    {
        _clusterOptions = new ClusterOptions()
        {
            expectedNodes = 1,
            identifyClusterTimeoutSec = 20,
            port = 58000
        };
        _cluster = new ClusterManager(_clusterOptions, state =>
            {
                switch (state)
                {
                    case ClusterState.Discovering:
                        Debug.Log("[DR] Cluster Discovery has begun.");
                        break;

                    case ClusterState.Ready:
                        // This call is expensive, so cache it.
                        // There is a delegate for when nodes change if you need to know this.
                        _clusterNodes = _cluster.Nodes;
                        Debug.Log("[DR] Cluster ready...");
                        foreach (var node in _clusterNodes)
                        {
                            Debug.Log($"[DR] Found node: {node.ToString()}");
                        }
                        break;

                    case ClusterState.TimedOut:
                        Debug.LogWarning("[DR] Cluster Discovery timed out.");
                        break;

                    case ClusterState.Disposed:
                        Debug.Log("[DR] Cluster has been disposed.");
                        break;
                }
            },
            false
        );
    }
    
    async void StartNetwork()
    {
        _cluster.RegisterHandler(kClusterMessageRequestServerId, true, ProcessUdpMessage);

        _listener = new AsyncTcpListener
        {
            IPAddress = IPAddress.IPv6Any,
            Port = _clusterOptions.port,
            ClientConnectedCallback = tcpClient =>
                new AsyncTcpClient
                {
                    ServerTcpClient = tcpClient,
                    ConnectedCallback = OnConnect,
                    //ReceivedCallback = OnReceivedDataFromClient,
                    ClosedCallback = OnDisconnect
                }.RunAsync(),
        };

        await _listener.RunAsync();
    }
    
    private Task OnConnect(AsyncTcpClient remoteClient, bool isReconnected)
    {
        // Debug.Log($" Client connected: {remoteClient.IPAddress}:{remoteClient.Port}, isReconnected: {isReconnected}");
        //
        // DisableTcpSocketDelay(remoteClient);
        //
        // var messageHandler = AddMessageHandler(remoteClient);
        // messageHandler.OnMessageReady += OnMessageReady;
        //
        // lock (_connectedClients)
        // {
        //     _connectedClients.Add(remoteClient);
        // }

        return Task.CompletedTask;
    }
    
    
    protected void DisableTcpSocketDelay(AsyncTcpClient client)
    {
        // Disable Nagle's algorithm to prevent messages from being batched.
        // This is only really necessary for preventing extremely small packets
        // from flooding the network.
        if (client.ClientSocket != null)
        {
            client.ClientSocket.NoDelay = true;
        }
    }

    private void OnDisconnect(AsyncTcpClient remoteClient, bool isRemote)
    {
        // Log.I($"[DR] Client disconnected: {remoteClient.IPAddress}:{remoteClient.Port}, isRemote: {isRemote}");
        //
        // lock (_connectedClients)
        // {
        //     _connectedClients.Remove(remoteClient);
        // }
        //
        // RemoveMessageHandler(remoteClient);
    }

    private void ProcessUdpMessage(Message message)
    {
        var node = GetClusterNodeFromMessage(message);

        if (message.messageId == kClusterMessageRequestServerId)
        {
            Debug.Log($"[UDP] RequestServerId from {node.address}");
            _cluster.SendMessage(node, kClusterMessageAnnounceServerId);
        }
        else
        {
            Debug.LogError($"[UDP] Unhandled message type: \"{Utils.FourCCToString(message.messageId)}\"");
        }
    }
    
    protected ClusterNode GetClusterNodeFromMessage(Message message)
    {
        var uniqueId = Utils.UniqueIdFromInstanceId(message.instanceId);
        var node = _cluster.GetNodeIfPresent(uniqueId);
        Debug.Assert(null != node, "[DR] Could not find destination node for this message");

        return node;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
