using UnityEngine;
using TMPro;

public class InteractionHintDistance : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI hintText; // Assign your TextMeshPro UI text here
    public float interactDistance = 2f; // Distance to show the hint

    [Header("Action Text")]
    public string hideText = "Press E to Hide";
    public string leaveText = "Press E to Leave";

    private bool isHiding = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hintText.text = "";
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            // Show hint depending on state
            hintText.text = isHiding ? leaveText : hideText;

            // Press E to interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isHiding)
                {
                    isHiding = true;
                    Debug.Log("Player is now hiding!");
                    // Add hide logic here
                }
                else
                {
                    isHiding = false;
                    Debug.Log("Player left hiding!");
                    // Add leave logic here
                }
            }
        }
        else
        {
            hintText.text = ""; // Hide hint if too far
        }
    }
}
