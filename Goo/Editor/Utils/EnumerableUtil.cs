using UnityEngine;
using System.Collections.Generic;

public static class EnumerableUtil {
	public static List<KeyValuePair<int, TElement>> Indexify<TElement>(IEnumerable<TElement> pEnumerable) {
		var ret = new List<KeyValuePair<int, TElement>>();
		
		int i = 0;
		foreach (var element in pEnumerable) {
			ret.Add(new KeyValuePair<int, TElement>(i, element));
			i++;
		}
		
		return ret;
	}
}
