using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Pen = System.Windows.Media.Pen;

namespace ExternalQuestionEditor {
    public class CustomTickBar : TickBar {
        private readonly static Brush col = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7f7f7f"));
        private readonly static Brush middle = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff3f3f3f"));
        protected override void OnRender(DrawingContext dc) {
            double num = this.Maximum - this.Minimum;
            int index = 0;
            if (Ticks.Count == 2) {
                double tick0 = 5 + (Ticks[0] / num) * (ActualWidth - 10);
                double tick1 = 5 + (Ticks[1] / num) * (ActualWidth - 10);
                dc.DrawRectangle(middle, null, new System.Windows.Rect(tick0 + 1, 0, tick1 - tick0, 6));
            }
            foreach (var tick in Ticks) {
                dc.DrawEllipse(col, null, new System.Windows.Point((5 + (tick / num) * (ActualWidth - 10)) - 1.5, 3), 3, 3);
                index++;
            }
        }
    }
}
