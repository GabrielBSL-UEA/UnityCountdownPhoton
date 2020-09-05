using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviour
{
    NavMeshAgent navMA;

    PhotonView phoView;

    private void Awake()
    {
        navMA = GetComponent<NavMeshAgent>();
        phoView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (phoView.IsMine)
            ClickToMove();
    }

    void ClickToMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, 1000))
            {
                navMA.destination = hit.point;
            }
        }
    }
}
