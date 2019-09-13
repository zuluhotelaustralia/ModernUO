using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles
{
  public class AntLion : BaseCreature
  {
    [Constructible]
    public AntLion() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
      Body = 787;
      BaseSoundID = 1006;

      SetStr(296, 320);
      SetDex(81, 105);
      SetInt(36, 60);

      SetHits(151, 162);

      SetDamage(7, 21);

      SetDamageType(ResistanceType.Physical, 70);
      SetDamageType(ResistanceType.Poison, 30);

      SetResistance(ResistanceType.Physical, 45, 60);
      SetResistance(ResistanceType.Fire, 25, 35);
      SetResistance(ResistanceType.Cold, 30, 40);
      SetResistance(ResistanceType.Poison, 40, 50);
      SetResistance(ResistanceType.Energy, 30, 35);

      SetSkill(SkillName.MagicResist, 70.0);
      SetSkill(SkillName.Tactics, 90.0);
      SetSkill(SkillName.Wrestling, 90.0);

      Fame = 4500;
      Karma = -4500;

      VirtualArmor = 45;

      PackItem(new Bone(3));
      PackItem(new FertileDirt(Utility.RandomMinMax(1, 5)));

      if (Core.ML && Utility.RandomDouble() < .33)
        PackItem(Seed.RandomPeculiarSeed(2));

      Item orepile = null; /* no trust, no love :( */

      switch (Utility.Random(4))
      {
        case 0:
          orepile = new DullCopperOre();
          break;
        case 1:
          orepile = new ShadowIronOre();
          break;
        case 2:
          orepile = new CopperOre();
          break;
        default:
          orepile = new BronzeOre();
          break;
      }

      orepile.Amount = Utility.RandomMinMax(1, 10);
      orepile.ItemID = 0x19B9;
      PackItem(orepile);

      // TODO: skeleton
    }


    public AntLion(Serial serial) : base(serial)
    {
    }

    public override string CorpseName => "an ant lion corpse";
    public override string DefaultName => "an ant lion";

    public override int GetAngerSound() => 0x5A;

    public override int GetIdleSound() => 0x5A;

    public override int GetAttackSound() => 0x164;

    public override int GetHurtSound() => 0x187;

    public override int GetDeathSound() => 0x1BA;

    public override void GenerateLoot()
    {
      AddLoot(LootPack.Average, 2);
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);
      writer.Write(0);
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);
      int version = reader.ReadInt();
    }
  }
}