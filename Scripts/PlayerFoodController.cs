using UnityEngine;
using System.Collections.Generic;

public class PlayerFoodController : MonoBehaviour
{

public float testValue = 10f;
    [Header("Trash Bin")]
    public bool nearTrashBin = false;

    [Header("Food Catalog")]
    public FoodCatalog foodCatalog;
    public Transform headStackPoint;

    [Header("Stack Settings")]
    public float stackSpacing = 0.4f;
    public float stackScatter = 0.12f;
    public float stackTilt = 12f;
    public int maxFood = 8;

    [Header("Digging")]
    public float digInterval = 1f;

    [Header("Throwing")]
    public Transform cameraTransform;
    public float throwForce = 8f;
    public float throwUpwardForce = 2f;

    [Header("Animation")]
    public DirectionalSpriteAnimator animator;

    private List<GameObject> foodStack = new List<GameObject>();
    private float digTimer;

    public int FoodCount => foodStack.Count;

    private void Update()
    {
        HandleDigging();
        HandleThrowing();
    }

    private void HandleDigging()
    {
        if (!nearTrashBin || foodStack.Count >= maxFood)
        {
            digTimer = 0f;
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            digTimer += Time.deltaTime;

            if (digTimer >= digInterval)
            {
                digTimer = 0f;
                SpawnFood();
            }
        }
        else
        {
            digTimer = 0f;
        }
    }

    private void SpawnFood()
    {
        if (foodCatalog == null || headStackPoint == null)
            return;

        FoodData selectedFood = foodCatalog.GetRandomFood();

        if (selectedFood == null || selectedFood.foodPrefab == null)
            return;

        GameObject food = Instantiate(
            selectedFood.foodPrefab,
            headStackPoint.position,
            Quaternion.identity,
            headStackPoint
        );

        FoodItem foodItem = food.GetComponent<FoodItem>();

        if (foodItem == null)
        {
            Debug.LogError(
                selectedFood.foodName + " prefab is missing FoodItem!"
            );

            Destroy(food);
            return;
        }

        foodItem.Initialize(selectedFood);
        foodItem.SetStacked(true);

        foodStack.Add(food);

        //SCATTER THAT POSITION SHIT
        food.transform.localPosition = new Vector3(
            Random.Range(-stackScatter, stackScatter),
            (foodStack.Count - 1) * stackSpacing,
            Random.Range(-stackScatter, stackScatter)
        );

        //RANDOM ROTATION (IDLE)
        food.transform.localRotation = Quaternion.Euler(
            Random.Range(-stackTilt, stackTilt),
            Random.Range(0f, 360f),
            Random.Range(-stackTilt, stackTilt)
        );

        UpdateAnimation();
    }

    private void HandleThrowing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ThrowFood();
        }
    }

    private void ThrowFood()
    {
        if (foodStack.Count == 0)
            return;

        //REMOVE BOTTOM FOOD ITEM
        GameObject food = foodStack[0];
        foodStack.RemoveAt(0);

        FoodItem foodItem = food.GetComponent<FoodItem>();

        if (foodItem != null)
        {
            Vector3 direction = cameraTransform != null
                ? cameraTransform.forward
                : transform.forward;

            direction = (
                direction + Vector3.up * throwUpwardForce * 0.1f
            ).normalized;

            foodItem.Throw(direction, throwForce);
        }
        else
        {
            Destroy(food);
        }

        ReorganizeStack();
        UpdateAnimation();
    }

    private void ReorganizeStack()
    {
        for (int i = 0; i < foodStack.Count; i++)
        {
            if (foodStack[i] == null)
                continue;

            Transform food = foodStack[i].transform;

            Vector3 position = food.localPosition;
            position.y = i * stackSpacing;

            food.localPosition = position;
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetCarrying(foodStack.Count > 0);
        }
    }

    public void SetNearTrashBin(bool value)
    {
        nearTrashBin = value;

        if (!value)
            digTimer = 0f;
    }
}