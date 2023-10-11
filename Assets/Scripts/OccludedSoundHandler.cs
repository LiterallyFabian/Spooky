using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPoint
{
    public Vector3 Position;
    public Vector3 Angle;
    public Vector3 ParentVector;
    public float BouncesLeft = 5;
}

public class OccludedSoundHandler : MonoBehaviour
{

    GameObject _player;
    LayerMask audioOcclusionLayerMask;
    float _soundMaxDist;
    List<SoundPoint> openList;

    private void Start()
    {
        _soundMaxDist = 100;
        _player = GameObject.FindGameObjectWithTag("Player");
        audioOcclusionLayerMask = ~LayerMask.GetMask("IgnoreAudioOcclusion");
        openList = new List<SoundPoint>();
        SoundPoint sp = new SoundPoint();
        sp.Position = transform.position;
        sp.Angle = Vector3.Normalize(new Vector3(1f,0,-1f));
        openList.Add(sp);

        FindPathToPlayer();
    }

    public void FindPathToPlayer()
    {
        SoundPoint sp = openList[0];
        RaycastHit hit;
        Physics.Raycast(sp.Position, sp.Angle, out hit, Mathf.Infinity, audioOcclusionLayerMask);
        Vector3 endPoint = Vector3.zero;
        if (hit.collider != null)
        {
            endPoint = hit.point;
            print("hit");
        }
        Debug.DrawRay(sp.Position, sp.Angle * Vector3.Distance(sp.Position, hit.point), Color.red, 99);
        sp.Position = endPoint;
        print($"Angle: {sp.Angle}");
        sp.Angle = new Vector3();  //<---FIXA DENNA
        print($"Angle: {sp.Angle}");
    }

    bool checkIfLOSPlayer(Vector3 startPos)
    {
        RaycastHit hit;
        Physics.Linecast(startPos, _player.transform.position, out hit, audioOcclusionLayerMask);
        if(hit.collider != null && hit.collider.tag == "Player")
        {
            return true;
        }
        return false;
    }
}
