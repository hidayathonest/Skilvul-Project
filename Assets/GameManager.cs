using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject grass;
    [SerializeField] GameObject road;
    [SerializeField] int extent = 7;
    [SerializeField] int frontDistance = 10;
    [SerializeField] int minZPos = -5;
    [SerializeField] int maxSameTerrainRepeat = 3;

    int maxZpos;
    Dictionary<int, TerrainBlock> map = new Dictionary<int, TerrainBlock>(50);
    TMP_Text gameOverText;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        gameOverText = gameOverPanel.GetComponentInChildren<TMP_Text>();

        //belakang
        for (int z = minZPos; z <= 0; z++)
        {
            CreateTerrain(grass, z);
        }

        //depan
        for (int z = 1; z <= frontDistance; z++)
        {
            var prefab = GetNextRandomTerrainPrefab(z);

            //instantiate blocker
            CreateTerrain(prefab, z);
        }

        player.SetUp(minZPos, extent);
    }    

    private int playerLastMaxTravel;
    private void Update()
    {
        if(player.IsDie && gameOverPanel.activeInHierarchy==false)
            StartCoroutine(ShowGameOverPanel());

        if(player.MaxTravel==playerLastMaxTravel)
        return;

        playerLastMaxTravel = player.MaxTravel;

        var randTbPrefab = GetNextRandomTerrainPrefab(player.MaxTravel+frontDistance);
        CreateTerrain(randTbPrefab, player.MaxTravel+frontDistance);

        // var lastTB = map[player.MaxTravel-1+minZPos];
        TerrainBlock lastTB = map[player.MaxTravel-1+minZPos];
        int lastPos = player.MaxTravel;
        foreach (var (pos,tb) in map)
        {
            if(pos<lastPos)
            {
                lastPos = pos;
                lastTB = tb;
            }
        }

        map.Remove(player.MaxTravel-1+minZPos);
        Destroy(lastTB.gameObject);

        player.SetUp(player.MaxTravel+minZPos, extent);
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(3);
        gameOverText.text = "Your Score: " + player.MaxTravel;
        gameOverPanel.SetActive(true);
    }

    private void CreateTerrain(GameObject prefab, int zPos)
    {
        var go = Instantiate(prefab, new Vector3(0, 0, zPos), Quaternion.identity);
        var tb = go.GetComponent<TerrainBlock>();
        tb.Build(extent);

        map.Add(zPos, tb);
    }

    private GameObject GetNextRandomTerrainPrefab(int nextPos)
    {
        bool isUniform = true;
        var tbRef = map[nextPos - 1];
        for (int distance = 2; distance <= maxSameTerrainRepeat; distance++)
        {
            if(map[nextPos - distance].GetType() !=tbRef.GetType())
            {
                isUniform = false;
                break;
            }
        }

        if (isUniform)
        {
            if(tbRef is Grass)
                return road;
            else
                return grass;
        }

        //penentuan terrain block dengan chance 50%
        return Random.value > 0.5f ? road : grass;
    }
}
