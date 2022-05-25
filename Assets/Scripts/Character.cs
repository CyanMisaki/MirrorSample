using System;
using UnityEngine;
using Mirror;
using Mirror.Examples.AdditiveScenes;

[RequireComponent(typeof(CharacterController))]
public abstract class Character : NetworkBehaviour
{
    protected Action OnUpdateAction { get; set; }
    protected Action<NetworkConnection> OnShootAction { get; set; }
    protected abstract FireAction fireAction { get; set; }

    [SyncVar] protected Vector3 serverPosition;
    [SyncVar] protected Quaternion serverRotation;
    [SyncVar] protected int serverHealth=100;
    public NetworkConnection Connection { get; set; }
    
    protected virtual void Initiate()
    {
        OnUpdateAction += Movement;
        OnShootAction += Shoot;
    }

    private void Shoot(NetworkConnection obj)
    {
        TargetCalculateHealth(obj, 50);
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        OnUpdateAction?.Invoke();
    }

    [Command]
    protected void CmdUpdateTransform(Vector3 position, Quaternion rotation)
    {
        serverPosition = position;
        serverRotation = rotation;
    }
    
    [TargetRpc]
    protected void TargetCalculateHealth(NetworkConnection networkConnection, int hitRate)
    {
        if (serverHealth - hitRate <= 0)
        {
            serverHealth = 0;
            NetworkServer.connections[networkConnection.connectionId].Disconnect();
        }
        else serverHealth -= hitRate;




    }

    public abstract void Movement();
    
    private void OnDestroy()
    {
        OnUpdateAction -= Movement;
        OnShootAction -= Shoot;
    }
}
