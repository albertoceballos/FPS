using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    [SerializeField]
    RectTransform thrusterFuelFill;

    private PlayerController controller;

    public void SetController(PlayerController cont)
    {
        controller = cont;
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }
}
