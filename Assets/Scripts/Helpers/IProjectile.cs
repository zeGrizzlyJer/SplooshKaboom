using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void Launch(Vector3 pos, Quaternion rot, float power);
}
