using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public enum PacketType { Test, Login, LoginResponse, FindMatch, CancelFindMatch, FoundMatch, EndTurn, StartTurn, CreateEntity, RequestEntity, MoveEntity, Attack, Damage, EndGame, RequestEndGame }
public enum FindMatchType { practice, game }
//public enum EntityType { player, card }


public class ByteReader
{
    byte[] ReaderBytes;
    int ReaderCurrent;
    public ByteReader(byte[] bytes)
    {
        SetReader(bytes);
    }

    public void SetReader(byte[] bytes)
    {
        this.ReaderBytes = bytes;
        ReaderCurrent = 4;
    }

    public Int16 GetNextInt16()
    {
        Int16 result = BitConverter.ToInt16(ReaderBytes, ReaderCurrent);
        ReaderCurrent += 2;
        return result;
    }

    public Int32 GetNextInt32()
    {
        Int32 result = BitConverter.ToInt32(ReaderBytes, ReaderCurrent);
        ReaderCurrent += 4;
        return result;
    }
    public int GetNextNullTerminatedIndex(byte[] bytes, int start)
    {
        int i = start;
        while (i < bytes.Length && bytes[i] != '\0')
        {
            i++;
        }
        return i;
    }
    public string GetNextString()
    {
        int NullIndex = GetNextNullTerminatedIndex(ReaderBytes, ReaderCurrent);
        int StringLength = NullIndex - ReaderCurrent;
        string result = Encoding.UTF8.GetString(ReaderBytes, ReaderCurrent, StringLength);
        ReaderCurrent = NullIndex + 1;
        return result;
    }
}
public class PacketMaker
{
    List<byte> ByteList = new List<byte>();
    public PacketMaker(PacketType type)
    {
        ByteList.Add(0);
        ByteList.Add(0);
        ByteList.AddRange(BitConverter.GetBytes((Int16)type));
    }
    void SetSize()
    {
        byte[] size = BitConverter.GetBytes((Int16)ByteList.Count);
        ByteList[0] = size[0];
        ByteList[1] = size[1];
    }

    public string GetNullTerminatedString(string str)
    {
        if (str.Length > 0)
        {
            if (str[str.Length - 1] != '\0')
            {
                return str + '\0';
            }
            else
            {
                return str;
            }
        }
        else
        {
            return "\0";
        }
    }
    public void Add(Int16 value)
    {
        ByteList.AddRange(BitConverter.GetBytes((Int16)value));
    }

    public void Add(Int32 value)
    {
        ByteList.AddRange(BitConverter.GetBytes((Int32)value));
    }

    public void Add(byte value)
    {
        ByteList.Add(value);
    }
    public void Add(string value)
    {
        string val = GetNullTerminatedString(value);
        byte[] bytes = Encoding.UTF8.GetBytes(val);
        ByteList.AddRange(bytes);
    }

    public byte[] GetBytes()
    {
        SetSize();
        return ByteList.ToArray();
    }
}

public class Packet
{
    public PacketType type;
    protected Packet()
    {
    }
    public Packet(PacketType type)
    {
        this.type = type;
    }
    public static byte[] GetBytes(PacketType type)
    {
        PacketMaker packet = new PacketMaker(type);
        return packet.GetBytes();
    }
}

public class LoginPacket : Packet
{
    public int version;
    public string user;
    public string pass;
    LoginPacket(short version, string user, string pass)
    {
        type = PacketType.Login;
        this.user = user;
        this.pass = pass;
    }

    public static byte[] GetBytes(short version, string user, string pass)
    {
        PacketMaker packet = new PacketMaker(PacketType.Login);
        packet.Add(version);
        packet.Add(user);
        packet.Add(pass);
        return packet.GetBytes();
    }
    public static LoginPacket GetPacket(byte[] data)
    {
        ByteReader br = new ByteReader(data);
        short version = br.GetNextInt16();
        string user = br.GetNextString();
        string pass = br.GetNextString();
        return new LoginPacket(version, user, pass);
    }
}

public class FindMatchPacket : Packet
{
    public FindMatchType match_type;
    FindMatchPacket(FindMatchType match_type)
    {
        type = PacketType.FindMatch;
        this.match_type = match_type;
    }

    public static byte[] GetBytes(FindMatchType match_type)
    {
        PacketMaker packet = new PacketMaker(PacketType.FindMatch);
        packet.Add((short)match_type);
        return packet.GetBytes();
    }
    public static FindMatchPacket GetPacket(byte[] data)
    {
        ByteReader br = new ByteReader(data);
        short match_type = br.GetNextInt16();
        return new FindMatchPacket((FindMatchType)match_type);
    }
}

public class LoginResponsePacket : Packet
{
    public int id;
    LoginResponsePacket(int id)
    {
        type = PacketType.LoginResponse;
        this.id = id;
    }
    public static byte[] GetBytes(int id)
    {
        PacketMaker packet = new PacketMaker(PacketType.LoginResponse);
        packet.Add((Int16)id);
        return packet.GetBytes();
    }
    public static LoginResponsePacket GetPacket(byte[] data)
    {
        ByteReader br = new ByteReader(data);
        int id = br.GetNextInt16();
        return new LoginResponsePacket(id);
    }

}
public class FoundMatchPacket : Packet
{
    public int PlayerID = -1;
    FoundMatchPacket(int PlayerID)
    {
        type = PacketType.FoundMatch;
        this.PlayerID = PlayerID;
    }
    public static byte[] GetBytes(int PlayerID)
    {
        PacketMaker packet = new PacketMaker(PacketType.FoundMatch);
        packet.Add((Int16)PlayerID);
        return packet.GetBytes();
    }
    public static FoundMatchPacket GetPacket(byte[] data)
    {
        int pID = BitConverter.ToInt16(data, 4);
        FoundMatchPacket packet = new FoundMatchPacket(pID);
        return packet;
    }
}

public class CreateEntityPacket : Packet
{
    public int PlayerID;
    public int NetID;
    public int CardID;
    public int x;
    public int y;

    CreateEntityPacket(int PlayerID, int NetID, int CardID, int x, int y)
    {
        type = PacketType.CreateEntity;
        this.PlayerID = PlayerID;
        this.NetID = NetID;
        this.CardID = CardID;
        this.x = x;
        this.y = y;
    }

    public static byte[] GetBytes(int PlayerID, int NetID, int CardID, int x, int y)
    {
        PacketMaker packet = new PacketMaker(PacketType.CreateEntity);
        packet.Add((Int16)PlayerID);
        packet.Add((Int16)NetID);
        packet.Add((Int16)CardID);
        packet.Add(x);
        packet.Add(y);
        return packet.GetBytes();
    }
    public static CreateEntityPacket GetPacket(byte[] data)
    {
        int PlayerID = BitConverter.ToInt16(data, 4);
        int NetID = BitConverter.ToInt16(data, 6);
        int CardID = BitConverter.ToInt16(data, 8);
        int x = BitConverter.ToInt32(data, 10);
        int y = BitConverter.ToInt32(data, 14);
        CreateEntityPacket packet = new CreateEntityPacket(PlayerID, NetID, CardID, x, y);
        return packet;
    }
}

public class RequestEntityPacket : Packet
{
    public int CardID;
    public int x;
    public int y;
    RequestEntityPacket(int CardID, int x, int y)
    {
        type = PacketType.RequestEntity;
        this.CardID = CardID;
        this.x = x;
        this.y = y;

    }
    public static byte[] GetBytes(int CardID, int x, int y)
    {
        PacketMaker packet = new PacketMaker(PacketType.RequestEntity);
        packet.Add((Int16)CardID);
        packet.Add(x);
        packet.Add(y);
        return packet.GetBytes();
    }
    public static RequestEntityPacket GetPacket(byte[] data)
    {
        ByteReader reader = new ByteReader(data);
        int CardID = reader.GetNextInt16();
        int x = reader.GetNextInt32();
        int y = reader.GetNextInt32();
        RequestEntityPacket packet = new RequestEntityPacket(CardID, x, y);
        return packet;
    }
}

public class MoveEntityPacket : Packet
{
    public int NetID;
    public int x;
    public int y;
    MoveEntityPacket(int NetID, int x, int y)
    {
        this.type = PacketType.MoveEntity;
        this.NetID = NetID;
        this.x = x;
        this.y = y;
    }
    public static byte[] GetBytes(int NetID, int x, int y)
    {
        PacketMaker packet = new PacketMaker(PacketType.MoveEntity);
        packet.Add((Int16)NetID);
        packet.Add(x);
        packet.Add(y);
        return packet.GetBytes();
    }
    public static MoveEntityPacket GetPacket(byte[] data)
    {
        ByteReader reader = new ByteReader(data);
        int NetID = reader.GetNextInt16();
        int x = reader.GetNextInt32();
        int y = reader.GetNextInt32();
        MoveEntityPacket packet = new MoveEntityPacket(NetID, x, y);
        return packet;
    }
}

public class AttackPacket : Packet
{
    public int attacker;
    public int defender;
    public int damage;
    AttackPacket(int attacker, int defender)
    {
        this.type = PacketType.Attack;
        this.attacker = attacker;
        this.defender = defender;
    }
    public static byte[] GetBytes(int attacker, int defender)
    {
        PacketMaker packet = new PacketMaker(PacketType.Attack);
        packet.Add((Int16)attacker);
        packet.Add((Int16)defender);
        return packet.GetBytes();
    }
    public static AttackPacket GetPacket(byte[] data)
    {
        ByteReader reader = new ByteReader(data);
        int attacker = reader.GetNextInt16();
        int defender = reader.GetNextInt16();
        AttackPacket packet = new AttackPacket(attacker, defender);
        return packet;
    }
}

public class DamagePacket : Packet
{
    int target;
    int damage;
    DamagePacket(int target, int damage)
    {
        this.type = PacketType.Damage;
        this.target = target;
        this.damage = damage;
    }
    public static byte[] GetBytes(int target, int damage)
    {
        PacketMaker packet = new PacketMaker(PacketType.Damage);
        packet.Add((Int16)target);
        packet.Add((Int16)damage);
        return packet.GetBytes();
    }
    public static DamagePacket GetPacket(byte[] data)
    {
        ByteReader reader = new ByteReader(data);
        int target = reader.GetNextInt16();
        int damage = reader.GetNextInt16();
        DamagePacket packet = new DamagePacket(target, damage);
        return packet;
    }
}

public class PacketManager
{
    PacketType GetPacketType(byte[] data)
    {
        return (PacketType)BitConverter.ToInt16(data, 2);
    }

    public Packet ReadPacket(byte[] data)
    {
        PacketType type = GetPacketType(data);

        switch (type)
        {
            case PacketType.Login:
                return LoginPacket.GetPacket(data);
            case PacketType.EndGame:
                return new Packet(PacketType.EndGame);
            case PacketType.RequestEndGame:
                return new Packet(PacketType.RequestEndGame);
            case PacketType.LoginResponse:
                return LoginResponsePacket.GetPacket(data);
            case PacketType.FindMatch:
                return FindMatchPacket.GetPacket(data);
            case PacketType.CancelFindMatch:
                return new Packet(PacketType.CancelFindMatch);
            case PacketType.FoundMatch:
                return FoundMatchPacket.GetPacket(data);
            case PacketType.StartTurn:
                return new Packet(PacketType.StartTurn);
            case PacketType.EndTurn:
                return new Packet(PacketType.EndTurn);
            case PacketType.CreateEntity:
                return CreateEntityPacket.GetPacket(data);
            case PacketType.RequestEntity:
                return RequestEntityPacket.GetPacket(data);
            case PacketType.MoveEntity:
                return MoveEntityPacket.GetPacket(data);
            case PacketType.Attack:
                return AttackPacket.GetPacket(data);
            case PacketType.Damage:
                return DamagePacket.GetPacket(data);
        }
        return null;
    }
}

