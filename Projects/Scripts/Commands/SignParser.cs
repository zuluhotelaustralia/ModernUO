using System.Collections.Generic;
using System.IO;
using LiteDB;
using Server.Items;
using Server.Misc;

namespace Server.Commands
{
  public class SignParser
  {
    private static Queue<Item> m_ToDelete = new Queue<Item>();

    public static void Initialize()
    {
      CommandSystem.Register("SignGen", AccessLevel.Administrator, SignGen_OnCommand);
    }

    [Usage("SignGen")]
    [Description("Generates world/shop signs on all facets.")]
    public static void SignGen_OnCommand(CommandEventArgs c)
    {
      Parse(c.Mobile);
    }

    public static void Parse(Mobile from)
    {
      LiteCollection<SignEntry> signs;
      from.SendMessage("Generating signs, please wait.");

      try
      {
        using (var db = Database.Instance)
        {
          signs = db.GetCollection<SignEntry>("signs");
        }

        Map[] brit = { Map.Felucca, Map.Trammel };
        Map[] fel = { Map.Felucca };
        Map[] tram = { Map.Trammel };
        Map[] ilsh = { Map.Ilshenar };
        Map[] malas = { Map.Malas };
        Map[] tokuno = { Map.Tokuno };

        foreach (var sign in signs.FindAll())
        {
          var maps = sign.m_Map switch
          {
            0 => brit,
            1 => fel,
            2 => tram,
            3 => ilsh,
            4 => malas,
            5 => tokuno,
            _ => null
          };

          for (int j = 0; maps?.Length > j; ++j)
            Add_Static(sign.m_ItemID, sign.m_Location, maps[j], sign.m_Text);

        }
        from.SendMessage("Sign generating complete.");
      }
      catch (LiteException ex)
      {
        throw ex;
      }
    }

    public static void Add_Static(int itemID, Point3D location, Map map, string name)
    {
      IPooledEnumerable<Item> eable = map.GetItemsInRange(location, 0);

      foreach (Item item in eable)
        if (item is Sign && item.Z == location.Z && item.ItemID == itemID)
          m_ToDelete.Enqueue(item);

      eable.Free();

      while (m_ToDelete.Count > 0)
        m_ToDelete.Dequeue().Delete();

      Item sign;

      if (name.StartsWith("#"))
      {
        sign = new LocalizedSign(itemID, Utility.ToInt32(name.Substring(1)));
      }
      else
      {
        sign = new Sign(itemID);
        sign.Name = name;
      }

      if (map == Map.Malas)
      {
        if (location.X >= 965 && location.Y >= 502 && location.X <= 1012 && location.Y <= 537)
          sign.Hue = 0x47E;
        else if (location.X >= 1960 && location.Y >= 1278 && location.X < 2106 && location.Y < 1413)
          sign.Hue = 0x44E;
      }

      sign.MoveToWorld(location, map);
    }

    private class SignEntry
    {
      public int m_id { get; set; }
      public int m_ItemID { get; set; }
      public Point3D m_Location { get; set; }
      public int m_Map { get; set; }
      public string m_Text { get; set; }

      public SignEntry(string text, Point3D pt, int itemID, int mapLoc)
      {
        m_Text = text;
        m_Location = pt;
        m_ItemID = itemID;
        m_Map = mapLoc;
      }
      public SignEntry()
      {

      }
    }
  }
}
