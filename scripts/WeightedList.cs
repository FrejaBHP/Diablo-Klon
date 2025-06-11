using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public readonly struct WeightedListItem<T> {
    internal readonly T item;
    internal readonly int weight;

    public WeightedListItem(T newItem, int newWeight) {
        item = newItem;
        weight = newWeight;
    }
}

public class WeightedList<T> : IEnumerable<T> {
    private readonly List<T> itemsList = new();
    private readonly List<int> weightsList = new();

    private readonly Random random;
    private int totalWeight;

    public WeightedList(Random rnd = null) {
        random = rnd ?? new Random();
    }

    public WeightedList(ICollection<WeightedListItem<T>> itemList, Random rnd = null) {
        random = rnd ?? new Random();
        Add(itemList);
    }

    public IEnumerator<T> GetEnumerator() {
        return itemsList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public bool Contains(T item) {
        return itemsList.Contains(item);
    }

    public int IndexOf(T item) {
        return itemsList.IndexOf(item);
    }

    public void Add(T newItem, int newWeight) {
        itemsList.Add(newItem);
        weightsList.Add(newWeight);
        UpdateList();
    }

    public void Add(WeightedListItem<T> listItem) {
        Add(listItem.item, listItem.weight);
    }

    public void Add(IEnumerable<WeightedListItem<T>> itemList) {
        foreach (WeightedListItem<T> listItem in itemList) {
            itemsList.Add(listItem.item);
            weightsList.Add(listItem.weight);
        }
        UpdateList();
    }

    public void Remove(T removedItem) {
        int index = IndexOf(removedItem);
        RemoveAt(index);
    }

    public void RemoveAt(int index) {
        itemsList.RemoveAt(index);
        weightsList.RemoveAt(index);
        UpdateList();
    }

    public int GetWeight(T item) {
        int index = itemsList.IndexOf(item);
        return GetWeightAt(index);
    }

    public int GetWeightAt(int index) {
        return weightsList[index];
    }

    public void SetWeight(T item, int newWeight) {
        SetWeightAt(IndexOf(item), newWeight);
    }

    public void SetWeightAt(int index, int newWeight) {
        weightsList[index] = newWeight;
        UpdateList();
    }

    public T GetRandomItem() {
        int randomNumber = random.Next(totalWeight);
        int iteratorFloor = 0;
        int iteratorCeil = 0;

        // If list is empty, return null
        if (itemsList.Count == 0) {
            return default;
        }

        for (int i = 0; i < itemsList.Count; i++) {
            if (i != 0) {
                iteratorFloor += weightsList[i - 1];
            }
            iteratorCeil += weightsList[i];

            // If a match, return the element
            if (randomNumber <= iteratorCeil && randomNumber > iteratorFloor) {
                return itemsList[i];
            }
        }

        // Should never happen, but if no item is picked, return first element in list or null
        return default;
    }

    private void UpdateList() {
        if (weightsList.Count == 0) {
            totalWeight = 0;
            return;
        }

        int newTotalWeight = 0;
        
        foreach (int weight in weightsList) {
            newTotalWeight += weight;
        }

        totalWeight = newTotalWeight;
    }
}
