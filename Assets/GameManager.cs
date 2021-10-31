using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.Instantiate("Player", SetPlayerPos(), Quaternion.identity);
        StartCoroutine(instantiateCubes());
    }

    IEnumerator instantiateCubes()
    {
        if (PhotonNetwork.IsMasterClient) // ? как работает в корутине прекращение действий по типу return ?
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-15, 15), 10, Random.Range(-15, 15));
                Quaternion quat = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
                PhotonNetwork.Instantiate("Cube", pos, quat);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public static Vector3 SetPlayerPos() => new Vector3(Random.Range(-15, 15), 10, Random.Range(-15, 15));
}
