using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    private PlayerController controller;

    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    void Update()
    {
        SetFuelAmount(controller.getThrusterFuelAmount());
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }
}
