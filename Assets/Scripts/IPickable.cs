using UnityEngine;

public interface IPickable
{
    GameObject OnPick(Transform player, Transform holdPoint);
}