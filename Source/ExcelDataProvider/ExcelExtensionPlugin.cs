using DotSpatial.Controls;
using DotSpatial.Data;

namespace ExcelExtension
{
    public class ExcelExtensionPlugin : Extension
    {
        public override void Activate()
        {
            var dataProvider = new ExcelVectorProvider();
            foreach (var extension in ExcelVectorProvider.Extensions)
            {
                if (!DataManager.DefaultDataManager.PreferredProviders.ContainsKey(extension))
                {
                    DataManager.DefaultDataManager.PreferredProviders.Add(extension, dataProvider);
                }
            }
            
            base.Activate();
        }

        public override void Deactivate()
        {
            foreach (var extension in ExcelVectorProvider.Extensions)
            {
                DataManager.DefaultDataManager.PreferredProviders.Remove(extension);
            }

            base.Deactivate();
        }
    }
}
