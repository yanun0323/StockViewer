using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace StockViewer.MVVM.View;
public class PopupLocator
{
    public static DependencyObject GetLocator(DependencyObject obj)
    {
        return (DependencyObject)obj.GetValue(LocatorProperty);
    }

    public static void SetLocator(DependencyObject obj, DependencyObject value)
    {
        obj.SetValue(LocatorProperty, value);
    }

    public static readonly DependencyProperty LocatorProperty =
        DependencyProperty.RegisterAttached("Locator", typeof(DependencyObject), typeof(PopupLocator), new PropertyMetadata(null, OnLocatorChanged));

    private static void OnLocatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            DependencyObject popupPopupPlacementTarget = (DependencyObject)e.NewValue;
            Popup pop = (Popup)d;

            Window w = Window.GetWindow(popupPopupPlacementTarget);
            if (null != w)
            {
                //让Popup随着窗体的移动而移动
                w.LocationChanged += delegate
                {
                    var mi = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if(pop.IsOpen)
                        mi!.Invoke(pop, null);
                };
                //让Popup随着窗体的Size改变而移动位置
                w.SizeChanged += delegate
                {
                    var mi = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (pop.IsOpen)
                        mi!.Invoke(pop, null);
                };
            }
        }
    }
}
