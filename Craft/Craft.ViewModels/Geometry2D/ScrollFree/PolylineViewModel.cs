﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree;

public class PolylineViewModel
{
    public string Points { get; set; }
    public double Thickness { get; }
    public Brush Brush { get; }

    public PolylineViewModel(
        IEnumerable<PointD> points,
        double thickness,
        Brush brush)
    {
        Points = string.Join(" ", points.Select(
            point => $"{string.Format(CultureInfo.InvariantCulture, "{0}", point.X)},{string.Format(CultureInfo.InvariantCulture, "{0}", point.Y)}"));
        Thickness = thickness;
        Brush = brush;
    }
}