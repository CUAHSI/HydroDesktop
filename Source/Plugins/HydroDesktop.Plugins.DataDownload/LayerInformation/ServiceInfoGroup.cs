using System;
using System.Collections.Generic;
using System.Linq;

namespace HydroDesktop.Plugins.DataDownload.LayerInformation
{
    public class ServiceInfoGroup
    {
        private readonly HashSet<ServiceInfo> _data = new HashSet<ServiceInfo>();

        public void AddOverlappedServiceInfo(ServiceInfo serviceInfo)
        {
            if (serviceInfo == null) throw new ArgumentNullException("serviceInfo");

            if (_data.Contains(serviceInfo)) return;

            if (_data.Count == 0)
            {
                _data.Add(serviceInfo);
                return;
            }

            var first = _data.First();
            if (IsOverlapped(first, serviceInfo))
                _data.Add(serviceInfo);
        }

        public static bool IsOverlapped(ServiceInfo first, ServiceInfo second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return first.SiteCode == second.SiteCode && 
                   first.VarCode != second.VarCode;
        }

        public IEnumerable<ServiceInfo> GetItems()
        {
            return _data.AsEnumerable();
        }

        public int ItemsCount
        {
            get { return _data.Count; }
        }

        public bool IsEmpty
        {
            get
            {
                return _data.Count == 0 ||
                       _data.All(el => el.IsEmpty);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var pi = obj as ServiceInfoGroup;
            if (pi == null) return false;

            if (pi._data.Count != _data.Count) return false;

            return pi._data.Select(el => _data.FirstOrDefault(item => item.Equals(el))).All(find => find != null);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (_data != null ? _data.GetHashCode() : 0);
        }

        public static ServiceInfoGroup Create(IEnumerable<ServiceInfo> potentialOverlappedPoints)
        {
            if (potentialOverlappedPoints == null) throw new ArgumentNullException("potentialOverlappedPoints");

            var result = new ServiceInfoGroup();
            foreach (var point in potentialOverlappedPoints)
                result.AddOverlappedServiceInfo(point);

            return result;
        }
    }
}
