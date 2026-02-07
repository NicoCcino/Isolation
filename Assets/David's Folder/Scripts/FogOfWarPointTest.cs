using UnityEngine;
using FOW;
using System.Linq;
public class FogOfWarPointTester : Singleton<FogOfWarPointTester>
{
    [SerializeField]
    private FogOfWarRevealer[] Revealers;
    public bool IsPlayerInTheLight(Vector3 playerPosition)
    {
        return Revealers.ToList().Any(r => r.TestPoint(playerPosition));
    }


}
