using UnityEngine;
using System.Collections.Generic;

public class HideBox : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public Transform player;
    public GameObject playerModel;

    private ThirdPersonController controller;
    private bool isPlayerHidden = false;
    private Vector3 originalPlayerPosition;

    [Header("Interaction Settings")]
    public float interactionDistance = 2f;

    // Static list to keep track of all hide boxes
    private static List<HideBox> allHideBoxes = new List<HideBox>();

    void OnEnable() => allHideBoxes.Add(this);
    void OnDisable() => allHideBoxes.Remove(this);

    void Start()
    {
        if (player != null)
            controller = player.GetComponent<ThirdPersonController>();

        if (playerModel == null && player != null)
            playerModel = player.GetComponentInChildren<SkinnedMeshRenderer>()?.gameObject;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && Input.GetKeyDown(interactKey))
        {
            ToggleHide();
        }
    }

    private void ToggleHide()
    {
        isPlayerHidden = !isPlayerHidden;

        if (isPlayerHidden)
        {
            originalPlayerPosition = player.position;
            player.position = transform.position;

            if (playerModel != null) playerModel.SetActive(false);
            if (controller != null) controller.enabled = false;

            foreach (Collider col in player.GetComponentsInChildren<Collider>())
                col.enabled = false;

            Debug.Log("Player is hidden in the box!");
        }
        else
        {
            player.position = originalPlayerPosition;

            if (playerModel != null) playerModel.SetActive(true);
            if (controller != null) controller.enabled = true;

            foreach (Collider col in player.GetComponentsInChildren<Collider>())
                col.enabled = true;

            Debug.Log("Player exited the box!");
        }
    }

    // Public getter
    public bool IsPlayerHidden => isPlayerHidden;

    // Static method to check if player is hidden in any hide box
    public static bool IsPlayerHiddenAnywhere()
    {
        foreach (var box in allHideBoxes)
            if (box.IsPlayerHidden) return true;
        return false;
    }
}
