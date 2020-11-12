using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassExtensions
{
	public static class StringExtensions
	{
		public static string SubstringStartEnd (this string str, int startIndex, int endIndex)
		{
			return str.Substring(startIndex, endIndex - startIndex);
		}
		
		public static string RemoveEach (this string str, string remove)
		{
			return str.Replace(remove, "");
		}
		
		public static string StartAfter (this string str, string startAfter)
		{
			return str.Substring(str.IndexOf(startAfter) + startAfter.Length);
		}

		public static string RemoveStartEnd (this string str, int startIndex, int endIndex)
		{
			return str.Remove(startIndex, endIndex - startIndex);
		}

		public static string RemoveBetween (this string str, string startString, string endString, bool removeStartAndEnd)
		{
			int indexOfStartString = str.IndexOf(startString);
			int indexOfEndString = str.IndexOf(endString);
			string startOfStr;
			string endOfStr;
			if (removeStartAndEnd)
			{
				startOfStr = str.Substring(0, indexOfStartString);
				endOfStr = str.Substring(indexOfEndString + endString.Length);
			}
			else
			{
				startOfStr = str.Substring(0, indexOfStartString + startString.Length);
				endOfStr = str.Substring(indexOfEndString);
			}
			return startOfStr + endOfStr;
		}
	}
}