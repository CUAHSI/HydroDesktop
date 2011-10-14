using System;

namespace Search3.Settings
{
    public class AreaSettings
    {
        public bool HasAnyArea
        {
            get { return AreaRectangle != null; }
        }

        private AreaRectangle _areaRectangle;
        public AreaRectangle AreaRectangle
        {
            get { return _areaRectangle; }
            set
            {
                _areaRectangle = value;
                RaiseAreaRectangleChanged();
            }
        }

        public event EventHandler AreaRectangleChanged;

        private void RaiseAreaRectangleChanged()
        {
            var handler = AreaRectangleChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}