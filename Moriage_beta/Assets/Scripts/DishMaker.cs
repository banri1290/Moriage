using System.Collections.Generic;
using UnityEngine;

public class DishMaker : GameSystem
{
    class Ingredient
    {
        public int id;
        public int[] cookingMethods;

        public Ingredient(int _id, int[] _cookingMethods)
        {
            id = _id;
            cookingMethods = _cookingMethods;
        }
    }

    class Dish
    {
        public Ingredient[] ingredients;
        private int currentIngredientIndex = 0;
        public bool isComplete { get; private set; }

        public Dish(int ingredientLength)
        {
            ingredients = new Ingredient[ingredientLength];
            currentIngredientIndex = 0;
            isComplete = false;
        }

        public void AddIngredient(Ingredient ingredient)
        {
            ingredients[currentIngredientIndex] = ingredient;
            currentIngredientIndex++;
            if (currentIngredientIndex >= ingredients.Length)
            {
                isComplete = true;
            }
        }
    }

    private Dish newDish = null;
    private List<Dish> completedDishes = new();

    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;
        return settingsAreCorrect;
    }

    public void StartMakingDish(int ingredientLength)
    {
        newDish = new Dish(ingredientLength);
    }

    public void AddIngredientToDish(int ingredientId, int[] cookingMethods)
    {
        if (newDish != null)
        {
            Ingredient ingredient = new(ingredientId, cookingMethods);
            newDish.AddIngredient(ingredient);
            Debug.Log($"Added ingredient {ingredientId} with methods [{string.Join(", ", cookingMethods)}] to the dish.");
            if (newDish.isComplete)
            {
                completedDishes.Add(newDish);
                newDish = null;
            }
        }
        else
        {
            Debug.LogWarning("No dish is being made currently.");
        }
    }
}
