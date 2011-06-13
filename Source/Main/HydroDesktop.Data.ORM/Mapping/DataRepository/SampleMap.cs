using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class SampleMap : ClassMap<Sample>
    {
        public SampleMap()
        {
            Table("Samples");
            
            Id(x => x.Id).Column("SampleID");
            
            Map(x => x.LabSampleCode).Column("LabSampleCode")
                                     .Not.Nullable();
            
            Map(x => x.SampleType).Not.Nullable();

            References(x => x.LabMethod)
                .Column("LabMethodID")
                .Not.Nullable().Fetch.Join();
        }
    }
}
