/***************************************************************************
 *                              GumpTextEntry.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Buffers;
using Server.Buffers;
using Server.Collections;

namespace Server.Gumps
{
  public class GumpTextEntry : GumpEntry
  {
    public GumpTextEntry(int x, int y, int width, int height, int hue, int entryID, string initialText)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
      Hue = hue;
      EntryID = entryID;
      InitialText = initialText;
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int Hue { get; set; }

    public int EntryID { get; set; }

    public string InitialText { get; set; }

    public override string Compile(ArraySet<string> strings) => $"{{ textentry {X} {Y} {Width} {Height} {Hue} {EntryID} {strings.Add(InitialText)} }}";

    private static readonly byte[] m_LayoutName = Gump.StringToBuffer(" { textentry ");

    public override void AppendTo(ArrayBufferWriter<byte> buffer, ArraySet<string> strings, ref int entries, ref int switches)
    {
      SpanWriter writer = new SpanWriter(buffer.GetSpan(90));
      writer.Write(m_LayoutName);
      writer.WriteAscii(X.ToString());
      writer.Write((byte)0x20); // ' '
      writer.WriteAscii(Y.ToString());
      writer.Write((byte)0x20); // ' '
      writer.WriteAscii(Width.ToString());
      writer.Write((byte)0x20); // ' '
      writer.WriteAscii(Height.ToString());
      writer.Write((byte)0x20); // ' '
      writer.WriteAscii(Hue.ToString());
      writer.Write((byte)0x20); // ' '
      writer.WriteAscii(EntryID.ToString());
      writer.Write((byte)0x20); // ' '
      writer.WriteAscii(strings.Add(InitialText).ToString());
      writer.Write((byte)0x20); // ' '
      writer.Write((byte)0x7D); // '}'

      buffer.Advance(writer.WrittenCount);

      entries++;
    }
  }
}
