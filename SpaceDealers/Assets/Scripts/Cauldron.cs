using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public List<StockInfo.StockType> ingredients = new List<StockInfo.StockType>();
        public GameObject resultPrefab;
    }

    // List of recipes configurable from the Inspector
    public List<Recipe> recipes = new List<Recipe>();

    [Header("Result spawn motion")]
    public float resultRiseHeight = 1.5f;
    public float resultRiseSpeed = 2f;

    // Track the actual StockObject instances inside the cauldron so we can destroy them only when a recipe matches
    private List<StockObject> objectsInside = new List<StockObject>();

    private void Awake()
    {
        // Provide a default recipe if none configured: fruit + cereal -> Water Bottle
        if (recipes == null || recipes.Count == 0)
        {
            GameObject defaultPrefab = Resources.Load<GameObject>("Water Bottle");
            if (defaultPrefab == null)
            {
                Debug.LogWarning("Cauldron: default prefab 'Water Bottle' not found in Resources. Assign recipe prefabs in the Inspector.");
            }

            recipes = new List<Recipe>()
            {
                new Recipe
                {
                    ingredients = new List<StockInfo.StockType>
                    {
                        StockInfo.StockType.fruit,
                        StockInfo.StockType.cereal
                    },
                    resultPrefab = defaultPrefab
                }
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // try to get the StockObject on the collider or one of its parents
        StockObject stockObj = other.GetComponentInParent<StockObject>();
        if (stockObj == null) return;

        objectsInside.Add(stockObj);

        CheckRecipe();
    }

    private void CheckRecipe()
    {
        // build a multiset of types currently inside
        List<StockInfo.StockType> typesInside = objectsInside.Select(o => o.info.typeOfStock).ToList();

        foreach (var recipe in recipes)
        {
            if (RecipeMatches(typesInside, recipe.ingredients))
            {
                // find the actual objects that fulfill the recipe (respecting multiplicity)
                List<StockObject> matched = new List<StockObject>();
                List<StockObject> tempObjects = new List<StockObject>(objectsInside);

                foreach (var ingredient in recipe.ingredients)
                {
                    int idx = tempObjects.FindIndex(o => o.info.typeOfStock == ingredient);
                    if (idx >= 0)
                    {
                        matched.Add(tempObjects[idx]);
                        tempObjects.RemoveAt(idx);
                    }
                }

                // Remove matched objects from the cauldron list and destroy their gameobjects
                foreach (var m in matched)
                {
                    objectsInside.Remove(m);
                    if (m != null && m.gameObject != null)
                        Destroy(m.gameObject);
                }

                SpawnResult(recipe.resultPrefab);

                // After producing one result, stop checking other recipes for this update
                break;
            }
        }
    }

    private bool RecipeMatches(List<StockInfo.StockType> available, List<StockInfo.StockType> required)
    {
        // simple multiset containment: try removing each required from a temp copy of available
        List<StockInfo.StockType> temp = new List<StockInfo.StockType>(available);
        foreach (var r in required)
        {
            if (temp.Contains(r)) temp.Remove(r);
            else return false;
        }
        return true;
    }

    private void SpawnResult(GameObject resultPrefab)
    {
        GameObject prefabToSpawn = resultPrefab;

        if (prefabToSpawn == null)
        {
            // nothing assigned in the inspector for this recipe; nothing to spawn
            Debug.LogWarning("Cauldron: recipe has no result prefab assigned.");
            return;
        }

        GameObject spawned = Instantiate(prefabToSpawn, transform.position + Vector3.up * 0.1f, Quaternion.identity);

        // add or configure SpawnRise component so the spawned object rises to the desired height and stops
        SpawnRise rise = spawned.GetComponent<SpawnRise>();
        if (rise == null)
        {
            rise = spawned.AddComponent<SpawnRise>();
        }

        rise.riseHeight = resultRiseHeight;
        rise.riseSpeed = resultRiseSpeed;

    }
}
