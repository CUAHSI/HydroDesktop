using System;

namespace Search3.Searching.Exceptions
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