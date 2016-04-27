using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using DotNet.Highcharts.Options;

namespace Catalog.Helpers
{
  /// <summary>
  /// Палитра цветов графиков.
  /// </summary>
  public static class ChartColors
  {
    #region Конструктор

    /// <summary>
    /// Конструктор.
    /// </summary>
    static ChartColors()
    {
      all = new []
      {
        Color1,
        Color2,
        Color3,
        Color4,
        Color5,
        Color6,
        Color7,
        Color8,
        Color9,
        Color10,
      };
    }

    #endregion

    public static Color Parse(string hex)
    {
      return System.Drawing.ColorTranslator.FromHtml(hex);
    }

    #region Цвета

    /// <summary>
    /// Получить цвет по индексу.
    /// </summary>
    /// <param name="index">Индекс.</param>
    /// <returns>Цвет.</returns>
    public static Color GetByIndex(int index)
    {
      return all[index % all.Length];
    }

    private static readonly Color[] all;

    /// <summary>
    /// Все цвета графиков.
    /// </summary>
    public static IReadOnlyCollection<Color> All => all;

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color1 => ColorTranslator.FromHtml("#FF6EA5D4");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color2 => ColorTranslator.FromHtml("#FF8DC7F8");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color3 => ColorTranslator.FromHtml("#FF99CA51");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color4 => ColorTranslator.FromHtml("#FF78A733");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color5 => ColorTranslator.FromHtml("#FF53A4A6");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color6 => ColorTranslator.FromHtml("#FF6BB3AF");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color7 => ColorTranslator.FromHtml("#FF8C6AB0");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color8 => ColorTranslator.FromHtml("#FFAC8FCC");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color9 => ColorTranslator.FromHtml("#FFB46CA2");

    /// <summary>
    /// Chart color.
    /// </summary>
    public static Color Color10 => ColorTranslator.FromHtml("#FFCC8FBD");

    /// <summary>
    /// Цвет для красных значений.
    /// </summary>
    public static Color Red => ColorTranslator.FromHtml("#FFD63540");

    /// <summary>
    /// Цвет для зеленых значений.
    /// </summary>
    public static Color Green => ColorTranslator.FromHtml("#FF78A733");

    /// <summary>
    /// Цвет для желтых значений.
    /// </summary>
    public static Color Yellow => ColorTranslator.FromHtml("#FFFFAE18");

    #endregion
  }
}
