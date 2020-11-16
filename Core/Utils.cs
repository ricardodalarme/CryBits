using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;
using Lidgren.Network;

namespace CryBits
{
    public static class Utils
    {
        // Números aleatórios
        public static readonly Random MyRandom = new Random();

        public static Directions ReverseDirection(Directions direction)
        {
            // Retorna a direção inversa
            switch (direction)
            {
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                default: return Directions.Count;
            }
        }

        public static void NextTile(Directions direction, ref byte x, ref byte y)
        {
            // Próximo azulejo
            switch (direction)
            {
                case Directions.Up: y -= 1; break;
                case Directions.Down: y += 1; break;
                case Directions.Right: x += 1; break;
                case Directions.Left: x -= 1; break;
            }
        }

        public static void Swap<T>(ref T item1, ref T item2)
        {
            // Troca dois elementos
            T temp = item1;
            item1 = item2;
            item2 = temp;
        }

        // Obtém o ID de alguma entidade, caso ela não existir retorna um ID zerado
        public static string GetID(this Entity @object) => @object == null ? Guid.Empty.ToString() : @object.ID.ToString();

        // Obtém o dado, caso ele não existir retorna nulo
        public static TV Get<TGuid, TV>(this Dictionary<TGuid, TV> dict, TGuid key) => dict.ContainsKey(key) ? dict[key] : default;

        public static void ObjectToByteArray(NetOutgoingMessage data, object obj)
        {
            var bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                bf.Serialize(stream, obj);
                data.Write(stream.ToArray().Length);
                data.Write(stream.ToArray());
            }
        }

        public static object ByteArrayToObject(NetIncomingMessage data)
        {
            int size = data.ReadInt32();
            byte[] array = data.ReadBytes(size);

            using (var stream = new MemoryStream(array))
                return new BinaryFormatter().Deserialize(stream);
        }
    }
}