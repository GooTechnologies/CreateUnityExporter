using UnityEngine;
using System.Collections.Generic;
using System;

public class GooBinaryPointer {
	private int _start, _length;
	private string _typeString;
	
	public GooBinaryPointer(GooBinaryRange range, Type dataType) {
		int typeSize = 1;
		
		if (dataType == typeof(byte)) {
			typeSize = 1;
			_typeString = "uint8";
		}
		else if (dataType == typeof(ushort)) {
			typeSize = 2;
			_typeString = "uint16";
		}
		else if (dataType == typeof(uint)) {
			typeSize = 4;
			_typeString = "uint32";
		}
		else if (dataType == typeof(float)) {
			typeSize = 4;
			_typeString = "float32";
		}
		else {
			throw new ArgumentException("Unrecognized range type " + dataType.ToString());
		}
		
		if (range.Length % typeSize != 0) {
			throw new ArgumentException("Mesh data is not even multiple of element size");
		}
		
		_start = range.Start;
		_length = range.Length / typeSize;
	}
	
	public override string ToString() {
		return string.Format("[{0}, {1}, \"{2}\"]", _start, _length, _typeString);
	}
}
