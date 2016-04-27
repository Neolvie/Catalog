using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sungero.Core
{
  /// <summary>
  /// Цвет.
  /// </summary>
  [Serializable]
  public struct Color : IEquatable<Color>
  {
    #region Поля

    private readonly byte a;

    private readonly byte r;

    private readonly byte g;

    private readonly byte b;

    /// <summary>
    /// Прозрачность.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A",
      Justification = "Общепринятое обозначение для цветов")]
    public byte A { get { return this.a; } }

    /// <summary>
    /// Красная составляющая.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "R",
      Justification = "Общепринятое обозначение для цветов")]
    public byte R { get { return this.r; } }

    /// <summary>
    /// Зеленая состовляющая.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "G",
      Justification = "Общепринятое обозначение для цветов")]
    public byte G { get { return this.g; } }

    /// <summary>
    /// Синяя составляющая.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "B",
      Justification = "Общепринятое обозначение для цветов")]
    public byte B { get { return this.b; } }

    #endregion

    #region Методы

    /// <summary>
    /// Сравнение текущего цвета с переданным.
    /// </summary>
    /// <param name="other">Второй цвет.</param>
    /// <returns>True, если цвета равны, иначе false.</returns>
    public bool Equals(Color other)
    {
      if (object.ReferenceEquals(other, null))
        return false;

      return other.a == this.a &&
        other.r == this.r &&
        other.g == this.g &&
        other.b == this.b;
    }

    public static bool operator ==(Color first, Color second)
    {
      if (object.ReferenceEquals(first, null))
        return object.ReferenceEquals(second, null);

      if (object.ReferenceEquals(second, null))
        return object.ReferenceEquals(first, null);

      return first.Equals(second);
    }

    public static bool operator !=(Color first, Color second)
    {
      if (object.ReferenceEquals(first, null))
        return !object.ReferenceEquals(second, null);

      if (object.ReferenceEquals(second, null))
        return !object.ReferenceEquals(first, null);

      return !first.Equals(second);
    }

    #endregion

    #region Базовый класс

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(obj, null) || obj.GetType() != typeof(Color))
        return false;

      return this.Equals((Color)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = this.a.GetHashCode();
        result = (result * 397) ^ this.r.GetHashCode();
        result = (result * 397) ^ this.g.GetHashCode();
        result = (result * 397) ^ this.b.GetHashCode();
        return result;
      }
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}{3:x2}", this.a, this.r, this.g, this.b);
    }

    #endregion

    #region Конструкторы

    internal Color(byte a, byte r, byte g, byte b)
    {
      this.a = a;
      this.r = r;
      this.g = g;
      this.b = b;
    }

    #endregion
  }

  /// <summary>
  /// Цвета.
  /// </summary>
  public static class Colors
  {
    #region Методы

    /// <summary>
    /// Разобрать цвет из строкового представления.
    /// </summary>
    /// <param name="value">Строковое значение.</param>
    /// <returns>Цвет.</returns>
    public static Color Parse(string value)
    {
      Color result;
      if (!Colors.TryParse(value, out result))
        throw new FormatException("Invalid format.");

      return result;
    }

    /// <summary>
    /// Разобрать цвет из строкового представления.
    /// </summary>
    /// <param name="value">Строковое представление цвета.</param>
    /// <param name="result">Цвет.</param>
    /// <returns>True, если получилось разобрать цвет, иначе false.</returns>
    internal static bool TryParse(string value, out Color result)
    {
      value = value.Trim();
      if (value.StartsWith("#", StringComparison.OrdinalIgnoreCase))
      {
        value = value.Trim('#');
        uint u;
        if (!uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u))
        {
          result = Colors.Empty;
          return false;
        }

        if (value.Length < 8)
          u += 0xFF000000;

        result = FromUInt32(u);
        return true;
      }

      var values = value.Split(',');
      if (values.Length < 3 || values.Length > 4)
      {
        result = Colors.Empty;
        return false;
      }

      var i = 0;

      byte alpha = 255;
      if (values.Length > 3)
        if (!byte.TryParse(values[i++], out alpha))
        {
          result = Colors.Empty;
          return false;
        }

      byte red;
      byte green;
      byte blue;

      if (!byte.TryParse(values[i++], out red) ||
          !byte.TryParse(values[i++], out green) ||
          !byte.TryParse(values[i], out blue))
      {
        result = Colors.Empty;
        return false;
      }

      result = FromArgb(alpha, red, green, blue);
      return true;
    }

    /// <summary>
    /// Получить цвет из числового представления.
    /// </summary>
    /// <param name="color">Числовое представление цвета.</param>
    /// <returns>Цвет.</returns>
    public static Color FromUInt32(uint color)
    {
      var a = (byte)(color >> 24);
      var r = (byte)(color >> 16);
      var g = (byte)(color >> 8);
      var b = (byte)(color >> 0);
      return FromArgb(a, r, g, b);
    }

    /// <summary>
    /// Получить цвет из составляющих.
    /// </summary>
    /// <param name="a">Прозрачность.</param>
    /// <param name="r">Красная составляющая.</param>
    /// <param name="g">Зеленая составляющая.</param>
    /// <param name="b">Синяя составляющая.</param>
    /// <returns>Цвет.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Argb", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "g", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Общепринятый термин")]
    public static Color FromArgb(byte a, byte r, byte g, byte b)
    {
      return new Color(a, r, g, b);
    }

    /// <summary>
    /// Получить цвет из составляющих.
    /// </summary>
    /// <param name="r">Красная составляющая.</param>
    /// <param name="g">Зеленая составляющая.</param>
    /// <param name="b">Синяя составляющая.</param>
    /// <returns>Цвет.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rgb", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "g", Justification = "Общепринятый термин"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Общепринятый термин")]
    public static Color FromRgb(byte r, byte g, byte b)
    {
      return new Color(255, r, g, b);
    }

    #endregion

    #region Константы для цветов.

    /// <summary>
    /// Пустое значение цвета.
    /// </summary>
    public static readonly Color Empty = new Color(0, 0, 0, 0);

    private static readonly Lazy<CommonColors> common = new Lazy<CommonColors>(() => new CommonColors(), true);

    /// <summary>
    /// Общие цвета.
    /// </summary>
    public static CommonColors Common { get { return common.Value; } }

    private static readonly Lazy<ChartColors> charts = new Lazy<ChartColors>(() => new ChartColors(), true);

    /// <summary>
    /// Цвета для графиков.
    /// </summary>
    public static ChartColors Charts { get { return charts.Value; } }

    #endregion
  }

  /// <summary>
  /// Палитра общих цветов.
  /// </summary>
  public class CommonColors
  {
    #region Instance

    internal CommonColors() { }

    #endregion

    #region Цвета

    /// <summary>
    /// The alice blue.
    /// </summary>
    public Color AliceBlue { get { return Colors.FromUInt32(0xFFF0F8FF); } }

    /// <summary>
    /// The antique white.
    /// </summary>
    public Color AntiqueWhite { get { return Colors.FromUInt32(0xFFFAEBD7); } }

    /// <summary>
    /// The aqua.
    /// </summary>
    public Color Aqua { get { return Colors.FromUInt32(0xFF00FFFF); } }

    /// <summary>
    /// The aquamarine.
    /// </summary>
    public Color Aquamarine { get { return Colors.FromUInt32(0xFF7FFFD4); } }

    /// <summary>
    /// The azure.
    /// </summary>
    public Color Azure { get { return Colors.FromUInt32(0xFFF0FFFF); } } 

    /// <summary>
    /// The beige.
    /// </summary>
    public Color Beige { get { return Colors.FromUInt32(0xFFF5F5DC); } }

    /// <summary>
    /// The bisque.
    /// </summary>
    public Color Bisque { get { return Colors.FromUInt32(0xFFFFE4C4); } }

    /// <summary>
    /// The black.
    /// </summary>
    public Color Black { get { return Colors.FromUInt32(0xFF000000); } }

    /// <summary>
    /// The blanched almond.
    /// </summary>
    public Color BlanchedAlmond { get { return Colors.FromUInt32(0xFFFFEBCD); } }

    /// <summary>
    /// The blue.
    /// </summary>
    public Color Blue { get { return Colors.FromUInt32(0xFF0000FF); } }

    /// <summary>
    /// The blue violet.
    /// </summary>
    public Color BlueViolet { get { return Colors.FromUInt32(0xFF8A2BE2); } }

    /// <summary>
    /// The brown.
    /// </summary>
    public Color Brown { get { return Colors.FromUInt32(0xFFA52A2A); } }

    /// <summary>
    /// The burly wood.
    /// </summary>
    public Color BurlyWood { get { return Colors.FromUInt32(0xFFDEB887); } }

    /// <summary>
    /// The cadet blue.
    /// </summary>
    public Color CadetBlue { get { return Colors.FromUInt32(0xFF5F9EA0); } }

    /// <summary>
    /// The chartreuse.
    /// </summary>
    public Color Chartreuse { get { return Colors.FromUInt32(0xFF7FFF00); } }

    /// <summary>
    /// The chocolate.
    /// </summary>
    public Color Chocolate { get { return Colors.FromUInt32(0xFFD2691E); } }

    /// <summary>
    /// The coral.
    /// </summary>
    public Color Coral { get { return Colors.FromUInt32(0xFFFF7F50); } }

    /// <summary>
    /// The cornflower blue.
    /// </summary>
    public Color CornflowerBlue { get { return Colors.FromUInt32(0xFF6495ED); } }

    /// <summary>
    /// The cornsilk.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cornsilk",
      Justification = "Корректное название цвета.")]
    public Color Cornsilk { get { return Colors.FromUInt32(0xFFFFF8DC); } }

    /// <summary>
    /// The crimson.
    /// </summary>
    public Color Crimson { get { return Colors.FromUInt32(0xFFDC143C); } }

    /// <summary>
    /// The cyan.
    /// </summary>
    public Color Cyan { get { return Colors.FromUInt32(0xFF00FFFF); } }

    /// <summary>
    /// The dark blue.
    /// </summary>
    public Color DarkBlue { get { return Colors.FromUInt32(0xFF00008B); } }

    /// <summary>
    /// The dark cyan.
    /// </summary>
    public Color DarkCyan { get { return Colors.FromUInt32(0xFF008B8B); } }

    /// <summary>
    /// The dark goldenrod.
    /// </summary>
    public Color DarkGoldenrod { get { return Colors.FromUInt32(0xFFB8860B); } }

    /// <summary>
    /// The dark gray.
    /// </summary>
    public Color DarkGray { get { return Colors.FromUInt32(0xFFA9A9A9); } }

    /// <summary>
    /// The dark green.
    /// </summary>
    public Color DarkGreen { get { return Colors.FromUInt32(0xFF006400); } }

    /// <summary>
    /// The dark khaki.
    /// </summary>
    public Color DarkKhaki { get { return Colors.FromUInt32(0xFFBDB76B); } }

    /// <summary>
    /// The dark magenta.
    /// </summary>
    public Color DarkMagenta { get { return Colors.FromUInt32(0xFF8B008B); } }

    /// <summary>
    /// The dark olive green.
    /// </summary>
    public Color DarkOliveGreen { get { return Colors.FromUInt32(0xFF556B2F); } }

    /// <summary>
    /// The dark orange.
    /// </summary>
    public Color DarkOrange { get { return Colors.FromUInt32(0xFFFF8C00); } }

    /// <summary>
    /// The dark orchid.
    /// </summary>
    public Color DarkOrchid { get { return Colors.FromUInt32(0xFF9932CC); } }

    /// <summary>
    /// The dark red.
    /// </summary>
    public Color DarkRed { get { return Colors.FromUInt32(0xFF8B0000); } }

    /// <summary>
    /// The dark salmon.
    /// </summary>
    public Color DarkSalmon { get { return Colors.FromUInt32(0xFFE9967A); } }

    /// <summary>
    /// The dark sea green.
    /// </summary>
    public Color DarkSeaGreen { get { return Colors.FromUInt32(0xFF8FBC8F); } }

    /// <summary>
    /// The dark slate blue.
    /// </summary>
    public Color DarkSlateBlue { get { return Colors.FromUInt32(0xFF483D8B); } }

    /// <summary>
    /// The dark slate gray.
    /// </summary>
    public Color DarkSlateGray { get { return Colors.FromUInt32(0xFF2F4F4F); } }

    /// <summary>
    /// The dark turquoise.
    /// </summary>
    public Color DarkTurquoise { get { return Colors.FromUInt32(0xFF00CED1); } }

    /// <summary>
    /// The dark violet.
    /// </summary>
    public Color DarkViolet { get { return Colors.FromUInt32(0xFF9400D3); } }

    /// <summary>
    /// The deep pink.
    /// </summary>
    public Color DeepPink { get { return Colors.FromUInt32(0xFFFF1493); } }

    /// <summary>
    /// The deep sky blue.
    /// </summary>
    public Color DeepSkyBlue { get { return Colors.FromUInt32(0xFF00BFFF); } }

    /// <summary>
    /// The dim gray.
    /// </summary>
    public Color DimGray { get { return Colors.FromUInt32(0xFF696969); } }

    /// <summary>
    /// The dodger blue.
    /// </summary>
    public Color DodgerBlue { get { return Colors.FromUInt32(0xFF1E90FF); } }

    /// <summary>
    /// The firebrick.
    /// </summary>
    public Color Firebrick { get { return Colors.FromUInt32(0xFFB22222); } }

    /// <summary>
    /// The floral white.
    /// </summary>
    public Color FloralWhite { get { return Colors.FromUInt32(0xFFFFFAF0); } }

    /// <summary>
    /// The forest green.
    /// </summary>
    public Color ForestGreen { get { return Colors.FromUInt32(0xFF228B22); } }

    /// <summary>
    /// The fuchsia.
    /// </summary>
    public Color Fuchsia { get { return Colors.FromUInt32(0xFFFF00FF); } }

    /// <summary>
    /// The gainsboro.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gainsboro",
      Justification = "Корректное название цвета.")]
    public Color Gainsboro { get { return Colors.FromUInt32(0xFFDCDCDC); } }

    /// <summary>
    /// The ghost white.
    /// </summary>
    public Color GhostWhite { get { return Colors.FromUInt32(0xFFF8F8FF); } }

    /// <summary>
    /// The gold.
    /// </summary>
    public Color Gold { get { return Colors.FromUInt32(0xFFFFD700); } }

    /// <summary>
    /// The goldenrod.
    /// </summary>
    public Color Goldenrod { get { return Colors.FromUInt32(0xFFDAA520); } }

    /// <summary>
    /// The gray.
    /// </summary>
    public Color Gray { get { return Colors.FromUInt32(0xFF808080); } }

    /// <summary>
    /// The green.
    /// </summary>
    public Color Green { get { return Colors.FromUInt32(0xFF008000); } }

    /// <summary>
    /// The green yellow.
    /// </summary>
    public Color GreenYellow { get { return Colors.FromUInt32(0xFFADFF2F); } }

    /// <summary>
    /// The honeydew.
    /// </summary>
    public Color Honeydew { get { return Colors.FromUInt32(0xFFF0FFF0); } }

    /// <summary>
    /// The hot pink.
    /// </summary>
    public Color HotPink { get { return Colors.FromUInt32(0xFFFF69B4); } }

    /// <summary>
    /// The indian red.
    /// </summary>
    public Color IndianRed { get { return Colors.FromUInt32(0xFFCD5C5C); } }

    /// <summary>
    /// The indigo.
    /// </summary>
    public Color Indigo { get { return Colors.FromUInt32(0xFF4B0082); } }

    /// <summary>
    /// The ivory.
    /// </summary>
    public Color Ivory { get { return Colors.FromUInt32(0xFFFFFFF0); } }

    /// <summary>
    /// The khaki.
    /// </summary>
    public Color Khaki { get { return Colors.FromUInt32(0xFFF0E68C); } }

    /// <summary>
    /// The lavender.
    /// </summary>
    public Color Lavender { get { return Colors.FromUInt32(0xFFE6E6FA); } }

    /// <summary>
    /// The lavender blush.
    /// </summary>
    public Color LavenderBlush { get { return Colors.FromUInt32(0xFFFFF0F5); } }

    /// <summary>
    /// The lawn green.
    /// </summary>
    public Color LawnGreen { get { return Colors.FromUInt32(0xFF7CFC00); } }

    /// <summary>
    /// The lemon chiffon.
    /// </summary>
    public Color LemonChiffon { get { return Colors.FromUInt32(0xFFFFFACD); } }

    /// <summary>
    /// The light blue.
    /// </summary>
    public Color LightBlue { get { return Colors.FromUInt32(0xFFADD8E6); } }

    /// <summary>
    /// The light coral.
    /// </summary>
    public Color LightCoral { get { return Colors.FromUInt32(0xFFF08080); } }

    /// <summary>
    /// The light cyan.
    /// </summary>
    public Color LightCyan { get { return Colors.FromUInt32(0xFFE0FFFF); } }

    /// <summary>
    /// The light goldenrod yellow.
    /// </summary>
    public Color LightGoldenrodYellow { get { return Colors.FromUInt32(0xFFFAFAD2); } }

    /// <summary>
    /// The light gray.
    /// </summary>
    public Color LightGray { get { return Colors.FromUInt32(0xFFD3D3D3); } }

    /// <summary>
    /// The light green.
    /// </summary>
    public Color LightGreen { get { return Colors.FromUInt32(0xFF90EE90); } }

    /// <summary>
    /// The light pink.
    /// </summary>
    public Color LightPink { get { return Colors.FromUInt32(0xFFFFB6C1); } }

    /// <summary>
    /// The light salmon.
    /// </summary>
    public Color LightSalmon { get { return Colors.FromUInt32(0xFFFFA07A); } }

    /// <summary>
    /// The light sea green.
    /// </summary>
    public Color LightSeaGreen { get { return Colors.FromUInt32(0xFF20B2AA); } }

    /// <summary>
    /// The light sky blue.
    /// </summary>
    public Color LightSkyBlue { get { return Colors.FromUInt32(0xFF87CEFA); } }

    /// <summary>
    /// The light slate gray.
    /// </summary>
    public Color LightSlateGray { get { return Colors.FromUInt32(0xFF778899); } }

    /// <summary>
    /// The light steel blue.
    /// </summary>
    public Color LightSteelBlue { get { return Colors.FromUInt32(0xFFB0C4DE); } }

    /// <summary>
    /// The light yellow.
    /// </summary>
    public Color LightYellow { get { return Colors.FromUInt32(0xFFFFFFE0); } }

    /// <summary>
    /// The lime.
    /// </summary>
    public Color Lime { get { return Colors.FromUInt32(0xFF00FF00); } }

    /// <summary>
    /// The lime green.
    /// </summary>
    public Color LimeGreen { get { return Colors.FromUInt32(0xFF32CD32); } }

    /// <summary>
    /// The linen.
    /// </summary>
    public Color Linen { get { return Colors.FromUInt32(0xFFFAF0E6); } }

    /// <summary>
    /// The magenta.
    /// </summary>
    public Color Magenta { get { return Colors.FromUInt32(0xFFFF00FF); } }

    /// <summary>
    /// The maroon.
    /// </summary>
    public Color Maroon { get { return Colors.FromUInt32(0xFF800000); } }

    /// <summary>
    /// The medium aquamarine.
    /// </summary>
    public Color MediumAquamarine { get { return Colors.FromUInt32(0xFF66CDAA); } }

    /// <summary>
    /// The medium blue.
    /// </summary>
    public Color MediumBlue { get { return Colors.FromUInt32(0xFF0000CD); } }

    /// <summary>
    /// The medium orchid.
    /// </summary>
    public Color MediumOrchid { get { return Colors.FromUInt32(0xFFBA55D3); } }

    /// <summary>
    /// The medium purple.
    /// </summary>
    public Color MediumPurple { get { return Colors.FromUInt32(0xFF9370DB); } }

    /// <summary>
    /// The medium sea green.
    /// </summary>
    public Color MediumSeaGreen { get { return Colors.FromUInt32(0xFF3CB371); } }

    /// <summary>
    /// The medium slate blue.
    /// </summary>
    public Color MediumSlateBlue { get { return Colors.FromUInt32(0xFF7B68EE); } }

    /// <summary>
    /// The medium spring green.
    /// </summary>
    public Color MediumSpringGreen { get { return Colors.FromUInt32(0xFF00FA9A); } }

    /// <summary>
    /// The medium turquoise.
    /// </summary>
    public Color MediumTurquoise { get { return Colors.FromUInt32(0xFF48D1CC); } }

    /// <summary>
    /// The medium violet red.
    /// </summary>
    public Color MediumVioletRed { get { return Colors.FromUInt32(0xFFC71585); } }

    /// <summary>
    /// The midnight blue.
    /// </summary>
    public Color MidnightBlue { get { return Colors.FromUInt32(0xFF191970); } }

    /// <summary>
    /// The mint cream.
    /// </summary>
    public Color MintCream { get { return Colors.FromUInt32(0xFFF5FFFA); } }

    /// <summary>
    /// The misty rose.
    /// </summary>
    public Color MistyRose { get { return Colors.FromUInt32(0xFFFFE4E1); } }

    /// <summary>
    /// The moccasin.
    /// </summary>
    public Color Moccasin { get { return Colors.FromUInt32(0xFFFFE4B5); } }

    /// <summary>
    /// The navajo white.
    /// </summary>
    public Color NavajoWhite { get { return Colors.FromUInt32(0xFFFFDEAD); } }

    /// <summary>
    /// The navy.
    /// </summary>
    public Color Navy { get { return Colors.FromUInt32(0xFF000080); } }

    /// <summary>
    /// The old lace.
    /// </summary>
    public Color OldLace { get { return Colors.FromUInt32(0xFFFDF5E6); } }

    /// <summary>
    /// The olive.
    /// </summary>
    public Color Olive { get { return Colors.FromUInt32(0xFF808000); } }

    /// <summary>
    /// The olive drab.
    /// </summary>
    public Color OliveDrab { get { return Colors.FromUInt32(0xFF6B8E23); } }

    /// <summary>
    /// The orange.
    /// </summary>
    public Color Orange { get { return Colors.FromUInt32(0xFFFFA500); } }

    /// <summary>
    /// The orange red.
    /// </summary>
    public Color OrangeRed { get { return Colors.FromUInt32(0xFFFF4500); } }

    /// <summary>
    /// The orchid.
    /// </summary>
    public Color Orchid { get { return Colors.FromUInt32(0xFFDA70D6); } }

    /// <summary>
    /// The pale goldenrod.
    /// </summary>
    public Color PaleGoldenrod { get { return Colors.FromUInt32(0xFFEEE8AA); } }

    /// <summary>
    /// The pale green.
    /// </summary>
    public Color PaleGreen { get { return Colors.FromUInt32(0xFF98FB98); } }

    /// <summary>
    /// The pale turquoise.
    /// </summary>
    public Color PaleTurquoise { get { return Colors.FromUInt32(0xFFAFEEEE); } }

    /// <summary>
    /// The pale violet red.
    /// </summary>
    public Color PaleVioletRed { get { return Colors.FromUInt32(0xFFDB7093); } }

    /// <summary>
    /// The papaya whip.
    /// </summary>
    public Color PapayaWhip { get { return Colors.FromUInt32(0xFFFFEFD5); } }

    /// <summary>
    /// The peach puff.
    /// </summary>
    public Color PeachPuff { get { return Colors.FromUInt32(0xFFFFDAB9); } }

    /// <summary>
    /// The peru.
    /// </summary>
    public Color Peru { get { return Colors.FromUInt32(0xFFCD853F); } }

    /// <summary>
    /// The pink.
    /// </summary>
    public Color Pink { get { return Colors.FromUInt32(0xFFFFC0CB); } }

    /// <summary>
    /// The plum.
    /// </summary>
    public Color Plum { get { return Colors.FromUInt32(0xFFDDA0DD); } }

    /// <summary>
    /// The powder blue.
    /// </summary>
    public Color PowderBlue { get { return Colors.FromUInt32(0xFFB0E0E6); } }

    /// <summary>
    /// The purple.
    /// </summary>
    public Color Purple { get { return Colors.FromUInt32(0xFF800080); } }

    /// <summary>
    /// The red.
    /// </summary>
    public Color Red { get { return Colors.FromUInt32(0xFFFF0000); } }

    /// <summary>
    /// The rosy brown.
    /// </summary>
    public Color RosyBrown { get { return Colors.FromUInt32(0xFFBC8F8F); } }

    /// <summary>
    /// The royal blue.
    /// </summary>
    public Color RoyalBlue { get { return Colors.FromUInt32(0xFF4169E1); } }

    /// <summary>
    /// The saddle brown.
    /// </summary>
    public Color SaddleBrown { get { return Colors.FromUInt32(0xFF8B4513); } }

    /// <summary>
    /// The salmon.
    /// </summary>
    public Color Salmon { get { return Colors.FromUInt32(0xFFFA8072); } }

    /// <summary>
    /// The sandy brown.
    /// </summary>
    public Color SandyBrown { get { return Colors.FromUInt32(0xFFF4A460); } }

    /// <summary>
    /// The sea green.
    /// </summary>
    public Color SeaGreen { get { return Colors.FromUInt32(0xFF2E8B57); } }

    /// <summary>
    /// The sea shell.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SeaShell",
      Justification = "Корректное название цвета.")]
    public Color SeaShell { get { return Colors.FromUInt32(0xFFFFF5EE); } }

    /// <summary>
    /// The sienna.
    /// </summary>
    public Color Sienna { get { return Colors.FromUInt32(0xFFA0522D); } }

    /// <summary>
    /// The silver.
    /// </summary>
    public Color Silver { get { return Colors.FromUInt32(0xFFC0C0C0); } }

    /// <summary>
    /// The sky blue.
    /// </summary>
    public Color SkyBlue { get { return Colors.FromUInt32(0xFF87CEEB); } }

    /// <summary>
    /// The slate blue.
    /// </summary>
    public Color SlateBlue { get { return Colors.FromUInt32(0xFF6A5ACD); } }

    /// <summary>
    /// The slate gray.
    /// </summary>
    public Color SlateGray { get { return Colors.FromUInt32(0xFF708090); } }

    /// <summary>
    /// The snow.
    /// </summary>
    public Color Snow { get { return Colors.FromUInt32(0xFFFFFAFA); } }

    /// <summary>
    /// The spring green.
    /// </summary>
    public Color SpringGreen { get { return Colors.FromUInt32(0xFF00FF7F); } }

    /// <summary>
    /// The steel blue.
    /// </summary>
    public Color SteelBlue { get { return Colors.FromUInt32(0xFF4682B4); } }

    /// <summary>
    /// The tan.
    /// </summary>
    public Color Tan { get { return Colors.FromUInt32(0xFFD2B48C); } }

    /// <summary>
    /// The teal.
    /// </summary>
    public Color Teal { get { return Colors.FromUInt32(0xFF008080); } }

    /// <summary>
    /// The thistle.
    /// </summary>
    public Color Thistle { get { return Colors.FromUInt32(0xFFD8BFD8); } }

    /// <summary>
    /// The tomato.
    /// </summary>
    public Color Tomato { get { return Colors.FromUInt32(0xFFFF6347); } }

    /// <summary>
    /// The turquoise.
    /// </summary>
    public Color Turquoise { get { return Colors.FromUInt32(0xFF40E0D0); } }

    /// <summary>
    /// The violet.
    /// </summary>
    public Color Violet { get { return Colors.FromUInt32(0xFFEE82EE); } }

    /// <summary>
    /// The wheat.
    /// </summary>
    public Color Wheat { get { return Colors.FromUInt32(0xFFF5DEB3); } }

    /// <summary>
    /// The white.
    /// </summary>
    public Color White { get { return Colors.FromUInt32(0xFFFFFFFF); } }

    /// <summary>
    /// The white smoke.
    /// </summary>
    public Color WhiteSmoke { get { return Colors.FromUInt32(0xFFF5F5F5); } }

    /// <summary>
    /// The yellow.
    /// </summary>
    public Color Yellow { get { return Colors.FromUInt32(0xFFFFFF00); } }

    /// <summary>
    /// The yellow green.
    /// </summary>
    public Color YellowGreen { get { return Colors.FromUInt32(0xFF9ACD32); } }

    #endregion
  }

  /// <summary>
  /// Палитра цветов графиков.
  /// </summary>
  public class ChartColors
  {
    #region Конструктор

    /// <summary>
    /// Конструктор.
    /// </summary>
    internal ChartColors()
    {
      this.all = new Color[]
      {
        this.Color1,
        this.Color2,
        this.Color3,
        this.Color4,
        this.Color5,
        this.Color6,
        this.Color7,
        this.Color8,
        this.Color9,
        this.Color10,
      };
    }

    #endregion

    #region Цвета

    /// <summary>
    /// Получить цвет по индексу.
    /// </summary>
    /// <param name="index">Индекс.</param>
    /// <returns>Цвет.</returns>
    public Color GetByIndex(int index)
    {
      return this.all[index % this.all.Length];
    }

    private readonly Color[] all;

    /// <summary>
    /// Все цвета графиков.
    /// </summary>
    public IReadOnlyCollection<Color> All { get { return this.all; } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color1 { get { return Colors.Parse("#FF6EA5D4"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color2 { get { return Colors.Parse("#FF8DC7F8"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color3 { get { return Colors.Parse("#FF99CA51"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color4 { get { return Colors.Parse("#FF78A733"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color5 { get { return Colors.Parse("#FF53A4A6"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color6 { get { return Colors.Parse("#FF6BB3AF"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color7 { get { return Colors.Parse("#FF8C6AB0"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color8 { get { return Colors.Parse("#FFAC8FCC"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color9 { get { return Colors.Parse("#FFB46CA2"); } }

    /// <summary>
    /// Chart color.
    /// </summary>
    public Color Color10 { get { return Colors.Parse("#FFCC8FBD"); } }

    /// <summary>
    /// Цвет для красных значений.
    /// </summary>
    public Color Red { get { return Colors.Parse("#FFD63540"); } }

    /// <summary>
    /// Цвет для зеленых значений.
    /// </summary>
    public Color Green { get { return Colors.Parse("#FF78A733"); } }

    /// <summary>
    /// Цвет для желтых значений.
    /// </summary>
    public Color Yellow { get { return Colors.Parse("#FFFFAE18"); } }

    #endregion
  }
}
