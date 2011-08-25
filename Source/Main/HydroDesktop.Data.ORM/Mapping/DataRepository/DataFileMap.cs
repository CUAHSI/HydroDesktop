using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class DataFileMap : ClassMap<DataFile>
    {
        public DataFileMap()
        {
            Table("DataFiles");

            Id(x => x.Id).Column("FileID");
            Map(x => x.FileName).Not.Nullable();
            Map(x => x.FileType).Not.Nullable();
            Map(x => x.FileDescription).Not.Nullable();
            Map(x => x.FilePath).Not.Nullable();
            Map(x => x.FileOrigin).Not.Nullable();
            Map(x => x.LoadMethod).Not.Nullable();
            Map(x => x.LoadDateTime).Not.Nullable();

            References(x => x.QueryInfo).Column("QueryID")
                                        .Cascade.SaveUpdate();
        }
    }
}
