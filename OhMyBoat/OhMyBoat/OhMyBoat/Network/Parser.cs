﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OhMyBoat.Network.Packets;

namespace OhMyBoat.Network
{
    static class Parser
    {
        static readonly Dictionary<byte, BasePacket> Packets = new Dictionary<byte, BasePacket>();

        public static void RegisterPackets()
        {

        }

        public static void Parse(Client client, Packet packet)
        {
            if(Packets.ContainsKey(packet.Header.OpCode))
                Packets[packet.Header.OpCode].Unpack(client, packet);
            else
                throw new Exception("Packet inconnu: " + packet.Header.OpCode);
        }
    }
}