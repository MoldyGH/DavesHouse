using UnityEngine;

[System.Serializable]
public class GenericChoice<T> {
	public T choice = default(T);
	public int weight = 1;	// [1-100]
}
