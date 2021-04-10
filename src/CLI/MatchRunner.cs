using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Painter.Data;
using Painter.Domain;
using Painter.Match;

namespace Painter.CLI {
    public class MatchRunner {
        public static void Execute(MatchOptions opts) {
            Color customColor = ColorTranslator.FromHtml("#" + opts.HexCode.Replace("#", ""));
            using ColorContext colorContext = new ColorContext(opts.DbFile);
            List<ColorSwatch> colorSwatches = colorContext.ColorSwatches.Include(s => s.ColorNumbers).ToList();
            IOrderedEnumerable<ColorMatch> colorMatches = colorSwatches.Select(x => new ColorMatch(customColor, x)).OrderBy(x => x.MatchError);
            foreach (ColorMatch colorMatch in colorMatches.Take(opts.Top)) {
                Console.WriteLine(colorMatch);
            }
        }
    }
}
