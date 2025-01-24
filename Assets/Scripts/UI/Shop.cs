using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public void OnClickNextRound()
    {
        GameManager.Instance.TryStartNextRound();
    }
}
