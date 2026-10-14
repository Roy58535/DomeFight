using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform players;
    [SerializeField] private int playerCount;
    [SerializeField] private ColorPalette colorPalette;

    private void Start()
    {
        for (int i = 0; i < playerCount; i++)
        {
            SpawnPlayer(i);
        }
    }

    private void SpawnPlayer(int playerIndex)
    {
        GameObject player = Instantiate(playerPrefab, players);
        player.name = $"Player {playerIndex + 1}";

        Vector3 sphericalCoord = new Vector3(ObjectOnSphere.Radius, 0 + playerIndex * 1f, 0.5f);

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.SetSphericalPosition(sphericalCoord);
        Material playerBodyMaterial = player.GetComponentInChildren<Renderer>().material;
        playerBodyMaterial.color = colorPalette.GetColor(playerIndex, Color.white);
    }
}
