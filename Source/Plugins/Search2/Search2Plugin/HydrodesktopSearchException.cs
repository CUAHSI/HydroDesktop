using System;

namespace HydroDesktop.Search
{
 public class HydrodesktopSearchException: Exception
 {
     public HydrodesktopSearchException():base()
     {
     }
     public HydrodesktopSearchException(string a ): base(a)
     {}
      public HydrodesktopSearchException(string a, Exception ex ): base(a, ex)
     {}
 }

}