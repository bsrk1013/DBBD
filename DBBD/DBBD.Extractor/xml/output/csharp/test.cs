using DBBD;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

// 유저 정보
public class UserInfo : DBBD.ICell
{
	public UserInfo()
	{
		fingerPrinter.AddRange(Enumerable.Repeat(false, 2));
	}

	public virtual void Serialize(DBBD.Buffer buffer)
	{
		DBBD.Serialize.Write(buffer, fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Serialize.Write(buffer, nickname); }
		if (fingerPrinter[1]) { DBBD.Serialize.Write(buffer, level); }
	}

	public virtual void Deserialize(DBBD.Buffer buffer)
	{
		DBBD.Deserialize.Read(buffer, out fingerPrinter);
		if (fingerPrinter[0]) { DBBD.Deserialize.Read(buffer, out nickname); }
		if (fingerPrinter[1]) { DBBD.Deserialize.Read(buffer, out level); }
	}

	public virtual uint GetLength()
	{
		uint totalLength = 0;
		totalLength += (uint)(sizeof(uint) + fingerPrinter.Count);
		if (fingerPrinter[0]) { totalLength += sizeof(uint) + Encoding.UTF8.GetByteCount(nickname) }
		if (fingerPrinter[1]) {  totalLength += sizeof(long);  }
		return totalLength;
	}

	public string Nickname { get { return nickname; } set { nickname = value; fingerPrinter[0] = true; } }
	public long Level { get { return level; } set { level = value; fingerPrinter[1] = true; } }

	private List<bool> fingerPrinter = new List<bool>();
	private string nickname;
	private long level;
}

public class PingCheckReq : DBBD.Request
{
	public PingCheckReq()
	{
		typeId = (uint)Base.Value.PingCheckReq;
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

public class PingCheckResp : DBBD.Response
{
	public PingCheckResp()
	{
		typeId = (uint)Base.Value.PingCheckResp;
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

