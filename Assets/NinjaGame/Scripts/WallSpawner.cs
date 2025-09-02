using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WallOption
    {
        public GameObject wallPrefab;     // Префаб стены
        public List<WallChild> children;  // Дочерние объекты с шансом
    }

    [System.Serializable]
    public class WallChild
    {
        public GameObject childObject; // сам объект
        [Range(0f, 1f)] public float chance = 0.5f; // шанс включения
    }

    public Transform player;
    public Camera mainCamera;

    public float wallHeight = 10.43626f;
    public int poolSize = 6; // сколько стен держим в пуле
    public List<WallOption> wallOptions; // варианты стен (левая/правая)

    private Queue<GameObject> activeWalls = new Queue<GameObject>();
    private float lastSpawnY;

    private void Start()
    {
        // Спавним начальные стены
        for (int i = 0; i < poolSize; i++)
        {
            SpawnWall(i * wallHeight);
        }
    }

    private void Update()
    {
        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;

        // Если последняя стена ещё над игроком — спавним новую
        if (player.position.y + wallHeight > lastSpawnY)
        {
            SpawnWall(lastSpawnY + wallHeight);
        }

        // Проверяем стены — те, что ниже камеры, отключаем
        if (activeWalls.Count > 0)
        {
            GameObject firstWall = activeWalls.Peek();
            if (firstWall.transform.position.y + wallHeight < cameraBottom)
            {
                firstWall.SetActive(false);
                activeWalls.Dequeue();
            }
        }
    }

    private void SpawnWall(float yPos)
    {
        foreach (var wallOption in wallOptions)
        {
            // Берём из пула или создаём новую стену
            GameObject wall = GetFromPool(wallOption.wallPrefab);
            wall.transform.position = new Vector3(wallOption.wallPrefab.transform.position.x, yPos, 0f);
            wall.SetActive(true);

            // Обновляем дочерние объекты
            HandleChildren(wall, wallOption.children);

            activeWalls.Enqueue(wall);
        }

        lastSpawnY = yPos;
    }

    private void HandleChildren(GameObject wall, List<WallChild> children)
    {
        // сначала выключаем все
        foreach (var child in children)
        {
            if (child.childObject != null)
                child.childObject.SetActive(false);
        }

        // выбираем один по шансам
        foreach (var child in children)
        {
            if (Random.value <= child.chance)
            {
                child.childObject.SetActive(true);
                break; // включаем только один
            }
        }
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        // ищем неактивный
        foreach (var wall in activeWalls)
        {
            if (!wall.activeSelf && wall.name.Contains(prefab.name))
                return wall;
        }

        // создаём новый, если пула не хватило
        GameObject newWall = Instantiate(prefab);
        newWall.name = prefab.name; // чтобы легче находить
        return newWall;
    }
}
