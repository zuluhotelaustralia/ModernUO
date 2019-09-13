﻿using Server.Mobiles;
using Server.Network;

namespace Server.Engines.MLQuests.Items
{
  public class BedlamTeleporter : Item
  {
    private static readonly Point3D PointDest = new Point3D(120, 1682, 0);
    private static readonly Map MapDest = Map.Malas;

    public BedlamTeleporter()
      : base(0x124D) =>
      Movable = false;

    public BedlamTeleporter(Serial serial)
      : base(serial)
    {
    }

    public override int LabelNumber => 1074161; // Access to Bedlam by invitation only

    public override void OnDoubleClick(Mobile from)
    {
      if (!from.InRange(GetWorldLocation(), 2))
      {
        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        return;
      }

      if (from is PlayerMobile mobile && MLQuestSystem.GetContext(mobile)?.BedlamAccess == true)
      {
        BaseCreature.TeleportPets(mobile, PointDest, MapDest);
        mobile.MoveToWorld(PointDest, MapDest);
      }
      else
      {
        from.SendLocalizedMessage(1074276); // You press and push on the iron maiden, but nothing happens.
      }
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);

      writer.Write(0); // version
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadInt();
    }
  }
}