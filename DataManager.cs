using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataManager
{
    public static byte[] ToBinary(object obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        formatter.Serialize(ms, obj);
        ms.Close();
        return ms.ToArray();
    }

    public static object FromBinary(byte[] bites)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream(bites);
        var obj = formatter.Deserialize(stream);
        return obj;
    }


    public static void WriteToFile(object obj, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.Create);
        
        formatter.Serialize(stream, obj);
        stream.Close();
    }

    public static object ReadFromFile(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(filePath, FileMode.Open);
        object obj = formatter.Deserialize(stream);
        stream.Close();
        return obj;
    }


    public static string ToBinaryString(object obj)
    {
        var bytes = ToBinary(obj);
        return System.Convert.ToBase64String(bytes);
    }

    public static object FromBinaryString(string json)
    {
        var bytes = System.Convert.FromBase64String(json);
        return FromBinary(bytes);
    }
}
