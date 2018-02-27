namespace services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WaterTempIdxReadingDateTime : DbMigration
    {
        public override void Up()
        {
            Sql(@"

IF NOT EXISTS (SELECT 'foo' FROM sys.indexes WHERE name = 'WATERTEMP_READINGDATETIME_IDX')
CREATE NONCLUSTERED INDEX WATERTEMP_READINGDATETIME_IDX ON [dbo].[WaterTemp_Detail](  [ReadingDateTime] ASC );

IF NOT EXISTS (SELECT 'foo' FROM sys.indexes WHERE name = 'WATERTEMP_VIEW_IDX')
CREATE NONCLUSTERED INDEX WATERTEMP_VIEW_IDX ON[dbo].[WaterTemp_Detail] 
([RowId],[RowStatusId],[ActivityId],[EffDt])
INCLUDE([Id],[ReadingDateTime],[WaterTemperature],[WaterTemperatureF],[WaterLevel],[TempAToD],[BatteryVolts],[ByUserId],[QAStatusId],[AirTemperature],[AirTemperatureF],[GMTReadingDateTime],[Conductivity],[PSI],[AbsolutePressure])
;

IF NOT EXISTS (SELECT 'foo' FROM sys.indexes WHERE name = 'DATASET_FIELDS_IDX')
CREATE NONCLUSTERED INDEX DATASET_FIELDS_IDX ON [dbo].[DatasetFields]
(
       [DatasetId] ASC
)
INCLUDE (    [Id],
       [FieldId],
       [FieldRoleId],
       [CreateDateTime],
       [Label],
       [DbColumnName],
       [Validation],
       [SourceId],
       [InstrumentId],
       [OrderIndex],
       [ControlType],
       [Rule]);");

        }
        
        public override void Down()
        {
            Sql(@"
DROP INDEX DATASET_FIELDS_IDX ON [dbo].[DatasetFields]
go

DROP INDEX WATERTEMP_VIEW_IDX ON[dbo].[WaterTemp_Detail]
go

DROP INDEX WATERTEMP_READINGDATETIME_IDX ON [dbo].[WaterTemp_Detail]
go
            ");
        }
    }
}