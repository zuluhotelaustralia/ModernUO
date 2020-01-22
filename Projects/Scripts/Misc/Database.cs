using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Misc
{
  class Database
  {
    private static readonly LiteDatabase instance = new LiteDatabase(Configuration.Instance.dataDB);

    static Database()
    {
    }

    private Database()
    {
    }

    public static LiteDatabase Instance
    {
      get
      {
        return instance;
      }
    }
  }
}
