using LiteDB;
using Server.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server
{
  public class ShrinkTable
  {
    public const int DefaultItemID = 0x1870; // Yellow virtue stone

    private static Dictionary<int, int> m_Dict;

    public static int Lookup(Mobile m) => Lookup(m.Body.BodyID, DefaultItemID);

    public static int Lookup(int body) => Lookup(body, DefaultItemID);

    public static int Lookup(Mobile m, int defaultValue) => Lookup(m.Body.BodyID, defaultValue);

    public static int Lookup(int body, int defaultValue)
    {
      if (m_Dict == null)
        Load();

      int val = 0;

      if (body >= 0)
        m_Dict.TryGetValue(body, out val);

      if (val == 0)
        val = defaultValue;

      return val;
    }

    private static void Load()
    {
      try
      {
        using (var db = Database.Instance)
        {
          var results = db.GetCollection<Shrinkling>("shrink");
          var list = new List<Shrinkling>(results.FindAll());
          m_Dict = new Dictionary<int, int>();
          m_Dict = list.ToDictionary(x => x.Key, x => x.Value);
        }
      }
      catch (LiteException ex)
      {
        throw ex;
      }
    }
  }
  public class Shrinkling
  {
    public long ID { get; set; }
    public int Key { get; set; }
    public int Value { get; set; }
  }
}
