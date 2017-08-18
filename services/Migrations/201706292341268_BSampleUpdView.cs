namespace services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BSampleUpdView : DbMigration
    {
        public override void Up()
        {
            Sql(@"
drop view BSample_vw
GO

create view BSample_vw
AS
SELECT        a.Id AS ActivityId, a.DatasetId, a.SourceId, a.LocationId, a.UserId, a.ActivityTypeId, a.CreateDate, a.ActivityDate, h.Id AS BSample_Header_Id, h.SampleYear, h.Technicians, 
                         h.HeaderComments, h.CollectionType, h.ByUserId AS BSample_Header_ByUserId, h.EffDt, d.Id AS BSample_Detail_Id, d.Sex, d.Mark, d.ForkLength, d.TotalLength, 
                         d.Weight, d.GeneticSampleId, d.ScaleId, d.SnoutId, d.LifeStage, d.Origin, d.Species, d.PITTagId, d.RadioTagId, d.FishComments, d.OtherTagId, d.KidneyId, d.Tag, 
                         d.PercentRetained, d.FinClip, d.TotalCount, d.RecordNumber, d.MEHPLength, d.SubSample, d.RowId, d.RowStatusId, d.ByUserId, d.QAStatusId, 
                         d.EffDt AS BSample_Detail_EffDt, q.QAStatusId AS ActivityQAStatusId, q.Comments, q.QAStatusName, q.UserId AS ActivityQAUserId, l.Label AS LocationLabel
FROM            dbo.Activities AS a INNER JOIN
                         dbo.BSample_Header_VW AS h ON a.Id = h.ActivityId INNER JOIN
                         dbo.BSample_Detail_VW AS d ON a.Id = d.ActivityId INNER JOIN
                         dbo.ActivityQAs_VW AS q ON a.Id = q.ActivityId INNER JOIN
                         dbo.Locations AS l ON a.LocationId = l.Id
GO
            ");
        }

        public override void Down()
        {
            Sql(@"
drop view BSample_vw
GO

create view BSample_vw
AS
SELECT        a.Id, a.DatasetId, a.SourceId, a.LocationId, a.UserId, a.ActivityTypeId, a.CreateDate, a.ActivityDate, h.Id AS BSample_Header_Id, h.SampleYear, h.Technicians, 
                         h.HeaderComments, h.CollectionType, h.ByUserId AS BSample_Header_ByUserId, h.EffDt, d.Id AS BSample_Detail_Id, d.Sex, d.Mark, d.ForkLength, d.TotalLength, 
                         d.Weight, d.GeneticSampleId, d.ScaleId, d.SnoutId, d.LifeStage, d.Origin, d.Species, d.PITTagId, d.RadioTagId, d.FishComments, d.OtherTagId, d.KidneyId, d.Tag, 
                         d.PercentRetained, d.FinClip, d.TotalCount, d.RecordNumber, d.MEHPLength, d.SubSample, d.RowId, d.RowStatusId, d.ByUserId, d.QAStatusId, 
                         d.EffDt AS BSample_Detail_EffDt, q.QAStatusId AS ActivityQAStatusId, q.Comments, q.QAStatusName, q.UserId AS ActivityQAUserId, l.Label AS LocationLabel
FROM            dbo.Activities AS a INNER JOIN
                         dbo.BSample_Header_VW AS h ON a.Id = h.ActivityId INNER JOIN
                         dbo.BSample_Detail_VW AS d ON a.Id = d.ActivityId INNER JOIN
                         dbo.ActivityQAs_VW AS q ON a.Id = q.ActivityId INNER JOIN
                         dbo.Locations AS l ON a.LocationId = l.Id
GO
            ");
        }
    }
}
