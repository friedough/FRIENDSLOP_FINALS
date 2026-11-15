
using UnityEngine;

public class TrashBinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerFoodController player =
            other.GetComponentInParent<PlayerFoodController>();

        if (player != null)
        {
            player.SetNearTrashBin(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerFoodController player =
            other.GetComponentInParent<PlayerFoodController>();

        if (player != null)
        {
            player.SetNearTrashBin(false);
        }
    }
}

