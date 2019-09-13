using System;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
  public enum ToolQuality
  {
    Low,
    Regular,
    Exceptional
  }

  public abstract class BaseTool : Item, IUsesRemaining, ICraftable
  {
    private Mobile m_Crafter;
    private ToolQuality m_Quality;
    private int m_UsesRemaining;

    public BaseTool(int itemID) : this(Utility.RandomMinMax(25, 75), itemID)
    {
    }

    public BaseTool(int uses, int itemID) : base(itemID)
    {
      m_UsesRemaining = uses;
      m_Quality = ToolQuality.Regular;
    }

    public BaseTool(Serial serial) : base(serial)
    {
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public Mobile Crafter
    {
      get => m_Crafter;
      set
      {
        m_Crafter = value;
        InvalidateProperties();
      }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public ToolQuality Quality
    {
      get => m_Quality;
      set
      {
        UnscaleUses();
        m_Quality = value;
        InvalidateProperties();
        ScaleUses();
      }
    }

    public virtual bool BreakOnDepletion => true;

    public abstract CraftSystem CraftSystem{ get; }

    #region ICraftable Members

    public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool,
      CraftItem craftItem, int resHue)
    {
      Quality = (ToolQuality)quality;

      if (makersMark)
        Crafter = from;

      return quality;
    }

    #endregion

    [CommandProperty(AccessLevel.GameMaster)]
    public int UsesRemaining
    {
      get => m_UsesRemaining;
      set
      {
        m_UsesRemaining = value;
        InvalidateProperties();
      }
    }

    private bool ShowUsesRemaining{ get; set; } = true;

    bool IUsesRemaining.ShowUsesRemaining
    {
      get => ShowUsesRemaining;
      set => ShowUsesRemaining = value;
    }

    public void ScaleUses()
    {
      m_UsesRemaining = m_UsesRemaining * GetUsesScalar() / 100;
      InvalidateProperties();
    }

    public void UnscaleUses()
    {
      m_UsesRemaining = m_UsesRemaining * 100 / GetUsesScalar();
    }

    public int GetUsesScalar()
    {
      if (m_Quality == ToolQuality.Exceptional)
        return 200;

      return 100;
    }

    public override void GetProperties(ObjectPropertyList list)
    {
      base.GetProperties(list);

      // Makers mark not displayed on OSI
      //if ( m_Crafter != null )
      //	list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

      if (m_Quality == ToolQuality.Exceptional)
        list.Add(1060636); // exceptional

      list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
    }

    public virtual void DisplayDurabilityTo(Mobile m)
    {
      LabelToAffix(m, 1017323, AffixType.Append, ": " + m_UsesRemaining); // Durability
    }

    public static bool CheckAccessible(Item tool, Mobile m) => tool.IsChildOf(m) || tool.Parent == m;

    public static bool CheckTool(Item tool, Mobile m)
    {
      Item check = m.FindItemOnLayer(Layer.OneHanded);

      if (check is BaseTool && check != tool && !(check is AncientSmithyHammer))
        return false;

      check = m.FindItemOnLayer(Layer.TwoHanded);

      if (check is BaseTool && check != tool && !(check is AncientSmithyHammer))
        return false;

      return true;
    }

    public override void OnSingleClick(Mobile from)
    {
      DisplayDurabilityTo(from);

      base.OnSingleClick(from);
    }

    public override void OnDoubleClick(Mobile from)
    {
      if (IsChildOf(from.Backpack) || Parent == from)
      {
        CraftSystem system = CraftSystem;

        int num = system.CanCraft(from, this, null);

        if (num > 0 && (num != 1044267 || !Core.SE)
        ) // Blacksmithing shows the gump regardless of proximity of an anvil and forge after SE
        {
          from.SendLocalizedMessage(num);
        }
        else
        {
          CraftContext context = system.GetContext(from);

          from.SendGump(new CraftGump(from, system, this, null));
        }
      }
      else
      {
        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
      }
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);

      writer.Write(1); // version

      writer.Write(m_Crafter);
      writer.Write((int)m_Quality);

      writer.Write(m_UsesRemaining);
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadInt();

      switch (version)
      {
        case 1:
        {
          m_Crafter = reader.ReadMobile();
          m_Quality = (ToolQuality)reader.ReadInt();
          goto case 0;
        }
        case 0:
        {
          m_UsesRemaining = reader.ReadInt();
          break;
        }
      }
    }
  }
}