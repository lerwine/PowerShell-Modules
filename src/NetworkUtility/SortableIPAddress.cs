using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NetworkUtility;

public class SortableIPAddress : IComparable<SortableIPAddress>, IEquatable<SortableIPAddress>, IComparable
{
    private readonly IPAddress _address;

    public IPAddress Address { get { return _address; } }

    public SortableIPAddress(IPAddress address)
    {
        if (address == null) throw new ArgumentNullException("address");
        switch (address.AddressFamily)
        {
            case AddressFamily.InterNetwork:
            case AddressFamily.InterNetworkV6:
                break;
            default:
                throw new ArgumentException("Address family " + address.AddressFamily.ToString("F") + " is not supported.");
        }
        _address = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
    }

    private static int CompareTo(byte[] lBytes, byte[] rBytes)
    {
        int length = lBytes.Length;
        for (int i = 0; i < lBytes.Length; i++)
        {
            int diff = (int)lBytes[i] - (int)rBytes[i];
            if (diff != 0) return diff;
        }
        return 0;
    }

    private static bool EqualTo(byte[] lBytes, byte[] rBytes)
    {
        int length = lBytes.Length;
        for (int i = 0; i < lBytes.Length; i++)
        {
            if (lBytes[i] != rBytes[i]) return false;
        }
        return true;
    }

    public int CompareTo(SortableIPAddress other)
    {
        if (other == null) return 1;
        if (ReferenceEquals(this._address, other._address)) return 0;
        if (_address.AddressFamily == AddressFamily.InterNetwork)
        {
            if (other._address.AddressFamily != AddressFamily.InterNetwork)
                return CompareTo(_address.GetAddressBytes(), _address.MapToIPv6().GetAddressBytes());
        }
        else if (other._address.AddressFamily != AddressFamily.InterNetwork)
                return CompareTo(_address.MapToIPv6().GetAddressBytes(), _address.GetAddressBytes());
        return CompareTo(_address.GetAddressBytes(), _address.GetAddressBytes());
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        if (obj is SortableIPAddress) return CompareTo((SortableIPAddress)obj);
        if (obj is IPAddress)
        {
            if (ReferenceEquals(obj, _address)) return 0;
            IPAddress other = (IPAddress)obj;
            switch (other.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    if (_address.AddressFamily == AddressFamily.InterNetwork)
                        return CompareTo(_address.GetAddressBytes(), other.GetAddressBytes());
                    return CompareTo(_address.GetAddressBytes(), other.MapToIPv6().GetAddressBytes());
                case AddressFamily.InterNetworkV6:
                    if (_address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (other.IsIPv4MappedToIPv6)
                            return CompareTo(_address.GetAddressBytes(), other.MapToIPv4().GetAddressBytes());
                        return CompareTo(_address.MapToIPv6().GetAddressBytes(), other.GetAddressBytes());
                    }
                    return CompareTo(_address.GetAddressBytes(), other.GetAddressBytes());
            }
        }
        string s = obj.ToString();
        return (s == null) ? 1 : _address.ToString().CompareTo(s);
    }

    public bool Equals(SortableIPAddress other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this._address, other._address)) return true;
        if (_address.AddressFamily == AddressFamily.InterNetwork)
        {
            if (other._address.AddressFamily != AddressFamily.InterNetwork)
                return EqualTo(_address.GetAddressBytes(), _address.MapToIPv6().GetAddressBytes());
        }
        else if (other._address.AddressFamily != AddressFamily.InterNetwork)
                return EqualTo(_address.MapToIPv6().GetAddressBytes(), _address.GetAddressBytes());
        return EqualTo(_address.GetAddressBytes(), _address.GetAddressBytes());
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj is SortableIPAddress) return Equals((SortableIPAddress)obj);
        if (obj is IPAddress)
        {
            if (ReferenceEquals(obj, _address)) return true;
            IPAddress other = (IPAddress)obj;
            switch (other.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    if (_address.AddressFamily == AddressFamily.InterNetwork)
                        return EqualTo(_address.GetAddressBytes(), other.GetAddressBytes());
                    return EqualTo(_address.GetAddressBytes(), other.MapToIPv6().GetAddressBytes());
                case AddressFamily.InterNetworkV6:
                    if (_address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (other.IsIPv4MappedToIPv6)
                            return EqualTo(_address.GetAddressBytes(), other.MapToIPv4().GetAddressBytes());
                        return EqualTo(_address.MapToIPv6().GetAddressBytes(), other.GetAddressBytes());
                    }
                    return EqualTo(_address.GetAddressBytes(), other.GetAddressBytes());
            }
        }
        string s = obj.ToString();
        return (s == null) ? 1 : _address.ToString().Equals(s);
    }

    public override int GetHashCode()
    {
        int hash = 7;
        unchecked
        {
            byte[] buffer = new byte[4];
            int index = 0;
            foreach (byte octet in _address.GetAddressBytes())
            {
                buffer[index] = octet;
                index++;
                if (index == 4)
                {
                    hash = hash * 13 + BitConverter.ToInt32(buffer, 0);
                    index = 0;
                }
            }
        }
        return hash;
    }

    public override string ToString() { return _address.ToString(); }
}
