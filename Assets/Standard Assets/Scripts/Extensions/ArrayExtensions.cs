using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassExtensions
{
	public static class ArrayExtensions 
	{
		public static List<T> ToList_class<T> (this T[] array) where T : class
		{
			List<T> output = new List<T>();
			output.AddRange(array);
			return output;
		}

		public static bool Contains_class<T> (this T[] array, T element) where T : class
		{
			foreach (T obj in array)
			{
				if (obj == element)
					return true;
			}
			return false;
		}

		public static int IndexOf_class<T> (this T[] array, T element) where T : class
		{
			for (int i = 0; i < array.Length; i ++)
			{
				if (array[i] == element)
					return i;
			}
			return -1;
		}

		public static T[] Add_class<T> (this T[] array, T element) where T : class
		{
			List<T> output = array.ToList_class();
			output.Add(element);
			return output.ToArray();
		}

		public static T[] Remove_class<T> (this T[] array, T element) where T : class
		{
			List<T> output = array.ToList_class();
			output.Remove(element);
			return output.ToArray();
		}

		public static T[] RemoveAt_class<T> (this T[] array, int index) where T : class
		{
			List<T> output = array.ToList_class();
			output.RemoveAt(index);
			return output.ToArray();
		}

		public static T[] AddRange_class<T> (this T[] array, IEnumerable<T> array2) where T : class
		{
			List<T> output = array.ToList_class();
			output.AddRange(array2);
			return output.ToArray();
		}

		public static T[] SwapIndicies_class<T> (this T[] array, int index1, int index2) where T : class
		{
			T temp = array[index1];
			array[index2] = temp;
			temp = array[index2];
			array[index1] = temp;
			return array;
		}

		public static List<T> ToList_struct<T> (this T[] array) where T : struct
		{
			List<T> output = new List<T>();
			output.AddRange(array);
			return output;
		}

		public static bool Contains_struct<T> (this T[] array, T element) where T : struct
		{
			foreach (T obj in array)
			{
				if (obj.Equals(element))
					return true;
			}
			return false;
		}

		public static int IndexOf_struct<T> (this T[] array, T element) where T : struct
		{
			for (int i = 0; i < array.Length; i ++)
			{
				if (array[i].Equals(element))
					return i;
			}
			return -1;
		}

		public static T[] Add_struct<T> (this T[] array, T element) where T : struct
		{
			List<T> output = array.ToList_struct();
			output.Add(element);
			return output.ToArray();
		}

		public static T[] Remove_struct<T> (this T[] array, T element) where T : struct
		{
			List<T> output = array.ToList_struct();
			output.Remove(element);
			return output.ToArray();
		}

		public static T[] RemoveAt_struct<T> (this T[] array, int index) where T : struct
		{
			List<T> output = array.ToList_struct();
			output.RemoveAt(index);
			return output.ToArray();
		}

		public static T[] AddRange_struct<T> (this T[] array, IEnumerable<T> array2) where T : struct
		{
			List<T> output = array.ToList_struct();
			output.AddRange(array2);
			return output.ToArray();
		}

		public static T[] SwapIndicies_struct<T> (this T[] array, int index1, int index2) where T : struct
		{
			T temp = array[index1];
			array[index2] = temp;
			temp = array[index2];
			array[index1] = temp;
			return array;
		}
	}
}