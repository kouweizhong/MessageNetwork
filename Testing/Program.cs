﻿using MessageNetwork;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var keyPair = Utilities.GenerateOrLoadKeyPair("id_rsa");
            Console.WriteLine($"[PUBLIC] {keyPair.Public.GetHashString()}");
            var keyStore = new TrustedKeyStore("authorized_nodes");

            var node = new MessageNode<TestingMessage>(keyPair, IPAddress.Any, 12345);
            node.TrustedKeys = keyStore;
            node.NodeJoined += Node_NodeJoined;
            node.NodeLeft += Node_NodeLeft;
            node.MessageReceived += Node_MessageReceived;

            node.Setup();

            while (true)
            {
                var line = Console.ReadLine();
                node.SendMessage(null, new TestingMessage() { Text = line });
            }
        }

        private static void Node_MessageReceived(MessageNode<TestingMessage> sender, RsaKeyParameters senderKey, bool isPublic, TestingMessage message, byte[] payload)
        {
            Console.WriteLine($">> {message.Text}");
        }

        private static void Node_NodeLeft(MessageNode<TestingMessage> sender, RsaKeyParameters publicKey)
        {
            Console.WriteLine($"[LEFT] {publicKey.GetHashString()}");
        }

        private static void Node_NodeJoined(MessageNode<TestingMessage> sender, RsaKeyParameters publicKey)
        {
            Console.WriteLine($"[JOIN] {publicKey.GetHashString()}");
        }
    }
}
