using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rantest : MonoBehaviour {
	int[] nums1 = { 0, 1, 2, 2, 3, 0, 4, 2 };
	int[] nums2 = { 2, 5, 6 };
	void Awake() {
		print(RemoveElement(nums1, 2));
	}
	public int RemoveElement(int[] nums, int val) {
		int result = 0;
		foreach (int item in nums) {
			if (item != val) {
				nums[result] = item;
				result++;
			}
		}

		printResult(nums);
		return result;
	}
	void SwapElements(int[] array, int index1, int index2) {
		int temp = array[index2];
		array[index2] = array[index1];
		array[index1] = temp;
	}
	void printResult(int[] nu) {
		print("The results are");
		foreach (int k in nu) {
			print(k);
		}
		print("///////////////");
	}
}
