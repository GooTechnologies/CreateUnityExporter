using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class GooBinary : IDisposable {
	private const int ByteRangeAlignment = 4;
	
	private MemoryStream _binaryStream;
	private BinaryWriter _writer;
	private string _binaryKeystring;
	private bool _checksumDirty = true;
	private int _nextOffset;
	
	public GooBinary() {
		_nextOffset = 0;
		_binaryStream = new MemoryStream();
		_writer = new BinaryWriter(_binaryStream);
	}

	#region IDisposable implementation
	public void Dispose() {
		if (_writer != null) {
			((IDisposable)_writer).Dispose();
		}
		if (_binaryStream != null) {
			_binaryStream.Dispose();
		}
	}
	#endregion
	
	public void MarkDirty() {
		_checksumDirty = true;
	}
	
	public int Length {
		get {
			return (int)_binaryStream.Length;
		}
	}
	
	public GooBinaryRange AllocateRange(int pLength) {
		int alignedOffset = _nextOffset;
		if (ByteRangeAlignment > 0) {
			int misalignment = alignedOffset % ByteRangeAlignment;
			if (misalignment != 0) {
				alignedOffset += (ByteRangeAlignment - misalignment);
			}
		}
		
		var range = new GooBinaryRange(this, _writer, alignedOffset, pLength);
		_nextOffset = range.Start + range.Length;
		return range;
	}
	
	public GooBinaryRange Add(Vector3[] pArray) {
		var range = AllocateRange(pArray.Length * sizeof(float) * 3);
		for (int i = 0; i < pArray.Length; ++i) {
			range.Write(pArray[i]);
		}
		return range;
	}
	
	public GooBinaryRange Add(Vector2[] pArray) {
		var range = AllocateRange(pArray.Length * sizeof(float) * 2);
		for (int i = 0; i < pArray.Length; ++i) {
			range.Write(pArray[i]);
		}
		return range;
	}
	
	public GooBinaryRange Add(byte[] pArray) {
		var range = AllocateRange(pArray.Length * sizeof(byte));
		range.Write(pArray);
		return range;
	}
	
	public GooBinaryRange Add(ushort[] pArray) {
		var range = AllocateRange(pArray.Length * sizeof(ushort));
		range.Write(pArray);
		return range;
	}
	
	
	public GooBinaryRange Add(float[] pArray) {
		var range = AllocateRange(pArray.Length * sizeof(float));
		range.Write(pArray);
		return range;
	}
	
	public void SaveToDisk() {
		var outPath = Path.Combine(GLexConfig.BasePath, BinaryId);
		if (!File.Exists(outPath)) {
			// Because the filename is a hash of its contents, we know we don't need to actually write out the file if it already exists
			File.WriteAllBytes(outPath, _binaryStream.ToArray());
		}
	}
	
	public string BinaryId {
		get {
			return BinaryKeystring + GLexConfig.GetExtensionFor("binary");
		}
	}
	
	public string BinaryKeystring {
		get {
			if (_checksumDirty) {
				_binaryKeystring = NamesUtil.GenerateBinaryId(_binaryStream.GetBuffer(), 0, (int)_binaryStream.Length, GLex.Instance.UserName);
				_checksumDirty = false;
			}
			return _binaryKeystring;
		}
	}
}
