using DBBD;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class TestInfo : DBBD.ICell
{
	public TestInfo()
	{
		fingerPrinter.AddRange(Enumerable.Repeat(false, 1));
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		DBBD.Serizlie.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, a); }
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out a); }
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) {  totalLength += sizeof(int);  }
		return totalLength;
	}

	public int A { get { return a; } set { a = value; fingerPrinter[0] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private int a;
}

class TestInfo2 : DBBD.ICell
{
	public TestInfo2()
	{
		fingerPrinter.AddRange(Enumerable.Repeat(false, 1));
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		DBBD.Serizlie.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, b); }
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out b); }
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) { b.GetLength() }
		return totalLength;
	}

	public TestInfo B { get { return b; } set { b = value; fingerPrinter[0] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private TestInfo b;
}

class TestInfo3 : DBBD.ICell
{
	public TestInfo3()
	{
		fingerPrinter.AddRange(Enumerable.Repeat(false, 1));
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		DBBD.Serizlie.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, c); }
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out c); }
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) { sizeof(uint) + Encoding.UTF8.GetByteCount(c) }
		return totalLength;
	}

	public List<string> C { get { return c; } set { c = value; fingerPrinter[0] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private List<string> c;
}

class TestInfo4 : DBBD.ICell
{
	public TestInfo4()
	{
		fingerPrinter.AddRange(Enumerable.Repeat(false, 1));
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		DBBD.Serizlie.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, d); }
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out d); }
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) { d.GetLength() }
		return totalLength;
	}

	public List<TestInfo3> D { get { return d; } set { d = value; fingerPrinter[0] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private List<TestInfo3> d;
}

// Test1
class ServerConnectReq : DBBD.Request
{
	public ServerConnectReq()
	{
		typeId = (uint)Base.Value.ServerConnectReq;
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		base.WriteHeader(buffer, GetLength());
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		base.ReadHeader(buffer);
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(base.GetLength());
		return totalLength;
	}
}

// Test2
class ServerConnectResp : DBBD.Response
{
	public ServerConnectResp()
	{
		typeId = (uint)Base.Value.ServerConnectResp;
		fingerPrinter.AddRange(Enumerable.Repeat(false, 1));
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		base.WriteHeader(buffer, GetLength());
		DBBD.Serialize.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, sessionId); }
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		base.ReadHeader(buffer);
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out sessionId); }
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(base.GetLength());
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) {  totalLength += sizeof(int);  }
		return totalLength;
	}

	public int SessionId { get { return sessionId; } set { sessionId = value; fingerPrinter[0] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private int sessionId;
}

class RelayNoti : DBBD.Request
{
	public RelayNoti()
	{
		typeId = (uint)Relay.Value.RelayNoti;
		fingerPrinter.AddRange(Enumerable.Repeat(false, 10));
	}

	public override void Serialize(DBBD.Buffer buffer)
	{
		base.WriteHeader(buffer, GetLength());
		DBBD.Serialize.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, userId); }
		if (fingerPrinter[1]) { DBBD.Serialize.Write(buffer, relayType); }
		if (fingerPrinter[2]) { DBBD.Serialize.Write(buffer, intValue01); }
		if (fingerPrinter[3]) { DBBD.Serialize.Write(buffer, intValue02); }
		if (fingerPrinter[4]) { DBBD.Serialize.Write(buffer, intValue03); }
		if (fingerPrinter[5]) { DBBD.Serialize.Write(buffer, intValue04); }
		if (fingerPrinter[6]) { DBBD.Serialize.Write(buffer, floatValue01); }
		if (fingerPrinter[7]) { DBBD.Serialize.Write(buffer, floatValue02); }
		if (fingerPrinter[8]) { DBBD.Serialize.Write(buffer, floatValue03); }
		if (fingerPrinter[9]) { DBBD.Serialize.Write(buffer, floatValue04); }
	}

	public override void Deserialize(DBBD.Buffer buffer)
	{
		base.ReadHeader(buffer);
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out userId); }
		if (fingerPrinter[1]) { DBBD.Deserialize.Read(buffer, out relayType); }
		if (fingerPrinter[2]) { DBBD.Deserialize.Read(buffer, out intValue01); }
		if (fingerPrinter[3]) { DBBD.Deserialize.Read(buffer, out intValue02); }
		if (fingerPrinter[4]) { DBBD.Deserialize.Read(buffer, out intValue03); }
		if (fingerPrinter[5]) { DBBD.Deserialize.Read(buffer, out intValue04); }
		if (fingerPrinter[6]) { DBBD.Deserialize.Read(buffer, out floatValue01); }
		if (fingerPrinter[7]) { DBBD.Deserialize.Read(buffer, out floatValue02); }
		if (fingerPrinter[8]) { DBBD.Deserialize.Read(buffer, out floatValue03); }
		if (fingerPrinter[9]) { DBBD.Deserialize.Read(buffer, out floatValue04); }
	}

	public override uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(base.GetLength());
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) {  totalLength += sizeof(int);  }
		if (fingerPrinter[1]) {  totalLength += sizeof(int);  }
		if (fingerPrinter[2]) {  totalLength += sizeof(int);  }
		if (fingerPrinter[3]) {  totalLength += sizeof(int);  }
		if (fingerPrinter[4]) {  totalLength += sizeof(int);  }
		if (fingerPrinter[5]) {  totalLength += sizeof(int);  }
		if (fingerPrinter[6]) {  totalLength += sizeof(float);  }
		if (fingerPrinter[7]) {  totalLength += sizeof(float);  }
		if (fingerPrinter[8]) {  totalLength += sizeof(float);  }
		if (fingerPrinter[9]) {  totalLength += sizeof(float);  }
		return totalLength;
	}

	public int UserId { get { return userId; } set { userId = value; fingerPrinter[0] = true; } }
	public int RelayType { get { return relayType; } set { relayType = value; fingerPrinter[1] = true; } }
	public int IntValue01 { get { return intValue01; } set { intValue01 = value; fingerPrinter[2] = true; } }
	public int IntValue02 { get { return intValue02; } set { intValue02 = value; fingerPrinter[3] = true; } }
	public int IntValue03 { get { return intValue03; } set { intValue03 = value; fingerPrinter[4] = true; } }
	public int IntValue04 { get { return intValue04; } set { intValue04 = value; fingerPrinter[5] = true; } }
	public float FloatValue01 { get { return floatValue01; } set { floatValue01 = value; fingerPrinter[6] = true; } }
	public float FloatValue02 { get { return floatValue02; } set { floatValue02 = value; fingerPrinter[7] = true; } }
	public float FloatValue03 { get { return floatValue03; } set { floatValue03 = value; fingerPrinter[8] = true; } }
	public float FloatValue04 { get { return floatValue04; } set { floatValue04 = value; fingerPrinter[9] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private int userId;
	private int relayType;
	private int intValue01;
	private int intValue02;
	private int intValue03;
	private int intValue04;
	private float floatValue01;
	private float floatValue02;
	private float floatValue03;
	private float floatValue04;
}

