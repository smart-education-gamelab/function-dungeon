using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;


namespace ExternalQuestionEditor {
    public class CustomSlider : Slider {
        public bool dragging;
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            Track track = Template.FindName("PART_Track", this) as Track;
            Thumb thumb = track.Thumb;
            thumb.MouseEnter += new MouseEventHandler(ThumbMouseEnter);
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(SliderThumbDragStarted));
            AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(SliderThumbDragCompleted));
        }

        private void ThumbMouseEnter(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed
                && e.MouseDevice.Captured == null) {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left) {
                    RoutedEvent = MouseLeftButtonDownEvent
                };
                (sender as Thumb).RaiseEvent(args);
            }
        }

        private void SliderThumbDragStarted(object sender, DragStartedEventArgs e) {
            dragging = true;
        }

        private void SliderThumbDragCompleted(object sender, DragCompletedEventArgs e) {
            dragging = false;
        }
    }
}
