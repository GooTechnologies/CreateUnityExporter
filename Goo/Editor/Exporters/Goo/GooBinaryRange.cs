using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class GooBinaryRange {
	public int Start { get; private set; }
	public int Length { get; private set; }
	
	private int _position;
	private GooBinary _binary;
	private BinaryWriter _writer;
	
	public GooBinaryRange(GooBinary pBinary, BinaryWriter pUnderlyingWriter, int pStart, int pLength) {
		_binary = pBinary;
		_writer = pUnderlyingWriter;
		Start = pStart;
		Length = pLength;
	}
	
	public void Seek(int pos) {
		_position = pos;
	}
	
	private void SyncWritePosition() {
		_writer.Seek(Start + _position, SeekOrigin.Begin);
	}
	
	public void Write(byte v) {
		SyncWritePosition();
		_position += sizeof(byte);
		_writer.Write(v);
		_binary.MarkDirty();
	}
	
	public void Write(ushort v) {
		SyncWritePosition();
		_position += sizeof(ushort);
		_writer.Write(v);
		_binary.MarkDirty();
	}
	
	public void Write(float v) {
		SyncWritePosition();
		_position += sizeof(float);
		_writer.Write(v);
		_binary.MarkDirty();
	}
	
	public void Write(Vector3 v) {
		Write(v.x);
		Write(v.y);
		Write(v.z);
	}
	
	public void Write(Vector2 v) {
		Write(v.x);
		Write(v.y);
	}
	
	public void Write(byte[] v) {
		SyncWritePosition();
		_position += sizeof(byte) * v.Length;
		_writer.Write(v);
		_binary.MarkDirty();
	}
	
	public void Write(ushort[] v) {
		SyncWritePosition();
		_position += sizeof(ushort) * v.Length;
		for (int i = 0; i < v.Length; ++i) {
			_writer.Write(v[i]);
		}
		_binary.MarkDirty();
	}
	
	public void Write(float[] v) {
		SyncWritePosition();
		_position += sizeof(float) * v.Length;
		for (int i = 0; i < v.Length; ++i) {
			_writer.Write(v[i]);
		}
		_binary.MarkDirty();
	}
}